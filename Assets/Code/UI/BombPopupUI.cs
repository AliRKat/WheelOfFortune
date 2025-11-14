using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class BombPopupUI : MonoBehaviour {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Transform _content;
        public System.Action OnContinue;
        public System.Action OnQuit;

#if UNITY_EDITOR
        private void OnValidate() {
            var buttonsRoot = transform.Find("Content/Buttons");
            _content = transform.Find("Content");
            _continueButton = buttonsRoot.Find("Button_Continue").GetComponent<Button>();
            _quitButton = buttonsRoot.Find("Button_Quit").GetComponent<Button>();
        }
#endif

        private void Awake() {
            _continueButton.onClick.AddListener(() => {
                OnContinue?.Invoke();
                Destroy(gameObject);
            });

            _quitButton.onClick.AddListener(() => {
                OnQuit?.Invoke();
                Destroy(gameObject);
            });
        }

        private void OnEnable() {
            PlayShow(_content);
        }

        public void PlayShow(Transform target, float duration = 0.35f) {
            if (target == null)
                return;

            target.gameObject.SetActive(true);

            // Fade-in (optional)
            CanvasGroup cg = target.GetComponent<CanvasGroup>();
            if (cg != null) {
                cg.alpha = 0f;
                cg.DOFade(1f, duration * 0.9f)
                    .SetEase(Ease.OutQuad);
            }

            // Smooth upscale
            target.localScale = Vector3.one * 0.85f;

            target.DOScale(1f, duration)
                .SetEase(Ease.OutCubic); // soft, smooth, premium feel
        }

    }
}
