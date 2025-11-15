using UnityEngine;
using System;
using Code.Core;

namespace Code.Managers {
    public class ZoneManager {
        public int CurrentZone { get; private set; } = 1;
        private ZoneConfig[] _zones;

        #region Constructor

        public ZoneManager() {
            LoadZones();
            ValidateZones();
        }

        #endregion

        #region Zone Loading

        private void LoadZones() {
            // Load all ZoneConfig assets
            _zones = Resources.LoadAll<ZoneConfig>("ZoneConfig");
            SortZonesById(_zones);
        }

        private void ValidateZones() {
            if (_zones == null || _zones.Length == 0) {
                GameLogger.Error(this, "Constructor", "ZoneLoad",
                    "No ZoneConfig assets found under Resources/ZoneConfig.");
            }
        }

        /// <summary>
        /// Sorts the given array of ZoneConfig objects in ascending order based on their
        /// zoneId values. This ensures zones are processed in the correct sequence
        /// determined by their identifiers, regardless of the order they were loaded from Resources.
        /// </summary>
        private void SortZonesById(ZoneConfig[] array) {
            if (array == null || array.Length <= 1)
                return;

            for (int i = 1; i < array.Length; i++) {
                ZoneConfig key = array[i];
                int j = i - 1;

                while (j >= 0 &&
                       string.Compare(array[j].zoneId, key.zoneId, StringComparison.CurrentCulture) > 0) {
                    array[j + 1] = array[j];
                    j--;
                }

                array[j + 1] = key;
            }
        }

        #endregion

        #region Zone Access

        /// <summary>
        /// Returns the currently active zone config.
        /// </summary>
        public ZoneConfig GetCurrentZone() {
            if (!HasZones())
                return null;

            int index = Mathf.Clamp(CurrentZone - 1, 0, _zones.Length - 1);
            return _zones[index];
        }

        /// <summary>
        /// Returns the zone config at a specific index (1-based).
        /// </summary>
        public ZoneConfig GetZoneByIndex(int index) {
            if (!HasZones())
                return null;

            int clamped = Mathf.Clamp(index - 1, 0, _zones.Length - 1);
            return _zones[clamped];
        }

        private bool HasZones() {
            return _zones != null && _zones.Length > 0;
        }

        #endregion

        #region Zone Progression

        /// <summary>
        /// Moves to the next zone. Clamps at the maximum available zone.
        /// </summary>
        public void AdvanceZone() {
            CurrentZone++;

            if (CurrentZone > _zones.Length)
                CurrentZone = _zones.Length;
        }

        /// <summary>
        /// Resets the current zone back to the first.
        /// </summary>
        public void ResetToStart() {
            CurrentZone = 1;
        }

        #endregion
    }
}
