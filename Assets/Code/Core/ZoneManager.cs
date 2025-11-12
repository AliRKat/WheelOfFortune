using UnityEngine;
using System.Linq;

namespace Code.Managers {
    public class ZoneManager {
        public int CurrentZone { get; private set; } = 1;
        private ZoneConfig[] _zones;

        public ZoneManager() {
            _zones = Resources.LoadAll<ZoneConfig>("ZoneConfig")
                              .OrderBy(z => z.zoneId)
                              .ToArray();

            if (_zones.Length == 0)
                Debug.LogError("No ZoneConfig assets found under Resources/ZoneConfig.");
        }

        public ZoneConfig GetCurrentZone() {
            if (_zones == null || _zones.Length == 0)
                return null;

            int index = Mathf.Clamp(CurrentZone - 1, 0, _zones.Length - 1);
            return _zones[index];
        }

        public void AdvanceZone() {
            CurrentZone++;
            if (CurrentZone > _zones.Length)
                CurrentZone = _zones.Length; // clamp at max
        }

        public void ResetToStart() {
            CurrentZone = 1;
        }

        public void RemoveBombFromCurrentZone() {
            var zone = GetCurrentZone();
            if (zone == null || zone.slices == null)
                return;

            bool modified = false;
            foreach (var slice in zone.slices) {
                if (slice.isBomb) {
                    slice.isBomb = false;
                    modified = true;
                }
            }

            if (modified)
                Debug.Log($"Bomb removed from {zone.zoneId}");
            else
                Debug.LogWarning($"No bomb found in {zone.zoneId}");
        }

        public ZoneConfig GetZoneByIndex(int index) {
            if (_zones == null || _zones.Length == 0)
                return null;

            int clamped = Mathf.Clamp(index - 1, 0, _zones.Length - 1);
            return _zones[clamped];
        }

    }

}
