using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Scriptable Objects/UpgradeDefinition")]
public abstract class UpgradeDefinition : ScriptableObject
{
    public string upgradeName;
    public string description;

    [SerializeReference]
    public List<IUpgradeComponent> components = new List<IUpgradeComponent>(); // Keep this for editor configuration

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
            if (editorComponent == null) continue; // Handle potential null entries

            // 1. Create a new instance of the same component type
            IUpgradeComponent runtimeComponent = Activator.CreateInstance(editorComponent.GetType()) as IUpgradeComponent;

            if (runtimeComponent != null)
            {
                // 2. Copy serialized field values from the editor component to the runtime component
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