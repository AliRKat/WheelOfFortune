using UnityEngine;

[System.Serializable]
public struct SlotViewData {
    public Sprite icon;
    public string valueText;
    public bool isVisible;

    public SlotViewData(Sprite icon, string valueText, bool isVisible = true) {
        this.icon = icon;
        this.valueText = valueText;
        this.isVisible = isVisible;
    }
}
