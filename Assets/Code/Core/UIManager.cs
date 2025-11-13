using System.Collections.Generic;
using Code.Core;
using Code.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Managers {
    public class UIManager : MonoBehaviour {
        [Header("Roots")]
        [SerializeField] private Transform _buttonsRoot;
        [SerializeField] private Transform _rewardsRoot;
        [SerializeField] private Transform _popupRoot;

        [Header("Buttons")]
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _exitButton;

        [Header("UI Elements")]
        [SerializeField] private RewardsUI _rewardsUI;
        [SerializeField] private ZoneProgressUI _zoneProgressUI;
        [SerializeField] private BombPopupUI _bombPopupPrefab;

        [Header("Mapping")]
        [SerializeField] private RewardUIMap _rewardMap;

        private RewardManager _rewardManager;
        private ZoneManager _zoneManager;

#if UNITY_EDITOR
        private void OnValidate() {
            // Ensure roots exist
            if (_buttonsRoot == null)
                _buttonsRoot = transform.parent.Find("Panel_Buttons");

            if (_rewardsRoot == null) {
                _rewardsRoot = transform.parent.Find("Panel_Rewards/RewardsUI");
            }

            Transform rewardsTopRoot = transform.parent.Find("Panel_Rewards/RewardsUI/TopContent");

            // Auto-bind buttons RELATIVE to the roots
            if (_spinButton == null && _buttonsRoot != null)
                _spinButton = _buttonsRoot.Find("Button_Spin")?.GetComponent<Button>();

            if (_exitButton == null && _rewardsRoot != null)
                _exitButton = rewardsTopRoot.Find("Button_Exit")?.GetComponent<Button>();

            if (_zoneProgressUI == null)
                _zoneProgressUI = transform.parent.Find("Panel_TopUI/ZoneIndicator")?.GetComponent<ZoneProgressUI>();

            if (_rewardsUI == null && _rewardsRoot != null)
                _rewardsUI = _rewardsRoot.GetComponentInChildren<RewardsUI>(true);

            // Reward Icon Map (ScriptableObject)
            if (_rewardMap == null)
                _rewardMap = Resources.Load<RewardUIMap>("RewardUIMap");

            if (_popupRoot == null)
                _popupRoot = transform.parent.Find("Panel_Popups");
        }
#endif

        public void Init(RewardManager rewardManager, ZoneManager zoneManager) {
            _rewardManager = rewardManager;
            _zoneManager = zoneManager;

            _zoneProgressUI.Init(_zoneManager);

            GameEvents.BombHit += OnBombHit;
            BindButtonEvents();
        }

        public void RefreshRewardsUI() {
            var totals = _rewardManager.GetTotals();
            var dtoList = BuildRewardDTOs(totals);

            _rewardsUI.Populate(dtoList);
        }

        public void RefreshZoneUI() {
            _zoneProgressUI.Refresh();
        }

        public void UpdateExitButtonVisibility(bool isSpinning) {
            if (_exitButton == null)
                return;

            var zone = _zoneManager.GetCurrentZone();
            bool canExit = !isSpinning && (zone.type == ZoneType.Safe || zone.type == ZoneType.Super);

            _exitButton.gameObject.SetActive(canExit);
        }

        private void OnBombHit() {
            UpdateExitButtonVisibility(true);
            InstantiateBombPopup();
        }

        private void InstantiateBombPopup() {
            var popup = Instantiate(_bombPopupPrefab, _popupRoot);

            popup.OnContinue = () => {
                GameManager.Instance.ResolveBombChoice(true);
            };

            popup.OnQuit = () => {
                GameManager.Instance.ResolveBombChoice(false);
            };
        }

        private List<RewardItemDTO> BuildRewardDTOs(Dictionary<string, int> totals) {
            var dtos = new List<RewardItemDTO>();

            foreach (var kv in totals) {
                string id = kv.Key;
                int amount = kv.Value;

                var icon = _rewardMap.GetIcon(id);
                if (icon == null)
                    continue;

                dtos.Add(new RewardItemDTO {
                    Icon = icon,
                    Amount = amount
                });
            }

            return dtos;
        }

        private void BindButtonEvents() {
            _spinButton.onClick.RemoveAllListeners();
            _spinButton.onClick.AddListener(() => GameManager.Instance.RequestSpin());

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(() => GameManager.Instance.RequestExit());
        }
    }
}
