using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "GenericUpgrade", menuName = "Upgrades/Upgrade base")]
public class UpgradeDefinition : ScriptableObject
{
    public string upgradeName;
    public string description;

    [SerializeReference]
    public List<IUpgradeComponent> components = new List<IUpgradeComponent>();

    public void ApplyUpgrade(GameObject player)
    {
        foreach (var component in components)
        {
            component.ApplyPassive(player);
        }
    }

    public virtual List<IUpgradeComponent> CreateRuntimeComponents()
    {
        List<IUpgradeComponent> runtimeComponents = new List<IUpgradeComponent>();

        foreach (var editorComponent in components)
        {
            if (editorComponent == null) continue;

            IUpgradeComponent runtimeComponent = Activator.CreateInstance(editorComponent.GetType()) as IUpgradeComponent;

            if (runtimeComponent != null)
            {
                CopySerializedFields(editorComponent, runtimeComponent);
                runtimeComponents.Add(runtimeComponent);
            }
            else
            {
                Debug.LogWarning($"Failed to create runtime instance of component type: {editorComponent.GetType().Name}");
            }
        }
        return runtimeComponents;
    }


    protected virtual void ConfigureRuntimeComponentDependencies(List<IUpgradeComponent> runtimeComponents)
    {
        InputActivationComponent inputActivation = runtimeComponents.OfType<InputActivationComponent>().FirstOrDefault();
        if (inputActivation != null)
        {
            // Set the runtime components list on the InputActivationComponent
            inputActivation.SetRuntimeComponents(runtimeComponents);

            // Create a list to hold all Activate actions with the new signature
            List<Action<GameObject, List<IUpgradeComponent>>> activateActions = new List<Action<GameObject, List<IUpgradeComponent>>>(); // Modified delegate type

            // Find all components with an "Activate" method (excluding InputActivationComponent itself)
            foreach (var component in runtimeComponents)
            {
                if (component != inputActivation) // Don't wire input to itself!
                {
                    MethodInfo activateMethod = component.GetType().GetMethod("Activate", new[] { typeof(GameObject), typeof(List<IUpgradeComponent>) }); // Find Activate(GameObject, List<IUpgradeComponent>) method

                    if (activateMethod != null)
                    {
                        // Create a delegate to the Activate method with the new signature
                        var activateDelegate = Delegate.CreateDelegate(typeof(Action<GameObject, List<IUpgradeComponent>>), component, activateMethod) as Action<GameObject, List<IUpgradeComponent>>; // Modified delegate type

                        if (activateDelegate != null) // Check if delegate creation was successful
                        {
                            activateActions.Add(activateDelegate); // Add the delegate to the list
                            // Debug.Log($"Found Activate method on {component.GetType().Name}, adding to activation list.");
                        }
                    }
                }
            }

            if (activateActions.Count > 0)
            {
                // Combine all Activate actions into a single delegate using lambda with the new signature
                inputActivation.OnActivate = (GameObject player, List<IUpgradeComponent> components) => // Modified lambda signature
                {
                    foreach (var action in activateActions)
                    {
                        action?.Invoke(player, components); // Invoke each Activate action, passing components list
                    }
                };
                // Debug.Log($"Combined and wired {activateActions.Count} Activate methods to InputActivationComponent.");
            }
            else
            {
                Debug.LogWarning("InputActivationComponent found, but no compatible ability components with Activate(GameObject, List<IUpgradeComponent>) method to wire to.");
            }
        }
    }

    public List<IUpgradeComponent> CreateRuntimeComponentsAndConfigure()
    {
        var runtimeComponents = CreateRuntimeComponents();
        ConfigureRuntimeComponentDependencies(runtimeComponents); // Call the dependency configuration
        return runtimeComponents;
    }

    // Helper function to copy serialized fields using reflection
    
    private void CopySerializedFields(object source, object destination)
    {
        if (source == null || destination == null) return;
        if (source.GetType() != destination.GetType())
        {
            Debug.LogError($"Source and destination types do not match: {source.GetType().Name} vs {destination.GetType().Name}");
            return;
        }

        // Turn your editor‐configured instance into JSON...
        string json = JsonUtility.ToJson(source, false);

        // ...and overwrite all serializable fields on the new runtime instance
        JsonUtility.FromJsonOverwrite(json, destination);

        // Optional: log out one field to confirm
        Debug.Log($"Component: {source.GetType()} \n JSON data: {json}");
    }
    
    
    // private void CopySerializedFields(object source, object destination)
    // {
    //     if (source == null || destination == null) return;
    //
    //     Type sourceType = source.GetType();
    //     Type destinationType = destination.GetType();
    //
    //     if (sourceType != destinationType)
    //     {
    //         Debug.LogError($"Source and destination types do not match: {sourceType.Name} vs {destinationType.Name}");
    //         return;
    //     }
    //
    //     FieldInfo[] fields = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //
    //     foreach (var field in fields)
    //     {
    //         if (field.IsDefined(typeof(SerializeField), false)) // Check for [SerializeField] attribute
    //         {
    //             field.SetValue(destination, field.GetValue(source));
    //             Debug.Log($"{field.Name} (type {field.FieldType.Name}) copied → value={field.GetValue(destination)}");
    //         }
    //         else
    //         {
    //             Debug.Log($"Not copying {field.Name} (type {field.FieldType.Name}) → original value={field.GetValue(source)}");
    //         }
    //         
    //
    //
    //         // You might also want to copy public fields if you intend to configure them in the editor
    //         // else if (field.IsPublic && !field.IsStatic) { // Optional: copy public fields
    //         //     field.SetValue(destination, field.GetValue(source));
    //         // }
    //     }
    // }
}