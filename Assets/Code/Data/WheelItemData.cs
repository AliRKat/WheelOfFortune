using UnityEngine;

[CreateAssetMenu(menuName = "Game/Wheel Item", fileName = "WheelItem_New")]
public class WheelItemData : ScriptableObject {
    [Header("Basic Info")]
    public string itemId;
    public Sprite icon;

    [Header("Parameters")]
    public bool isBomb;
    public bool showAmount = true;
    public int baseAmount;
    public WheelItemCategory category;
}
