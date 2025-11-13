using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class BombPopupUI : MonoBehaviour {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;
        public System.Action OnContinue;
        public System.Action OnQuit;

#if UNITY_EDITOR
        private void OnValidate() {
            var buttonsRoot = transform.Find("Buttons");
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
    }
}
