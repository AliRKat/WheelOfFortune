using UnityEngine;

public class WheelLogic
{
    public SpinResult Spin(ZoneConfig zone)
    {
        if (zone == null || zone.slices == null || zone.slices.Count == 0)
            return SpinResult.Empty;

        int index = Random.Range(0, zone.slices.Count);
        var slice = zone.slices[index];

        if (slice.isBomb)
            return new SpinResult(true, 0, "bomb", null);

        int finalAmount = slice.customAmount > 0
            ? slice.customAmount
            : slice.reward.baseAmount;

        return new SpinResult(false, finalAmount, slice.reward.rewardId, slice.reward);
    }
}
