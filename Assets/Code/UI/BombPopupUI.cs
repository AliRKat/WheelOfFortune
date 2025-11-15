using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class BombPopupUI : MonoBehaviour {

        #region Serialized Fields

        [Header("Buttons")]
        [SerializeField] private Button _continueButton_value;
        [SerializeField] private Button _quitButton_value;

        [Header("Content Root")]
        [SerializeField] private Transform _contentRoot_value;

        #endregion

        #region Public Callbacks

        public System.Action OnContinue;
        public System.Action OnQuit;

        #endregion

        #region Internal State

        private Sequence _introSeq;
        private Sequence _outroSeq;

        #endregion

        #region Editor Validation

#if UNITY_EDITOR
        private void OnValidate() {
            _contentRoot_value = transform.Find("Content");

            Transform buttonsRoot = transform.Find("Content/Buttons");
            if (buttonsRoot != null) {
                _continueButton_value = buttonsRoot.Find("Button_Continue")?.GetComponent<Button>();
                _quitButton_value = buttonsRoot.Find("Button_Quit")?.GetComponent<Button>();
            }
        }
#endif

        #endregion

        #region Unity Events

        private void Awake() {
            if (_continueButton_value != null) {
                _continueButton_value.onClick.RemoveAllListeners();
                _continueButton_value.onClick.AddListener(OnContinuePressed);
            }

            if (_quitButton_value != null) {
                _quitButton_value.onClick.RemoveAllListeners();
                _quitButton_value.onClick.AddListener(OnQuitPressed);
            }
        }

        private void OnEnable() {
            PlayShow(_contentRoot_value);
        }

        #endregion

        #region Public UI Animation API

        /// <summary>
        /// Plays the popup intro animation (scale + fade + button reveals).
        /// </summary>
        public void PlayShow(Transform target, float duration = 0.35f) {
            if (target == null)
                return;

            KillSequences();

            Transform buttonsRoot = target.Find("Buttons");
            Transform continueBtn = buttonsRoot?.Find("Button_Continue");
            Transform quitBtn = buttonsRoot?.Find("Button_Quit");

            CanvasGroup cg = PrepareCanvasGroup(target);
            PrepareInitialPopupState(target);
            PrepareButtonInitialState(continueBtn);
            PrepareButtonInitialState(quitBtn);

            _introSeq = DOTween.Sequence();

            // POPUP CONTENT INTRO
            AppendPopupIntro(_introSeq, target, cg, duration);

            // BUTTON APPEAR SEQUENCE
            _introSeq.AppendInterval(0.05f);
            AppendButtonIntro(_introSeq, continueBtn, 0.22f);

            _introSeq.AppendInterval(0.06f);
            AppendButtonIntro(_introSeq, quitBtn, 0.22f);

            _introSeq.Play();
        }

        #endregion

        #region Outro Animation

        private void PlayHide(System.Action onComplete = null) {
            KillSequences();

            if (_contentRoot_value == null) {
                Destroy(gameObject);
                return;
            }

            Transform buttonsRoot = _contentRoot_value.Find("Buttons");
            Transform continueBtn = buttonsRoot?.Find("Button_Continue");
            Transform quitBtn = buttonsRoot?.Find("Button_Quit");
            CanvasGroup cg = _contentRoot_value.GetComponent<CanvasGroup>();

            _outroSeq = DOTween.Sequence();

            // BUTTONS OUTRO
            AppendButtonOutro(_outroSeq, continueBtn, 0.12f);
            AppendButtonOutro(_outroSeq, quitBtn, 0.12f);

            // POPUP FADE / SHRINK
            AppendPopupOutro(_outroSeq, _contentRoot_value, cg);

            _outroSeq.OnComplete(() => {
                onComplete?.Invoke();
                Destroy(gameObject);
            });

            _outroSeq.Play();
        }

        #endregion

        #region Button Callbacks

        private void OnContinuePressed() {
            OnContinue?.Invoke();
            PlayHide();
        }

        private void OnQuitPressed() {
            OnQuit?.Invoke();
            PlayHide();
        }

        #endregion

        #region Helper: Initial Setup

        private CanvasGroup PrepareCanvasGroup(Transform target) {
            CanvasGroup cg = target.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = target.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            return cg;
        }

        private void PrepareInitialPopupState(Transform target) {
            target.localScale = Vector3.one * 0.85f;
        }

        private void PrepareButtonInitialState(Transform btn) {
            if (btn == null)
                return;

            btn.localScale = Vector3.one * 0.6f;

            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = btn.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
        }

        #endregion

        #region Helper: Intro Animations

        private void AppendPopupIntro(Sequence seq, Transform target, CanvasGroup cg, float duration) {
            seq.Append(target.DOScale(1f, duration).SetEase(Ease.OutCubic));
            seq.Join(cg.DOFade(1f, duration * 0.9f).SetEase(Ease.OutQuad));
        }

        private void AppendButtonIntro(Sequence seq, Transform btn, float scaleDur) {
            if (btn == null)
                return;

            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            seq.Append(btn.DOScale(1f, scaleDur).SetEase(Ease.OutBack));
            seq.Join(cg.DOFade(1f, scaleDur * 0.85f).SetEase(Ease.OutQuad));
        }

        #endregion

        #region Helper: Outro Animations

        private void AppendButtonOutro(Sequence seq, Transform btn, float dur) {
            if (btn == null)
                return;

            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            seq.Join(btn.DOScale(0.85f, dur).SetEase(Ease.InQuad));
            seq.Join(cg.DOFade(0f, dur).SetEase(Ease.InQuad));
        }

        private void AppendPopupOutro(Sequence seq, Transform root, CanvasGroup cg) {
            seq.Append(root.DOScale(0.75f, 0.20f).SetEase(Ease.InCubic));
            if (cg != null)
                seq.Join(cg.DOFade(0f, 0.18f).SetEase(Ease.InQuad));
        }

        #endregion

        #region Helper: Sequence Safety

        private void KillSequences() {
            _introSeq?.Kill();
            _outroSeq?.Kill();
        }

        #endregion
    }
}
