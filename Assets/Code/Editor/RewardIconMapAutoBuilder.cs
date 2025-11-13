using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class RewardIconMapAutoBuilder : EditorWindow {
    private RewardUIMap _targetMap;
    private string _dataFolder = "Assets/Resources/WheelItemData"; // default

    [MenuItem("Tools/Reward Icon Map Builder")]
    public static void Open() {
        GetWindow<RewardIconMapAutoBuilder>("Reward Icon Map Builder").Show();
    }

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

    private void BuildMap() {
        if (_targetMap == null) {
            Debug.LogError("No RewardUIMap selected.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:WheelItemData", new[] { _dataFolder });

        if (guids.Length == 0) {
            Debug.LogWarning("No WheelItemData found in folder: " + _dataFolder);
            return;
        }

        Undo.RecordObject(_targetMap, "Auto-Populate RewardUIMap");

        var newEntries = new List<RewardUIMap.Entry>();

        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var wheelItem = AssetDatabase.LoadAssetAtPath<WheelItemData>(path);

            if (wheelItem == null)
                continue;

            if (wheelItem.isBomb)
                continue; // <-- EXCLUDE BOMB

            if (string.IsNullOrEmpty(wheelItem.itemId)) {
                Debug.LogWarning($"WheelItemData at {path} has no itemId.");
                continue;
            }

            var entry = new RewardUIMap.Entry {
                itemId = wheelItem.itemId,
                icon = wheelItem.icon
            };

            newEntries.Add(entry);
        }

        _targetMap.entries = newEntries;

        EditorUtility.SetDirty(_targetMap);
        AssetDatabase.SaveAssets();

        Debug.Log($"RewardUIMap updated. {newEntries.Count} entries added.");
    }
}
