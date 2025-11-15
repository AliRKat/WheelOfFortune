#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Code.Core;

public class ZoneGeneratorWindow : EditorWindow {

    #region Fields

    private int _zoneCount = 50;
    private int _slicesPerZone = 8;
    private int _safeZoneInterval = 5;
    private int _superZoneInterval = 30;

    private static string _outputPath = "Assets/Resources/ZoneConfig";
    private static string _inputPath = "WheelItemData";

    #endregion

    #region Menu

    /// <summary>
    /// Opens the Zone Generator Editor Window.
    /// </summary>
    [MenuItem("Tools/Zone Generator")]
    public static void ShowWindow() {
        GetWindow<ZoneGeneratorWindow>("Zone Generator");
    }

    #endregion

    #region GUI

    private void OnGUI() {
        GUILayout.Label("Zone Generation Settings", EditorStyles.boldLabel);

        _zoneCount = EditorGUILayout.IntField("Zone Count", _zoneCount);
        _slicesPerZone = EditorGUILayout.IntField("Slices per Zone", _slicesPerZone);
        _safeZoneInterval = EditorGUILayout.IntField("Safe Zone Interval", _safeZoneInterval);
        _superZoneInterval = EditorGUILayout.IntField("Super Zone Interval", _superZoneInterval);

        _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);

        GUILayout.Space(8);
        if (GUILayout.Button("Generate Zones")) {
            GenerateZones();
        }

