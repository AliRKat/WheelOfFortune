using System.Collections.Generic;
using UnityEngine;

public class WheelView : MonoBehaviour {
    [Header("References")]
    [SerializeField] private List<WheelSlotController> _slots;
    [SerializeField] private Transform _wheelVisual;

    /// <summary>
    /// Populates the wheel slots based on the given ZoneConfig.
    /// Each slice is rendered using its WheelItemData.
    /// </summary>
    public void SetUp(ZoneConfig zone) {
        if (zone == null) {
            Debug.LogError("WheelView: ZoneConfig is null.");
            return;
        }

        if (_slots == null || _slots.Count == 0) {
            Debug.LogError("WheelView: Slot references are missing.");
            return;
        }

        for (int i = 0; i < _slots.Count; i++) {
            if (i >= zone.slices.Count) {
                _slots[i].SetData(new SlotViewData(null, "", false));
                continue;
            }

            var slice = zone.slices[i];
            var item = slice.itemData;

            if (item == null) {
                _slots[i].SetData(new SlotViewData(null, "", false));
                continue;
            }

            string amountText = item.showAmount
                ? (slice.customAmount > 0 ? slice.customAmount : item.baseAmount).ToString()
                : "";

            var viewData = new SlotViewData(item.icon, amountText, true);
            _slots[i].SetData(viewData);
        }
    }

    /// <summary>
    /// Clears all slot visuals (used before switching zones or resetting).
    /// </summary>
    public void ClearAll() {
        foreach (var slot in _slots)
            slot.Clear();
    }

    /// <summary>
    /// Visual spin animation placeholder. Can be replaced with tweening later.
    /// </summary>
    public void SpinVisual(float rotationAmount = 720f, float duration = 2f) {
        // Example (if DOTween is used):
        // _wheelVisual.DORotate(new Vector3(0, 0, rotationAmount), duration, RotateMode.FastBeyond360);
    }
}