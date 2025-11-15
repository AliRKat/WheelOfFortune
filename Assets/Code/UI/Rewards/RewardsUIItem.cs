using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Code.UI {
    public class RewardsUIItem : MonoBehaviour {
        [SerializeField] private Image _icon_value;
        [SerializeField] private TMP_Text _text_value;
        [SerializeField] private string _id;

        [SerializeField] private int _prevAmount;
        [SerializeField] private int _currentAmount;
        private Tween _countTween;

        public void Set(Sprite icon, int newAmount, string category) {
            _icon_value.sprite = icon;
            _id = category;
            _prevAmount = _currentAmount;
            _text_value.text = _prevAmount.ToString();
            _currentAmount = newAmount;
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

        public void AnimateAmountChange() {
            _countTween?.Kill();

            int startValue = _prevAmount;
            int endValue = _currentAmount;

            const float duration = 0.25f; // smooth, clearly visible
            const Ease easeType = Ease.OutQuad;

            _countTween = DOTween
                .To(() => startValue,
                    v => {
                        startValue = v;
                        _text_value.text = v.ToString();
                    },
                    endValue,
                    duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    _prevAmount = _currentAmount;
                    _text_value.text = endValue.ToString();
                });
        }


    }
}
