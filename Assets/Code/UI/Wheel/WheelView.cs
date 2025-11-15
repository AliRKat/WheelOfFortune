using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.Core;

public class WheelViewInitData {
    public ZoneConfig zoneConfig;
    public WheelSkinData wheelSkinData;
}

public class WheelView : MonoBehaviour {

    #region Serialized Fields

    [Header("References")]
    [SerializeField] private WheelSlotController[] _slots;
    [SerializeField] private Transform _wheelVisual;

    [Header("Spin Settings")]
    [SerializeField] private float _indicatorAngle = 90f;
    [SerializeField] private int _extraSpins = 3;

    [Header("Skin Elements")]
    [SerializeField] private Image _baseWheelImage_value;
    [SerializeField] private Image _indicatorImage_value;

    #endregion

    #region Private Fields

    private Tweener _spinTween;
    private WheelSlotController _winningSlot;

    #endregion

    #region Public API

    /// <summary>
    /// Initializes the wheel with the provided zone configuration and skin data.
    /// </summary>
    public void SetUp(WheelViewInitData initData) {
        if (!ValidateInitData(initData))
            return;

        ApplySkin(initData.wheelSkinData);
        ApplySlots(initData.zoneConfig);
    }

    /// <summary>
    /// Returns the slot that corresponds to the last winning spin result.
    /// </summary>
    public WheelSlotController GetWinningSlot() {
        return _winningSlot;
    }

    /// <summary>
    /// Spins the wheel so that the given slot index lands at the indicator.
    /// </summary>
    public void SpinToIndex(int targetIndex, float duration, System.Action onComplete) {
        if (!ValidateSpinIndex(targetIndex, onComplete))
            return;

        GameEvents.SpinStarted?.Invoke();

        _spinTween?.Kill();
        _winningSlot = _slots[targetIndex];
        _wheelVisual.localEulerAngles = Vector3.zero;

        float deltaAngle = ComputeDeltaAngle(targetIndex);
        float totalRotation = (_extraSpins * 360f) + deltaAngle;

        _spinTween = _wheelVisual
            .DOLocalRotate(new Vector3(0, 0, totalRotation), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => FinishSpin(deltaAngle, onComplete));
    }

    /// <summary>
    /// Clears all slot visuals.
    /// </summary>
    public void ClearAll() {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Clear();
    }

    #endregion

    #region Setup

    private bool ValidateInitData(WheelViewInitData initData) {
        if (initData == null || initData.zoneConfig == null) {
            GameLogger.Error(this, "SetUp", "Init", "ZoneConfig is null.");
            return false;
        }

        if (_slots == null || _slots.Length == 0) {
            GameLogger.Error(this, "SetUp", "Init", "Slots array not assigned.");
            return false;
        }

        return true;
    }

    private void ApplySlots(ZoneConfig zone) {
        int sliceCount = zone.slices.Count;

        for (int i = 0; i < _slots.Length; i++) {
            if (i >= sliceCount) {
                _slots[i].SetData(new SlotViewData(null, "", false));
                continue;
            }

            ApplySingleSlot(_slots[i], zone.slices[i]);
        }

        _wheelVisual.localEulerAngles = Vector3.zero;
    }

    private void ApplySingleSlot(WheelSlotController slot, ZoneSlice slice) {
        var item = slice.itemData;

        if (item == null) {
            slot.SetData(new SlotViewData(null, "", false));
            return;
        }

        string amountText = item.showAmount
            ? (slice.customAmount > 0 ? slice.customAmount : item.baseAmount).ToString()
            : "";

        var viewData = new SlotViewData(item.icon, amountText, true);
        slot.SetData(viewData);
    }

    #endregion

    #region Spin Calculation

    private bool ValidateSpinIndex(int index, System.Action onComplete) {
        if (index < 0 || index >= _slots.Length) {
            GameLogger.Warn(this, "SpinToIndex", "Guard", "Invalid spin index.");
            onComplete?.Invoke();
            return false;
        }
        return true;
    }

    private float ComputeDeltaAngle(int targetIndex) {
        var slotTransform = _slots[targetIndex].transform;

        Vector3 localPos = _wheelVisual.InverseTransformPoint(slotTransform.position);
        float currentAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

        return _indicatorAngle - currentAngle;
    }

    private void FinishSpin(float deltaAngle, System.Action onComplete) {
        _wheelVisual.localEulerAngles = new Vector3(0, 0, deltaAngle);

        GameEvents.SpinEnded?.Invoke();
        onComplete?.Invoke();
    }

    #endregion

    #region Skin

    private void ApplySkin(WheelSkinData skin) {
        if (skin == null)
            return;

        _baseWheelImage_value.sprite = skin.baseWheel;
        _indicatorImage_value.sprite = skin.indicator;
    }

    #endregion
}
