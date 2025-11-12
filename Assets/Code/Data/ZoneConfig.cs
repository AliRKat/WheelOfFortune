using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneSlice {
    public bool isBomb;
    public WheelItemData itemData;
    public int customAmount;
}

[CreateAssetMenu(menuName = "Game/Zone Config", fileName = "Zone_New")]
public class ZoneConfig : ScriptableObject {
    public string zoneId;
    public ZoneType type;
    public List<ZoneSlice> slices = new();
}
