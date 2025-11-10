namespace Code.Managers {
    public class ZoneManager {
        public int CurrentZone { get; private set; } = 1;

        public ZoneType GetCurrentZoneType() {
            if (CurrentZone % 30 == 0) return ZoneType.Super;
            if (CurrentZone % 5 == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }

        public void AdvanceZone() {
            CurrentZone++;
        }
    }
}
