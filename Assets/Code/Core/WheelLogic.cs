using UnityEngine;

public class WheelLogic {
    public SpinResult Spin(ZoneConfig zone) {
        // Guard – should not happen in practice, but avoids null ref explosions
        if (zone == null || zone.slices == null || zone.slices.Count == 0) {
            Debug.LogError("WheelLogic.Spin: Zone or slices invalid.");
            return new SpinResult(true, 0, null, -1);
        }

        int index = Random.Range(0, zone.slices.Count);
        var slice = zone.slices[index];

        var data = slice.itemData;

        // Safety: no data on slice
        if (data == null) {
            Debug.LogWarning($"WheelLogic.Spin: slice {index} has no itemData.");
            return new SpinResult(true, 0, null, index);
        }

        bool isBomb = data.category == WheelItemCategory.Bomb;

        if (isBomb)
            return new SpinResult(true, 0, data, index);

        int baseAmount = data.baseAmount;
        int amount = slice.customAmount > 0 ? slice.customAmount : baseAmount;

        return new SpinResult(false, amount, data, index);
    }
}
