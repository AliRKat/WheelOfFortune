public struct SpinResult {
    public bool IsBomb;
    public int RewardAmount;
    public string RewardId;
    public RewardData RewardData;

    public static SpinResult Empty => new SpinResult(true, 0, "none", null);

    public SpinResult(bool isBomb, int amount, string id, RewardData data) {
        IsBomb = isBomb;
        RewardAmount = amount;
        RewardId = id;
        RewardData = data;
    }
}
