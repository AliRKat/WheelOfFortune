using UnityEngine;
using System.Collections.Generic;

namespace Code.UI {
    public class RewardsUI : MonoBehaviour {

        #region Serialized Fields

        [SerializeField] private Transform _contentRoot_value;
        [SerializeField] private RewardsUIItem _itemPrefab_value;

        #endregion

        #region Internal State

        // Key: rewardId → UI item
        private readonly Dictionary<string, RewardsUIItem> _activeMap = new();

        private readonly List<RewardsUIItem> _orderedList = new();

        #endregion

        #region Public API

        /// <summary>
        /// Updates reward UI by syncing spawned items with the given list.
        /// Keeps existing items, creates missing ones, and disables removed ones.
        /// </summary>
        public void Populate(List<RewardItemDTO> items) {

            if (items == null || items.Count == 0) {
                ClearAll();
                return;
            }

            SyncActiveItems(items);
            RebuildOrder(items);
        }

        /// <summary>
        /// Returns the UI item with the given reward id, or null if not found.
        /// </summary>
        public RewardsUIItem GetSpawnedWithId(string id) {
            if (_activeMap.TryGetValue(id, out var item))
                return item;

            return null;
        }

        #endregion

        #region Internal Logic

        private void SyncActiveItems(List<RewardItemDTO> items) {
            // First mark all existing as unused
            HashSet<string> usedThisUpdate = new();

            for (int i = 0; i < items.Count; i++) {
                var dto = items[i];

                if (_activeMap.TryGetValue(dto.Id, out RewardsUIItem existing)) {
                    // Update existing
                    existing.Set(dto.Icon, dto.Amount, dto.Id);
                } else {
                    // Create new entry
                    RewardsUIItem inst = Instantiate(_itemPrefab_value, _contentRoot_value);
                    inst.Set(dto.Icon, dto.Amount, dto.Id);

                    _activeMap[dto.Id] = inst;
                }

                usedThisUpdate.Add(dto.Id);
            }

            // Disable removed items
            foreach (var kvp in _activeMap) {
                if (!usedThisUpdate.Contains(kvp.Key)) {
                    kvp.Value.gameObject.SetActive(false);
                }
            }
        }

        private void RebuildOrder(List<RewardItemDTO> items) {
            // Remove all from hierarchy temporarily
            _orderedList.Clear();

            for (int i = 0; i < items.Count; i++) {
                var dto = items[i];

                var item = _activeMap[dto.Id];
                _orderedList.Add(item);

                item.transform.SetSiblingIndex(i);
                item.gameObject.SetActive(true);
            }
        }

        private void ClearAll() {
            foreach (var kvp in _activeMap) {
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
            }

            _activeMap.Clear();
            _orderedList.Clear();
        }

        #endregion
    }
}
