using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelViewInitData {
    public ZoneConfig zoneConfig;
    public WheelSkinData wheelSkinData;
}

public class WheelView : MonoBehaviour {
    [Header("References")]
    [SerializeField] private List<WheelSlotController> _slots;
    [SerializeField] private Transform _wheelVisual;

    [SerializeField] private Image _baseWheelImage;
    [SerializeField] private Image _indicatorImage;

    /// <summary>
    /// Populates the wheel slots based on the given ZoneConfig.
    /// Each slice is rendered using its WheelItemData.
    /// </summary>
    public void SetUp(WheelViewInitData initData) {
        var zone = initData.zoneConfig;
        var skinData = initData.wheelSkinData;

        if (zone == null) {
            Debug.LogError("WheelView: ZoneConfig is null.");
            return;
        }

        if (_slots == null || _slots.Count == 0) {
            Debug.LogError("WheelView: Slot references are missing.");
            return;
        }

        ApplySkin(skinData);
        
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

    public void ApplySkin(WheelSkinData skin) {
        if (skin == null) return;
        _baseWheelImage.sprite = skin.baseWheel;
        _indicatorImage.sprite = skin.indicator;
    }

}