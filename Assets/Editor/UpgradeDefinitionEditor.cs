using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeDefinition), true)] // Apply to UpgradeDefinition and subclasses
public class UpgradeDefinitionEditor : Editor
{
    private SerializedProperty componentsProperty;

    private void OnEnable()
    {
        componentsProperty = serializedObject.FindProperty("components");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Update SerializedObject at the start

        DrawDefaultInspector(); // Draw other properties of UpgradeDefinition

        // Draw the components list using EditorGUILayout.PropertyField, which will use the IUpgradeComponentPropertyDrawer
        //EditorGUILayout.PropertyField(componentsProperty, true);


        serializedObject.ApplyModifiedProperties(); // Apply changes at the end
    }
}