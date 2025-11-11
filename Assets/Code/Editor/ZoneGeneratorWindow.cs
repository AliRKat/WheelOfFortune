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
    private string outputPath = "Assets/Resources/ZoneConfig";

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

        var rewards = Resources.LoadAll<RewardData>("RewardData");
        if (rewards.Length == 0) {
            Debug.LogError("No RewardData found under Resources/RewardData.");
            return;
        }

        try {
            for (int i = 1; i <= zoneCount; i++) {
                float progress = (float)i / zoneCount;
                EditorUtility.DisplayProgressBar("Generating Zones",
                    $"Creating Zone {i}/{zoneCount}", progress);

                ZoneType type = ZoneType.Normal;
                if (i % superZoneInterval == 0) type = ZoneType.Super;
                else if (i % safeZoneInterval == 0) type = ZoneType.Safe;

                var zone = ScriptableObject.CreateInstance<ZoneConfig>();
                zone.zoneId = $"Zone_{i:D3}";
                zone.type = type;
                zone.slices = new List<ZoneSlice>();

                int bombIndex = -1;
                if (type == ZoneType.Normal)
                    bombIndex = Random.Range(0, slicesPerZone);

                for (int s = 0; s < slicesPerZone; s++) {
                    var slice = new ZoneSlice();
                    slice.isBomb = s == bombIndex;

                    if (!slice.isBomb) {
                        var reward = rewards[Random.Range(0, rewards.Length)];
                        slice.reward = reward;

                        float multiplier = 1f;

                        // Zone-based scaling
                        switch (type) {
                            case ZoneType.Safe:
                                multiplier = 1.5f;
                                break;
                            case ZoneType.Super:
                                multiplier = 2.5f;
                                break;
                        }

                        // Progressive scaling: zone index affects output
                        // (so Zone_50 gives higher amounts than Zone_5)
                        multiplier *= 1f + (i / 50f); // increase gradually with depth

                        // Randomize slightly for variety (±20%)
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
