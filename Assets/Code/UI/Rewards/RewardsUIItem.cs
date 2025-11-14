using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.UI {
    public class RewardsUIItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _id;

        public void Set(Sprite icon, string label, string category) {
            _icon.sprite = icon;
            _text.text = label;
            _id = category;
        }

        public string GetId() {
            return _id;
        }
    }
}