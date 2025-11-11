namespace Code.Managers {
    using UnityEngine;
    using System.Linq;

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
    }

}
