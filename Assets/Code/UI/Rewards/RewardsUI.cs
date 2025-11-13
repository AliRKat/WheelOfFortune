using UnityEngine;
using System.Collections.Generic;
using Code.Managers;

namespace Code.UI {
    public class RewardsUI : MonoBehaviour {
        [SerializeField] private GameObject _root;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private RewardsUIItem _itemPrefab;

        private readonly List<RewardsUIItem> _spawned = new();

        public void Show() => _root.SetActive(true);
        public void Hide() => _root.SetActive(false);

        public void Populate(List<RewardItemDTO> items) {
            Clear();

            foreach (var dto in items) {
                var inst = Instantiate(_itemPrefab, _contentRoot);
                inst.Set(dto.Icon, dto.Amount.ToString());
                _spawned.Add(inst);
            }
        }

        private void Clear() {
            foreach (var s in _spawned)
                Destroy(s.gameObject);
            _spawned.Clear();
        }
    }

}
