using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class UpgradeListUpdater : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool upgradesListUpdated = false;

        // Check if any relevant assets were changed (created, deleted, moved, imported)
        if (importedAssets.Any(assetPath => assetPath.EndsWith(".asset")) ||
            deletedAssets.Any(assetPath => assetPath.EndsWith(".asset")) ||
            movedAssets.Any(assetPath => assetPath.EndsWith(".asset")))
        {
            // Find the UpgradesList ScriptableObject
            string[] upgradesListGUIDs = AssetDatabase.FindAssets("t:UpgradesList"); // Find assets of type UpgradesList
            if (upgradesListGUIDs.Length > 0)
            {
                string upgradesListPath = AssetDatabase.GUIDToAssetPath(upgradesListGUIDs[0]); // Get path from GUID
                UpgradesList upgradesList = AssetDatabase.LoadAssetAtPath<UpgradesList>(upgradesListPath);

                if (upgradesList != null)
                {
                    // Find all UpgradeDefinition ScriptableObjects
                    string[] upgradeDefinitionGUIDs = AssetDatabase.FindAssets("t:UpgradeDefinition"); // Find assets of type UpgradeDefinition
                    UpgradeDefinition[] foundUpgrades = new UpgradeDefinition[upgradeDefinitionGUIDs.Length];

                    for (int i = 0; i < upgradeDefinitionGUIDs.Length; i++)
                    {
                        string upgradeDefinitionPath = AssetDatabase.GUIDToAssetPath(upgradeDefinitionGUIDs[i]);
                        foundUpgrades[i] = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(upgradeDefinitionPath);
                    }

                    // **Change Detection Logic:**
                    bool listChanged = false;
                    if (upgradesList.upgrades == null || upgradesList.upgrades.Length != foundUpgrades.Length)
                    {
                        listChanged = true; // Lengths are different, list has changed
                    }
                    else
                    {
                        // Compare element by element (assuming order doesn't matter, or you can sort if order matters)
                        for (int i = 0; i < foundUpgrades.Length; i++)
                        {
                            if (upgradesList.upgrades[i] != foundUpgrades[i])
                            {
                                listChanged = true; // An element is different, list has changed
                                break;
                            }
                        }
                    }

                    if (listChanged)
                    {
                        // Update the upgrades array in UpgradesList only if changed
                        upgradesList.upgrades = foundUpgrades;

                        // Mark UpgradesList as dirty and save changes
                        EditorUtility.SetDirty(upgradesList);
                        AssetDatabase.SaveAssets();

                        upgradesListUpdated = true;
                    }
                    else
                    {
                        Debug.Log("UpgradesList is already up-to-date. No update needed.");
                    }
                }
                else
                {
                    Debug.LogError("UpgradesList ScriptableObject not found at path: " + upgradesListPath);
                }
            }
            else
            {
                Debug.LogWarning("No UpgradesList ScriptableObject found in the project. Automatic upgrade list updating skipped.");
            }
        }

        if (upgradesListUpdated)
        {
            Debug.Log("UpgradesList automatically updated with UpgradeDefinitions.");
        }
    }
}