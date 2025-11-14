using UnityEngine;
using System.Collections.Generic;
using Code.Managers;

namespace Code.UI {
    public class RewardsUI : MonoBehaviour {
        [SerializeField] private GameObject _root;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private RewardsUIItem _itemPrefab;

        private readonly List<RewardsUIItem> _spawned = new();

        public void Populate(List<RewardItemDTO> items) {
            Clear();

            foreach (var dto in items) {
                var inst = Instantiate(_itemPrefab, _contentRoot);
                inst.Set(dto.Icon, dto.Amount.ToString(), dto.Id);
                _spawned.Add(inst);
            }
        }

        public Transform GetSpawnedWithId(string id) {
            foreach (var item in _spawned) {
                if(item.GetId() == id) {
                    return item.transform;
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
