using System.Collections.Generic;
using UnityEngine;

public class WheelView : MonoBehaviour {
    [Header("References")]
    [SerializeField] private List<WheelSlotController> _slots;
    [SerializeField] private Transform _wheelVisual;
    [SerializeField] private Sprite _bombSprite;

    /// <summary>
    /// Populates the wheel UI with the given ZoneConfig data.
    /// Each slot is filled with its corresponding reward visuals.
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

        // Fill or clear slots depending on zone data
        for (int i = 0; i < _slots.Count; i++) {
            if (i >= zone.slices.Count) {
                _slots[i].SetData(new SlotViewData(null, "", false));
                continue;
            }

            var slice = zone.slices[i];

            if (slice.isBomb) {
                _slots[i].SetData(new SlotViewData(_bombSprite, "", true));
                continue;
            }

            if (slice.reward == null) {
                _slots[i].SetData(new SlotViewData(null, "", false));
                continue;
            }

            int amount = slice.customAmount > 0
                ? slice.customAmount
                : slice.reward.baseAmount;

            var data = new SlotViewData(slice.reward.icon, amount.ToString(), true);
            _slots[i].SetData(data);
        }
    }

    /// <summary>
    /// Clears all slot visuals (used before setting up a new zone or when resetting).
    /// </summary>
    public void ClearAll() {
        foreach (var slot in _slots)
            slot.Clear();
    }

    /// <summary>
    /// Spins the wheel visually (placeholder for animation).
    /// </summary>
    public void SpinVisual(float rotationAmount = 720f, float duration = 2f) {
        // Optional placeholder for animation, e.g. using DOTween later.
        // _wheelVisual.DORotate(new Vector3(0, 0, rotationAmount), duration, RotateMode.FastBeyond360);
    }
}
