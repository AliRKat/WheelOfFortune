using System.Collections.Generic;
using UnityEngine;

namespace Code.Managers {
    public class RewardManager {
        private readonly Dictionary<WheelItemCategory, int> _categoryTotals = new();

        public void Add(WheelItemData reward, int amount) {
            if (reward == null) return;

            var cat = reward.category;
            if (_categoryTotals.ContainsKey(cat))
                _categoryTotals[cat] += amount;
            else
                _categoryTotals[cat] = amount;
        }

        public void Reset() {
            _categoryTotals.Clear();
        }

        // For "Earnings: 25 Chest, 25 Points"
        public string GetEarningsString() {
            if (_categoryTotals.Count == 0)
                return "—";

            var parts = new List<string>();
            foreach (var kvp in _categoryTotals) {
                // kvp.Key is enum, printed as name
                parts.Add($"{kvp.Value} {kvp.Key}");
            }
            return string.Join(", ", parts);
        }

        // Optional: for L
        public void Collect() {
            string earnings = GetEarningsString();
            Debug.Log($"Player left.\nEarnings: {earnings}");
            Reset();
        }
    }
}
