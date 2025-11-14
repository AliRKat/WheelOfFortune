using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.Core;

public class WheelViewInitData {
    public ZoneConfig zoneConfig;
    public WheelSkinData wheelSkinData;
}

public class WheelView : MonoBehaviour {
    [Header("References")]
    [SerializeField] private WheelSlotController[] _slots;
    [SerializeField] private Transform _wheelVisual;

    [Header("Spin Settings")]
    [SerializeField] private float _indicatorAngle = 90f; // arrow points UP (90° in Atan2 space)
    [SerializeField] private int _extraSpins = 3;

    [SerializeField] private Image _baseWheelImage;
    [SerializeField] private Image _indicatorImage;
    private Tweener _spinTween;
    private WheelSlotController _winningSlot;

    // ------------------------------------------------------------
    // SETUP
    // ------------------------------------------------------------
    public void SetUp(WheelViewInitData initData) {
        var zone = initData.zoneConfig;
        var skinData = initData.wheelSkinData;

        if (zone == null) {
            Debug.LogError("WheelView: ZoneConfig is null.");
            return;
        }

        if (_slots == null || _slots.Length == 0) {
            Debug.LogError("WheelView: Slot references are missing.");
            return;
        }

        ApplySkin(skinData);
        ApplySlots(zone);
    }

    public WheelSlotController GetWinningSlot() {
        return _winningSlot;
    }

    private void ApplySlots(ZoneConfig zone) {
        for (int i = 0; i < _slots.Length; i++) {

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

        _wheelVisual.localEulerAngles = Vector3.zero;
    }

    public void ClearAll() {
        foreach (var slot in _slots)
            slot.Clear();
    }

    // ------------------------------------------------------------
    // SPIN ANIMATION
    // ------------------------------------------------------------
    /// <summary>
    /// Spins the wheel visually so that the given slice index lands at the top.
    /// </summary>
    public void SpinToIndex(int targetIndex, float duration, System.Action onComplete) {
        if (targetIndex < 0 || targetIndex >= _slots.Length) {
            onComplete?.Invoke();
            return;
        }
        GameEvents.SpinStarted?.Invoke();

        // Kill any previous tween and hard reset rotation.
        _spinTween?.Kill();
        _winningSlot = _slots[targetIndex];
        _wheelVisual.localEulerAngles = Vector3.zero;

        // Get the slot transform for the winning index.
        var slotTransform = _slots[targetIndex].transform;

        // Compute its local position relative to wheel center.
        Vector3 localPos = _wheelVisual.InverseTransformPoint(slotTransform.position);

        // Angle in degrees: 0 = right, 90 = up, -90 = down, 180/-180 = left.
        float currentAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

        // We want this slot to end up at _indicatorAngle (usually 90° for "up").
        float deltaAngle = _indicatorAngle - currentAngle;

        // Add extra full spins for visuals.
        float totalRotation = (_extraSpins * 360f) + deltaAngle;

        _spinTween = _wheelVisual
            .DOLocalRotate(new Vector3(0, 0, totalRotation), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                // Ensure we end exactly at the intended angle (no float residue).
                _wheelVisual.localEulerAngles = new Vector3(0, 0, deltaAngle);
                GameEvents.SpinEnded?.Invoke();
                onComplete?.Invoke();
            });
    }

    // ------------------------------------------------------------
    // SKIN
    // ------------------------------------------------------------
    public void ApplySkin(WheelSkinData skin) {
        if (skin == null) return;
        _baseWheelImage.sprite = skin.baseWheel;
        _indicatorImage.sprite = skin.indicator;
    }
}
