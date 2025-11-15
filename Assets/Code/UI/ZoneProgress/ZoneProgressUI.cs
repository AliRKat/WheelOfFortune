using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Code.Managers; // ZoneManager, ZoneType

namespace Code.UI {
    public class ZoneProgressUI : MonoBehaviour {

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private RectTransform _contentRoot_value;
        [SerializeField] private GameObject _zoneEntryPrefab_value;

        [Header("Settings")]
        [SerializeField] private int _visibleRange = 13;          // number of items always visible
        [SerializeField] private float _entrySpacing = 70f;       // horizontal distance between entries
        [SerializeField] private float _slideDuration = 0.25f;    // slide animation duration

        #endregion

        #region Internal State

        private readonly List<ZoneEntryUI> _entries = new();

        private ZoneManager _zoneManager;
        private Tweener _slideTween;
        private int _lastZone = -1;

        #endregion

        #region Public API

        /// <summary>
        /// Initializes the zone progress UI with the given ZoneManager and creates required entries.
        /// </summary>
        public void Init(ZoneManager manager) {
            _zoneManager = manager;
            CreateInitialEntries();
            Refresh();
        }

        /// <summary>
        /// Refreshes the UI to reflect the current zone and animates if zone changed.
        /// </summary>
        public void Refresh() {
            if (_zoneManager == null)
                return;

            int currentZone = _zoneManager.CurrentZone;

            // First-time Initialization
            if (_lastZone < 0) {
                _lastZone = currentZone;
                ForceRebuild(currentZone);
                return;
            }

            // No movement → just rebuild
            if (currentZone == _lastZone) {
                ForceRebuild(currentZone);
                return;
            }

            bool movingForward = currentZone > _lastZone;
            _lastZone = currentZone;

            PlaySlideAnimation(movingForward);
        }

        #endregion

        #region Entry Creation

        private void CreateInitialEntries() {
            // Cleanup
            for (int i = _contentRoot_value.childCount - 1; i >= 0; i--) {
                Destroy(_contentRoot_value.GetChild(i).gameObject);
            }

            _entries.Clear();

            // Pre-create entries
            for (int i = 0; i < _visibleRange; i++) {
                GameObject inst = Instantiate(_zoneEntryPrefab_value, _contentRoot_value);
                ZoneEntryUI ui = inst.GetComponent<ZoneEntryUI>();
                _entries.Add(ui);
            }
        }

        #endregion

        #region Animation Logic

        private void PlaySlideAnimation(bool forward) {
            _slideTween?.Kill();

            float direction = forward ? -1f : 1f;
            float moveAmount = direction * _entrySpacing;

            Vector2 startPos = _contentRoot_value.anchoredPosition;
            Vector2 endPos = startPos + new Vector2(moveAmount, 0f);

            _slideTween = _contentRoot_value
                .DOAnchorPos(endPos, _slideDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    // Snap back to original position
                    _contentRoot_value.anchoredPosition = startPos;

                    // Update entries for new zone window
                    ForceRebuild(_zoneManager.CurrentZone);
                });
        }

        #endregion

        #region Rebuild Logic

        private void ForceRebuild(int centerZone) {
            int half = _visibleRange / 2;
            int startZone = centerZone - half;

            for (int i = 0; i < _visibleRange; i++) {
                int zoneIndex = startZone + i;

                if (zoneIndex < 1) {
                    _entries[i].SetData(0, ZoneType.Normal); // Empty
                    continue;
                }

                ZoneConfig zone = _zoneManager.GetZoneByIndex(zoneIndex);

                if (zone == null) {
                    _entries[i].SetData(zoneIndex, ZoneType.Normal);
                } else {
                    _entries[i].SetData(zoneIndex, zone.type);
                }
            }
        }

        #endregion
    }
}
