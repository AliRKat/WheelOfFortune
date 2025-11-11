using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneSlice {
    public bool isBomb;
    public RewardData reward;
    public int customAmount; // optional override; 0 means use reward.baseAmount
}

[CreateAssetMenu(menuName = "Game/Zone Config", fileName = "Zone_New")]
public class ZoneConfig : ScriptableObject {
    public string zoneId;
    public ZoneType type;
    public List<ZoneSlice> slices = new();
}
