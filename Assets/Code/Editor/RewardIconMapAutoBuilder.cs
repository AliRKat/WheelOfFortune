#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Code.Core;

public class RewardIconMapAutoBuilder : EditorWindow {

    private RewardUIMap _targetMap;
    private string _dataFolder = "Assets/Resources/WheelItemData";

    #region Menu

    /// <summary>
    /// Opens the Reward Icon Map Builder editor window.
    /// </summary>
    [MenuItem("Tools/Reward Icon Map Builder")]
    public static void Open() {
        GetWindow<RewardIconMapAutoBuilder>("Reward Icon Map Builder").Show();
    }

    #endregion

    #region GUI

    private void OnGUI() {
        GUILayout.Label("Auto-Populate RewardUIMap", EditorStyles.boldLabel);

        _targetMap = (RewardUIMap)EditorGUILayout.ObjectField(
            "Target Map", _targetMap, typeof(RewardUIMap), false);

        _dataFolder = EditorGUILayout.TextField("Data Folder", _dataFolder);

        GUILayout.Space(10);

        GUI.enabled = _targetMap != null;

        if (GUILayout.Button("Build Icon Map")) {
            BuildMap();
        }

        GUI.enabled = true;
    }

    #endregion

    #region Build Logic

    /// <summary>
    /// Rebuilds the RewardUIMap by scanning WheelItemData assets in the configured folder,
    /// generating entries for all non-bomb items with a valid itemId.
    /// </summary>
    private void BuildMap() {
        if (_targetMap == null) {
            GameLogger.Error(this, "BuildMap", "Validation",
                "No RewardUIMap asset assigned.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:WheelItemData", new[] { _dataFolder });

        if (guids.Length == 0) {
            GameLogger.Warn(this, "BuildMap", "AssetScan",
                $"No WheelItemData assets found in folder: {_dataFolder}");
            return;
        }

        Undo.RecordObject(_targetMap, "Auto-Populate RewardUIMap");

        var newEntries = new List<RewardUIMap.Entry>();

        foreach (string guid in guids) {
            ProcessWheelItem(guid, newEntries);
        }

        ApplyEntries(newEntries);
    }

    #endregion

    #region Processing

    private void ProcessWheelItem(string guid, List<RewardUIMap.Entry> list) {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        var wheelItem = AssetDatabase.LoadAssetAtPath<WheelItemData>(path);

        if (wheelItem == null)
            return;

        if (wheelItem.isBomb)
            return; // Exclude bomb items

        if (string.IsNullOrEmpty(wheelItem.itemId)) {
            GameLogger.Warn(this, "ProcessWheelItem", "MissingId",
                $"WheelItemData at {path} has no itemId.");
            return;
        }

        list.Add(new RewardUIMap.Entry {
            itemId = wheelItem.itemId,
            icon = wheelItem.icon
        });
    }

    private void ApplyEntries(List<RewardUIMap.Entry> entries) {
        _targetMap.entries = entries;

        EditorUtility.SetDirty(_targetMap);
        AssetDatabase.SaveAssets();

        GameLogger.Log(this, "ApplyEntries", "Result",
            $"RewardUIMap updated. {entries.Count} entries added.");
    }

    #endregion
}
#endif
