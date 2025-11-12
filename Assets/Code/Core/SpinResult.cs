public struct SpinResult {
    public bool IsBomb;
    public int RewardAmount;
    public string RewardId;
    public WheelItemData RewardData;
    public static SpinResult Empty => new SpinResult(true, 0, "none", null);

    public SpinResult(bool isBomb, int amount, string id, WheelItemData data) {
        IsBomb = isBomb;
        RewardAmount = amount;
        RewardId = id;
        RewardData = data;
    }
}
