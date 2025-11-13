using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.UI {
    public class RewardsUIItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;

        public void Set(Sprite icon, string label) {
            _icon.sprite = icon;
            _text.text = label;
        }
    }
}