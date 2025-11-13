using System.Collections.Generic;
using UnityEngine;
using Code.Managers; // For ZoneManager, ZoneType

namespace Code.UI {
    public class ZoneProgressUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _zoneEntryPrefab;

        [Header("Settings")]
        [SerializeField] private int _visibleRange = 13; // 6 left + current + 6 right

        private readonly List<ZoneEntryUI> _entries = new();
        private ZoneManager _zoneManager;

        public void Init(ZoneManager manager) {
            _zoneManager = manager;
            CreateInitialEntries();
            Refresh();
        }

        private void CreateInitialEntries() {
            // Clear old
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            _entries.Clear();

            // Pre-create all slots (empty)
            for (int i = 0; i < _visibleRange; i++) {
                var entry = Instantiate(_zoneEntryPrefab, _content)
                    .GetComponent<ZoneEntryUI>();
                _entries.Add(entry);
            }
        }

        public void Refresh() {
            if (_zoneManager == null) return;
            if (_entries.Count == 0) CreateInitialEntries();

            int currentZone = _zoneManager.CurrentZone;
            int half = _visibleRange / 2;

            // Always show a full window
            int startZone = currentZone - half;
            int endZone = currentZone + half;

            for (int i = 0; i < _visibleRange; i++) {
                int zoneIndex = startZone + i;

                // Fill blanks if before zone 1
                if (zoneIndex < 1) {
                    _entries[i].SetData(0, ZoneType.Normal); // 0 = empty
                    continue;
                }

                var zone = _zoneManager.GetZoneByIndex(zoneIndex);
                _entries[i].SetData(
                    zoneIndex,
                    zone != null ? zone.type : ZoneType.Normal
                );
            }

            // (later) Slide animation goes here
        }

        // optional: later tonight, implement slide animation here
        // private IEnumerator SlideLeftAnimation() { ... }
    }
}
