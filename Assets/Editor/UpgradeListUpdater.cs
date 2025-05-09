using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class UpgradeListUpdater : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool upgradesListUpdated = false;
        bool statUpgradesListUpdated = false;

        // Check if any relevant assets were changed (created, deleted, moved, imported)
        if (!importedAssets.Any(assetPath => assetPath.EndsWith(".asset")) &&
            !deletedAssets.Any(assetPath => assetPath.EndsWith(".asset")) &&
            !movedAssets.Any(assetPath => assetPath.EndsWith(".asset")))
        {
            return;
        }
        
        // StatUpgrade section
        
        string[] statUpgradesListGUIDs = AssetDatabase.FindAssets("t:StatUpgradesList");
        if (statUpgradesListGUIDs.Length <= 0)
        {
            Debug.LogWarning("No StatUpgradesList ScriptableObject found in the project. Automatic upgrade list updating skipped.");
        }
        
        string statUpgradesListPath = AssetDatabase.GUIDToAssetPath(statUpgradesListGUIDs[0]);
        StatUpgradesList statUpgradesList = AssetDatabase.LoadAssetAtPath<StatUpgradesList>(statUpgradesListPath);

        if (statUpgradesList == null)
        {
            Debug.LogError("StatUpgradesList ScriptableObject not found at: " + statUpgradesListPath);
        }

        // Upgrade section
        
        string[] upgradesListGUIDs = AssetDatabase.FindAssets("t:UpgradesList"); // Find assets of type UpgradesList
        if (upgradesListGUIDs.Length <= 0)
        {
            Debug.LogWarning(
                "No UpgradesList ScriptableObject found in the project. Automatic upgrade list updating skipped.");
        }
        
        string upgradesListPath = AssetDatabase.GUIDToAssetPath(upgradesListGUIDs[0]); // Get path from GUID
        UpgradesList upgradesList = AssetDatabase.LoadAssetAtPath<UpgradesList>(upgradesListPath);

        if (upgradesList == null)
        {
            Debug.LogError("UpgradesList ScriptableObject not found at path: " + upgradesListPath);
        }
        
        // Common logic
        // Doing the iteration over all upgrades once, and filtering them into the appropriate lists
        // instead of doing the loop twice, for better maintainability and performance
        
        // Find all UpgradeDefinition ScriptableObjects
        string[] upgradeDefinitionGUIDs = 
            AssetDatabase.FindAssets("t:UpgradeDefinition"); 
        UpgradeDefinition[] foundUpgrades = new UpgradeDefinition[upgradeDefinitionGUIDs.Length];

        string[] statUpgradeDefinitionGUIDs = AssetDatabase.FindAssets("t:UpgradeDefinition",
            new[] { "Assets/Scripts/Upgrades/Prefabs/Instances/Stats" }); 
        
        List<UpgradeDefinition> foundStatUpgrades = new List<UpgradeDefinition>();
            
        // This loops over all upgrade definitions, so we need to filter out the pure stat upgrades
        // so we can store them in a seperate list (as well as in the main list!)
        for (int i = 0; i < upgradeDefinitionGUIDs.Length; i++)
        {
            string upgradeDefinitionPath = AssetDatabase.GUIDToAssetPath(upgradeDefinitionGUIDs[i]);
            // Do not include the starting weapons
            if (upgradeDefinitionPath.StartsWith("Assets/Scripts/Upgrades/Prefabs/Instances/Base"))
            {
                Debug.Log($"Not adding base weapon to list: {upgradeDefinitionPath}");
                continue;
            }
            
            var upgradeDefinition = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(upgradeDefinitionPath);
            
            // Only store pure stat upgrades (present in this path) in the stat upgrades list, as it expects that
            if (upgradeDefinitionPath.StartsWith("Assets/Scripts/Upgrades/Prefabs/Instances/Stats"))
            {
                foundStatUpgrades.Add(upgradeDefinition);
            }
            
            // Pure stat upgrades should also be stored in the main upgrades list the player chooses from!
            foundUpgrades[i] = upgradeDefinition;
        }



        foundUpgrades = foundUpgrades
            .Where(u => u != null)
            .ToArray();
        
        foundStatUpgrades = foundStatUpgrades
            .Where(u => u != null)
            .ToList();



        // Change detection logic
        bool upgradeListChanged = false;
        if (upgradesList.upgrades == null || upgradesList.upgrades.Length != foundUpgrades.Length)
        {
            upgradeListChanged = true;
        }
        else
        {

            for (int i = 0; i < foundUpgrades.Length; i++)
            {
                if (upgradesList.upgrades[i] != foundUpgrades[i])
                {
                    upgradeListChanged = true; 
                    break;
                }
            }
        }

        if (upgradeListChanged)
        {
            upgradesList.upgrades = foundUpgrades;

            EditorUtility.SetDirty(upgradesList);
            AssetDatabase.SaveAssets();

            upgradesListUpdated = true;
        }
        else
        {
            // Debug.Log("UpgradesList is already up-to-date. No update needed.");
        }

        if (upgradesListUpdated)
        {
            Debug.Log("UpgradesList automatically updated with UpgradeDefinitions.");
        }
        
        
        bool statUpgradeListChanged = false;
        if (statUpgradesList.upgrades == null || statUpgradesList.upgrades.Length != foundStatUpgrades.Count)
        {
            statUpgradeListChanged = true;
        }
        else
        {
            for (int i = 0; i < foundStatUpgrades.Count; i++)
            {
                if (statUpgradesList.upgrades[i] != foundStatUpgrades[i])
                {
                    statUpgradeListChanged = true;
                    break;
                }
            }
        }

        if (statUpgradeListChanged)
        {
            statUpgradesList.upgrades = foundStatUpgrades.ToArray();

            EditorUtility.SetDirty(statUpgradesList);
            AssetDatabase.SaveAssets();

            statUpgradesListUpdated = true;
        }
        else
        {
            // Debug.Log("UpgradesList is already up-to-date. No update needed.");
        }

        if (statUpgradesListUpdated)
        {
            Debug.Log("UpgradesList automatically updated with UpgradeDefinitions.");
        }
        
    }
}