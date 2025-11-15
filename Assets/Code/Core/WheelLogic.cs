using Code.Core;
using UnityEngine;

public class WheelLogic {

    #region Public API

    /// <summary>
    /// Selects a random slice from the given zone and produces a spin result.
    /// </summary>
    public SpinResult Spin(ZoneConfig zone) {
        if (!IsZoneValid(zone))
            return CreateInvalidResult();

        int index = Random.Range(0, zone.slices.Count);
        var slice = zone.slices[index];

        var data = slice.itemData;
        if (data == null)
            return CreateMissingItemDataResult(index);

        if (data.category == WheelItemCategory.Bomb)
            return new SpinResult(true, 0, data, index);

        int amount = ResolveAmount(slice, data);

        return new SpinResult(false, amount, data, index);
    }

    #endregion

    #region Validation

    private bool IsZoneValid(ZoneConfig zone) {
        if (zone == null || zone.slices == null || zone.slices.Count == 0) {
            GameLogger.Error(this, "Spin", "ZoneValidation", "Zone or slice list is invalid.");
            return false;
        }

        return true;
    }

    #endregion

    #region Result Construction

    private SpinResult CreateInvalidResult() {
        return new SpinResult(true, 0, null, -1);
    }

    private SpinResult CreateMissingItemDataResult(int index) {
        GameLogger.Warn(this, "Spin", "MissingItemData",
            $"Slice {index} contains null itemData.");

        return new SpinResult(true, 0, null, index);
    }

    #endregion

    #region Amount Resolution

    private int ResolveAmount(ZoneSlice slice, WheelItemData data) {
        int baseAmount = data.baseAmount;
        return slice.customAmount > 0 ? slice.customAmount : baseAmount;
    }

    #endregion
}
