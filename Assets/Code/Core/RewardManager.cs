using System.Collections.Generic;

namespace Code.Managers {
    public class RewardManager {
        private readonly Dictionary<string, int> _totals = new();

        #region Public API

        /// <summary>
        /// Adds an amount of a specific reward item to the total.
        /// </summary>
        public void Add(WheelItemData item, int amount) {
            if (item == null)
                return;

            string id = item.itemId;

            if (_totals.ContainsKey(id))
                _totals[id] += amount;
            else
                _totals[id] = amount;
        }

        /// <summary>
        /// Attempts to spend an amount of a specific reward item.
        /// Returns true if successful, false if not enough balance exists.
        /// </summary>
        public bool Spend(string itemId, int amount) {
            if (string.IsNullOrEmpty(itemId))
                return false;

            if (!_totals.ContainsKey(itemId))
                return false;

            int current = _totals[itemId];
            if (current < amount)
                return false;

            _totals[itemId] = current - amount;
            return true;
        }

        /// <summary>
        /// Clears all stored rewards.
        /// </summary>
        public void Reset() {
            _totals.Clear();
        }

        /// <summary>
        /// Returns the internal totals dictionary (reference, not a copy).
        /// </summary>
        public Dictionary<string, int> GetTotals() {
            return _totals;
        }

        /// <summary>
        /// Returns a compact human-readable string of all rewards.
        /// </summary>
        public string GetEarningsString() {
            if (_totals.Count == 0)
                return "—";

            var parts = new List<string>();

            foreach (var kv in _totals)
                parts.Add($"{kv.Value} {kv.Key}");

            return string.Join(", ", parts);
        }

        #endregion
    }
}
