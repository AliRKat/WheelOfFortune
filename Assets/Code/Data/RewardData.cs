using UnityEngine;

[CreateAssetMenu(menuName = "Game/Reward Data", fileName = "Reward_New")]
public class RewardData : ScriptableObject {
    [Header("ID")]
    public string rewardId;               // e.g. "cash_small", "chest_gold", "points_sniper"

    [Header("Meta")]
    public RewardCategory category;

    [Header("Presentation")]
    public Sprite icon;

    [Header("Economy")]
    public int baseAmount = 0;            // zone or slice can add on top
}
