using UnityEngine;
using TMPro;

public class ZoneEntryUI : MonoBehaviour {
    [SerializeField] private TMP_Text _label;
    public void SetData(int zoneIndex, ZoneType type) {
        if (zoneIndex <= 0) {
            _label.text = "";
            return;
        }

        _label.text = zoneIndex.ToString();
        _label.color = type switch {
            ZoneType.Safe => Color.green,
            ZoneType.Super => Color.yellow,
            _ => Color.white
        };
    }

}