using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSlotController : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _valueText;

    public void SetData(SlotViewData data) {
        _icon.enabled = data.isVisible;
        _valueText.enabled = data.isVisible;

        if (!data.isVisible) {
            _valueText.text = "";
            return;
        }

        _icon.sprite = data.icon;
        _valueText.text = data.valueText;
    }

    public void Clear() {
        _icon.enabled = false;
        _valueText.enabled = false;
        _valueText.text = "";
    }
}
