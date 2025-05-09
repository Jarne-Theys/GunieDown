using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeDefinition), true)]
public class UpgradeDefinitionEditor : Editor
{
    private SerializedProperty componentsProperty;

    private void OnEnable()
    {
        componentsProperty = serializedObject.FindProperty("components");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}