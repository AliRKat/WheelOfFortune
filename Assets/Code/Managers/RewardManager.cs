namespace Code.Managers {
    public class RewardManager {
        public int Total { get; private set; }

        public void Add(int amount) {
            Total += amount;
        }

        public void Reset() {
            Total = 0;
        }

        public int Collect() {
            int collected = Total;
            Total = 0;
            return collected;
        }
    }
}