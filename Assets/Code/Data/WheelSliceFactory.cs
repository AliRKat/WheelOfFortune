using System.Collections.Generic;

public static class WheelSliceFactory {
    public static List<WheelSlice> Build(ZoneType zoneType, int baseReward) {
        var list = new List<WheelSlice>();

        list.Add(new WheelSlice { Kind = SliceKind.Reward, Amount = baseReward });
        list.Add(new WheelSlice { Kind = SliceKind.Reward, Amount = baseReward + 5 });
        list.Add(new WheelSlice { Kind = SliceKind.Reward, Amount = baseReward + 10 });
        list.Add(new WheelSlice { Kind = SliceKind.Reward, Amount = baseReward + 15 });

        if (zoneType == ZoneType.Normal) {
            list.Add(new WheelSlice { Kind = SliceKind.Bomb, Amount = 0 });
        }

        return list;
    }
}
