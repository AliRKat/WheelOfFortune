#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ZoneGeneratorWindow : EditorWindow {
    private int zoneCount = 50;
    private int slicesPerZone = 8;
    private int safeZoneInterval = 5;
    private int superZoneInterval = 30;
    private static string outputPath = "Assets/Resources/ZoneConfig";
    private static string inputPath = "WheelItemData";

    [MenuItem("Tools/Zone Generator")]
    public static void ShowWindow() {
        GetWindow<ZoneGeneratorWindow>("Zone Generator");
    }

    private void OnGUI() {
        GUILayout.Label("Zone Generation Settings", EditorStyles.boldLabel);

        zoneCount = EditorGUILayout.IntField("Zone Count", zoneCount);
        slicesPerZone = EditorGUILayout.IntField("Slices per Zone", slicesPerZone);
        safeZoneInterval = EditorGUILayout.IntField("Safe Zone Interval", safeZoneInterval);
        superZoneInterval = EditorGUILayout.IntField("Super Zone Interval", superZoneInterval);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        GUILayout.Space(8);
        if (GUILayout.Button("Generate Zones")) {
            GenerateZones();
        }
    }

    private void GenerateZones() {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var items = Resources.LoadAll<WheelItemData>(inputPath);
        if (items.Length == 0) {
            Debug.LogError("No WheelItemData found under Resources/" + inputPath);
            return;
        }

        try {
            for (int i = 1; i <= zoneCount; i++) {
                float progress = (float)i / zoneCount;
                EditorUtility.DisplayProgressBar("Generating Zones",
                    $"Creating Zone {i}/{zoneCount}", progress);

                // --- Zone setup ---
                ZoneType type = ZoneType.Normal;
                if (i % superZoneInterval == 0) type = ZoneType.Super;
                else if (i % safeZoneInterval == 0) type = ZoneType.Safe;

                var zone = ScriptableObject.CreateInstance<ZoneConfig>();
                zone.zoneId = $"Zone_{i:D3}";
                zone.type = type;
                zone.slices = new List<ZoneSlice>();

                bool allowBombs = (type == ZoneType.Normal);

                var bombItems = new List<WheelItemData>();
                var rewardItems = new List<WheelItemData>();
                foreach (var item in items) {
                    if (item.isBomb)
                        bombItems.Add(item);
                    else
                        rewardItems.Add(item);
                }

                if (rewardItems.Count == 0) {
                    Debug.LogError("No non-bomb WheelItemData found.");
                    return;
                }

                int bombIndex = allowBombs ? Random.Range(0, slicesPerZone) : -1;

                // --- Slice generation ---
                for (int s = 0; s < slicesPerZone; s++) {
                    var slice = new ZoneSlice();

                    if (allowBombs && s == bombIndex && bombItems.Count > 0) {
                        var bombItem = bombItems[Random.Range(0, bombItems.Count)];
                        slice.itemData = bombItem;
                        slice.isBomb = true;
                        slice.customAmount = 0;
                    } else {
                        var reward = rewardItems[Random.Range(0, rewardItems.Count)];
                        slice.itemData = reward;
                        slice.isBomb = reward.isBomb;

                        float multiplier = 1f;
                        switch (type) {
                            case ZoneType.Safe: multiplier = 1.5f; break;
                            case ZoneType.Super: multiplier = 2.5f; break;
                        }

                        multiplier *= 1f + (i / 50f);
                        float variance = Random.Range(0.8f, 1.2f);
                        int finalAmount = Mathf.RoundToInt(reward.baseAmount * multiplier * variance);

                        slice.customAmount = Mathf.Max(1, finalAmount);
                    }

                    zone.slices.Add(slice);
                }

                string assetPath = $"{outputPath}/{zone.zoneId}.asset";
                AssetDatabase.CreateAsset(zone, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated {zoneCount} zones under {outputPath}");
        } finally {
            EditorUtility.ClearProgressBar();
        }
    }


}
#endif
