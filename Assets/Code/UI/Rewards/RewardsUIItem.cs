using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Code.UI {
    public class RewardsUIItem : MonoBehaviour {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TMP_Text _text_value;
        [SerializeField] private string _id;
        // [SerializeField] private int _prevAmount;
        // [SerializeField] private int _currentAmount;
        // private Tween _countTween;

        public void Set(Sprite icon, int newAmount, string category) {
            _icon_value.sprite = icon;
            _id = category;
            _text_value.text = newAmount.ToString();

            // _prevAmount = _currentAmount;
            //_currentAmount = newAmount;
        }

        public string GetId() {
            return _id;
        }

        public void PlayBumpEffect() {
            _icon_value.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(_icon_value.transform.DOScale(1.15f, 0.08f).SetEase(Ease.OutQuad));
            seq.Append(_icon_value.transform.DOScale(1f, 0.08f).SetEase(Ease.InQuad));
        }

        // public void AnimateAmountChange() {
        //     // Stop previous tween
        //     _countTween?.Kill();

        //     int startValue = _prevAmount;
        //     int endValue = _currentAmount;
        //     float duration = 0.15f;

        //     // Safety: if UI text has something else, sync _prevAmount once
        //     if (_text.text.Length > 0 && int.TryParse(_text.text, out int parsed)) {
        //         startValue = parsed;
        //         _prevAmount = parsed;
        //     }

        //     _countTween = DOTween.To(
        //         () => startValue,
        //         value => {
        //             _prevAmount = value;          // internal state
        //             _text.text = value.ToString(); // visual
        //         },
        //         endValue,
        //         duration
        //     )
        //     .SetEase(Ease.OutQuad)
        //     .OnComplete(() => {
        //         // ensure final value is fully correct
        //         _prevAmount = endValue;
        //         _text.text = endValue.ToString();
        //     });
        // }

    }
}
