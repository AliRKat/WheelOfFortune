using UnityEngine;
using System.Collections.Generic;

namespace Code.UI {
    public class RewardsUI : MonoBehaviour {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private RewardsUIItem _itemPrefab;
        private readonly List<RewardsUIItem> _spawned = new();

        public void Populate(List<RewardItemDTO> items) {
            Clear();

            foreach (var dto in items) {
                var inst = Instantiate(_itemPrefab, _contentRoot);
                inst.Set(dto.Icon, dto.Amount, dto.Id);
                _spawned.Add(inst);
            }
        }

        public RewardsUIItem GetSpawnedWithId(string id) {
            foreach (var item in _spawned) {
                if(item.GetId() == id) {
                    return item;
                }
            }

            return null;
        }

        private void Clear() {
            foreach (var s in _spawned)
                Destroy(s.gameObject);
            _spawned.Clear();
        }
    }

}
