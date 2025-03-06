using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "GenericUpgrade", menuName = "Scripts/Upgrades/Generic Upgrade")]
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
        // Generic ability interaction logic can go here, or be overridden in subclasses if needed.
        // For example, look for InputActivationComponent and ability components and wire them up.

        InputActivationComponent inputActivation = runtimeComponents.OfType<InputActivationComponent>().FirstOrDefault();
        if (inputActivation != null)
        {
            // Try to find a component that has an "Activate" method that can be used as the delegate
            foreach (var component in runtimeComponents)
            {
                if (component != inputActivation) // Don't wire input to itself!
                {
                    MethodInfo activateMethod = component.GetType().GetMethod("Activate", new[] { typeof(GameObject) }); // Find Activate(GameObject) method

                    if (activateMethod != null)
                    {
                        // Create a delegate to the Activate method
                        var activateDelegate = Delegate.CreateDelegate(typeof(Action<GameObject>), component, activateMethod);

                        if (activateDelegate is Action<GameObject> actionDelegate)
                        {
                            inputActivation.onActivate = actionDelegate;
                            Debug.Log($"Wired InputActivationComponent to {component.GetType().Name}.Activate");
                            return; // Stop after wiring up to the first ability component found
                        }
                    }
                }
            }
            Debug.LogWarning("InputActivationComponent found, but no compatible ability component with Activate(GameObject) method to wire to.");
        }
    }

    public virtual List<IUpgradeComponent> CreateRuntimeComponentsAndConfigure()
    {
        var runtimeComponents = CreateRuntimeComponents();
        ConfigureRuntimeComponentDependencies(runtimeComponents); // Call the dependency configuration
        return runtimeComponents;
    }

    // Helper function to copy serialized fields using reflection
    private void CopySerializedFields(object source, object destination)
    {
        if (source == null || destination == null) return;

        Type sourceType = source.GetType();
        Type destinationType = destination.GetType();

        if (sourceType != destinationType)
        {
            Debug.LogError($"Source and destination types do not match: {sourceType.Name} vs {destinationType.Name}");
            return;
        }

        FieldInfo[] fields = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false)) // Check for [SerializeField] attribute
            {
                field.SetValue(destination, field.GetValue(source));
            }
            // You might also want to copy public fields if you intend to configure them in the editor
            // else if (field.IsPublic && !field.IsStatic) { // Optional: copy public fields
            //     field.SetValue(destination, field.GetValue(source));
            // }
        }
    }
}