        GUILayout.Space(6);
        if (GUILayout.Button("Clear All Zones")) {
            ClearAllZonesInOutputPath();
        }
    }

    #endregion

    #region Generation Entry

    /// <summary>
    /// Generates all zone ScriptableObjects based on current settings.
    /// </summary>
    private void GenerateZones() {
        EnsureOutputDirectory();

        var items = Resources.LoadAll<WheelItemData>(_inputPath);
        if (items.Length == 0) {
            GameLogger.Error(this, "GenerateZones", "WheelItemLoad",
                $"No WheelItemData found under Resources/{_inputPath}");
            return;
        }

        try {
            GenerateAllZones(items);
            FinalizeGeneration();
        } finally {
            EditorUtility.ClearProgressBar();
        }
    }

    #endregion

    #region Directory

    private void EnsureOutputDirectory() {
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);
    }

    #endregion

    #region Main Generation

    private void GenerateAllZones(WheelItemData[] items) {
        for (int i = 1; i <= _zoneCount; i++) {
            float progress = (float)i / _zoneCount;

            EditorUtility.DisplayProgressBar(
                "Generating Zones",
                $"Creating Zone {i}/{_zoneCount}",
                progress);

            GenerateSingleZone(i, items);
        }
    }

    private void GenerateSingleZone(int zoneIndex, WheelItemData[] items) {
        ZoneType type = DetermineZoneType(zoneIndex);

        ZoneConfig zone = CreateZoneAsset(zoneIndex, type);

        SeparateItems(items, out var bombItems, out var rewardItems);
        if (rewardItems.Count == 0) {
            GameLogger.Error(this, "GenerateSingleZone", "ItemValidation",
                "No non-bomb WheelItemData found.");
            return;
        }

        int bombIndex = ComputeBombIndex(type, _slicesPerZone);

        GenerateSlices(zone, zoneIndex, type, rewardItems, bombItems, bombIndex);

        SaveZoneAsset(zone);
    }

    #endregion

    #region Zone Creation

    private ZoneType DetermineZoneType(int zoneIndex) {
        if (zoneIndex % _superZoneInterval == 0)
            return ZoneType.Super;

        if (zoneIndex % _safeZoneInterval == 0)
            return ZoneType.Safe;

        return ZoneType.Normal;
    }

    private ZoneConfig CreateZoneAsset(int index, ZoneType type) {
        var zone = ScriptableObject.CreateInstance<ZoneConfig>();
        zone.zoneId = $"Zone_{index:D3}";
        zone.type = type;
        zone.slices = new List<ZoneSlice>();
        return zone;
    }

    private void SaveZoneAsset(ZoneConfig zone) {
        string assetPath = $"{_outputPath}/{zone.zoneId}.asset";
        AssetDatabase.CreateAsset(zone, assetPath);
    }

    #endregion

    #region Item Separation

    private void SeparateItems(WheelItemData[] items,
        out List<WheelItemData> bombItems,
        out List<WheelItemData> rewardItems) {
        bombItems = new List<WheelItemData>();
        rewardItems = new List<WheelItemData>();

        for (int i = 0; i < items.Length; i++) {
            if (items[i].isBomb)
                bombItems.Add(items[i]);
            else
                rewardItems.Add(items[i]);
        }
    }

    #endregion

    #region Bomb Index

    private int ComputeBombIndex(ZoneType type, int sliceCount) {
        bool allowBomb = type == ZoneType.Normal;
        if (!allowBomb)
            return -1;

        return Random.Range(0, sliceCount);
    }

    #endregion

    #region Slice Generation

    private void GenerateSlices(
        ZoneConfig zone,
        int zoneIndex,
        ZoneType type,
        List<WheelItemData> rewardItems,
        List<WheelItemData> bombItems,
        int bombIndex) {
        for (int s = 0; s < _slicesPerZone; s++) {
            ZoneSlice slice = new ZoneSlice();

            if (s == bombIndex && bombItems.Count > 0) {
                AssignBombSlice(slice, bombItems);
            } else {
                AssignRewardSlice(slice, rewardItems, zoneIndex, type);
            }

            zone.slices.Add(slice);
        }
    }

    private void AssignBombSlice(ZoneSlice slice, List<WheelItemData> bombItems) {
        var bombItem = bombItems[Random.Range(0, bombItems.Count)];
        slice.itemData = bombItem;
        slice.isBomb = true;
        slice.customAmount = 0;
    }

    private void AssignRewardSlice(
        ZoneSlice slice,
        List<WheelItemData> rewardItems,
        int zoneIndex,
        ZoneType type) {
        var reward = rewardItems[Random.Range(0, rewardItems.Count)];
        slice.itemData = reward;
        slice.isBomb = reward.isBomb;

        float multiplier = GetZoneMultiplier(type);

        // Original soft scaling — preserved 1:1
        float softScale = 1f + (zoneIndex / 50f);

        // NEW: demo-friendly linear progression
        float progressiveBoost = 1f + (zoneIndex * 0.02f);

        // Combine
        multiplier *= softScale * progressiveBoost;

        float variance = Random.Range(0.8f, 1.2f);
        int finalAmount = Mathf.RoundToInt(reward.baseAmount * multiplier * variance);

        slice.customAmount = Mathf.Max(1, finalAmount);
    }

    private float GetZoneMultiplier(ZoneType type) {
        switch (type) {
            case ZoneType.Safe: return 1.5f;
            case ZoneType.Super: return 2.5f;
            default: return 1f;
        }
    }

    #endregion

    #region Clearing

    /// <summary>
    /// Deletes all zone asset files inside the configured output directory.
    /// </summary>
    private void ClearAllZonesInOutputPath() {
        if (!Directory.Exists(_outputPath)) {
            GameLogger.Warn(this, "ClearAllZonesInOutputPath", "Directory",
                $"Output path does not exist: {_outputPath}");
            return;
        }

        bool confirm = EditorUtility.DisplayDialog(
            "Clear All Zones",
            $"Are you sure you want to delete ALL zone assets in:\n\n{_outputPath}\n\nThis cannot be undone.",
            "Yes, Delete All",
            "Cancel");

        if (!confirm)
            return;

        string[] files = Directory.GetFiles(_outputPath, "*.asset", SearchOption.TopDirectoryOnly);
        int deletedCount = 0;

        for (int i = 0; i < files.Length; i++) {
            File.Delete(files[i]);
            deletedCount++;
        }

        AssetDatabase.Refresh();

        GameLogger.Log(this, "ClearAllZonesInOutputPath", "Complete",
            $"Deleted {deletedCount} zone assets from {_outputPath}");
    }

    #endregion

    #region Finalization

    private void FinalizeGeneration() {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameLogger.Log(this, "GenerateZones", "Complete",
            $"Generated {_zoneCount} zones under {_outputPath}");
    }

    #endregion
}
#endif
