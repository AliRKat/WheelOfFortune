using System.Collections.Generic;
using UnityEngine;

namespace Code.Managers {
    public class RewardManager {
        private readonly Dictionary<string, int> _totals = new();

        public void Add(WheelItemData item, int amount) {
            if (item == null) return;

            string id = item.itemId;

            if (_totals.ContainsKey(id))
                _totals[id] += amount;
            else
                _totals[id] = amount;
        }

        public void Reset() {
            _totals.Clear();
        }

        public Dictionary<string, int> GetTotals() {
            return _totals;
        }

        public string GetEarningsString() {
            if (_totals.Count == 0)
                return "—";

            var parts = new List<string>();

            foreach (var kv in _totals)
                parts.Add($"{kv.Value} {kv.Key}");

            return string.Join(", ", parts);
        }

        public void Collect() {
            string earnings = GetEarningsString();
            Debug.Log($"Player left.\nEarnings: {earnings}");
            Reset();
        }
    }

    public class RewardItemDTO {
        public Sprite Icon;
        public int Amount;
    }

}
