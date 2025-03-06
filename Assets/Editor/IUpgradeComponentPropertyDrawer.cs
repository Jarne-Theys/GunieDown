using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(IUpgradeComponent), true)] // Apply to IUpgradeComponent and derived classes
public class IUpgradeComponentPropertyDrawer : PropertyDrawer
{
    private Type[] _componentTypes; // Cache of available component types
    private string[] _typeNameOptions; // Names for dropdown
    private bool _isInitialized = false;

    private void Initialize()
    {
        if (_isInitialized) return;

        // Find all classes that implement IUpgradeComponent and are not abstract
        _componentTypes = Assembly.GetAssembly(typeof(IUpgradeComponent))
            .GetTypes()
            .Where(t => typeof(IUpgradeComponent).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToArray();

        _typeNameOptions = _componentTypes.Select(t => t.Name).ToArray();
        _isInitialized = true;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize(); // Initialize on first GUI draw

        EditorGUI.BeginProperty(position, label, property);

        Rect objectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        if (property.managedReferenceValue == null)
        {
            // Draw "Add Component" button if element is null
            if (GUI.Button(objectRect, "Add Component"))
            {
                ShowAddComponentMenu(objectRect, property);
            }
        }
        else
        {
            // Get the component type name for the label
            string componentTypeNamePascalCase = property.managedReferenceValue.GetType().Name;
            string componentTypeNameFormatted = ObjectNames.NicifyVariableName(componentTypeNamePascalCase);
            GUIContent componentLabel = new GUIContent(componentTypeNameFormatted);

            // Draw the foldout and default property field if component exists
            property.isExpanded = EditorGUI.Foldout(objectRect, property.isExpanded, componentLabel); // Use componentTypeName for foldout label
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, property, GUIContent.none, true); // Pass GUIContent.none to avoid extra label
                EditorGUI.indentLevel--;
            }
            else
            {
                // Just draw object field (collapsed)
                EditorGUI.PropertyField(objectRect, property, componentLabel, false); // Use componentTypeName for collapsed label
            }
        }


        EditorGUI.EndProperty();
    }


    private void ShowAddComponentMenu(Rect buttonRect, SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();

        for (int i = 0; i < _componentTypes.Length; i++)
        {
            int index = i; // Capture index for the lambda
            menu.AddItem(new GUIContent(_typeNameOptions[i]), false, () => ReplaceComponentInstance(property, _componentTypes[index]));
        }

        menu.DropDown(buttonRect);
    }

    private void ReplaceComponentInstance(SerializedProperty property, Type componentType)
    {
        // Create an instance of the selected component type and set it as the element's value
        object instance = Activator.CreateInstance(componentType);
        property.managedReferenceValue = instance;

        property.serializedObject.ApplyModifiedProperties(); // Important to apply changes!
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue != null && property.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(property, GUIContent.none, true); // Use GUIContent.none for height calculation too when expanded
        }
        return EditorGUIUtility.singleLineHeight; // Single line height when collapsed or for "Add Component" button
    }
}