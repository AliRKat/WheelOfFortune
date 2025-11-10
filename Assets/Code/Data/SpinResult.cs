public struct SpinResult {
    public bool IsBomb;
    public int RewardAmount;
    public string RewardId;
    public int SliceIndex;

    public SpinResult(bool isBomb, int rewardAmount, string rewardId, int sliceIndex) {
        IsBomb = isBomb;
        RewardAmount = rewardAmount;
        RewardId = rewardId;
        SliceIndex = sliceIndex;
    }

    public static SpinResult Empty => new SpinResult(true, 0, string.Empty, -1);
}
