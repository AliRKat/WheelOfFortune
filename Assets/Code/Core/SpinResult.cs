public struct SpinResult {
    public bool IsBomb;
    public int RewardAmount;
    public WheelItemData RewardData;
    public int WinningIndex;

    public SpinResult(bool isBomb, int amount, WheelItemData data, int index) {
        IsBomb = isBomb;
        RewardAmount = amount;
        RewardData = data;
        WinningIndex = index;
    }
}
