using System.Collections.Generic;
using Code.Core;
using Code.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Managers {
    public class UIManager : MonoBehaviour {

        #region Serialized Fields

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
        [SerializeField] private Image _iconPrefab;

        #endregion

        #region Private Fields

        private RewardManager _rewardManager;
        private ZoneManager _zoneManager;
        private static string panelHeader = "ui_panel_";

        #endregion

        #region Editor Validation

#if UNITY_EDITOR
        private void OnValidate() {
            ValidateRootReferences();
            ValidateButtonReferences();
            ValidateUIReferences();
            ValidateRewardMap();
        }

        private void ValidateRootReferences() {
            if (_buttonsRoot == null)
                _buttonsRoot = transform.parent.Find($"{panelHeader}Buttons");

            if (_rewardsRoot == null)
                _rewardsRoot = transform.parent.Find($"{panelHeader}Rewards/ui_rewards");

            if (_popupRoot == null)
                _popupRoot = transform.parent.Find($"{panelHeader}Popups");
        }

        private void ValidateButtonReferences() {
            if (_buttonsRoot != null && _spinButton == null)
                _spinButton = _buttonsRoot.Find("Button_Spin")?.GetComponent<Button>();

            Transform rewardsTopRoot = transform.parent.Find($"{panelHeader}Rewards/ui_rewards/TopContent");

            if (rewardsTopRoot != null && _exitButton == null)
                _exitButton = rewardsTopRoot.Find("Button_Exit")?.GetComponent<Button>();
        }

        private void ValidateUIReferences() {
            if (_zoneProgressUI == null)
                _zoneProgressUI = transform.parent.Find($"{panelHeader}TopUI/ui_element_zoneIndicator")
                    ?.GetComponent<ZoneProgressUI>();

            if (_rewardsUI == null && _rewardsRoot != null)
                _rewardsUI = _rewardsRoot.GetComponentInChildren<RewardsUI>(true);
        }

        private void ValidateRewardMap() {
            if (_rewardMap == null)
                _rewardMap = Resources.Load<RewardUIMap>("RewardUIMap");
        }
#endif

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the UI with game managers and binds event listeners.
        /// </summary>
        public void Init(RewardManager rewardManager, ZoneManager zoneManager) {
            _rewardManager = rewardManager;
            _zoneManager = zoneManager;

            _zoneProgressUI.Init(_zoneManager);

            GameEvents.BombHit += OnBombHit;
            GameEvents.SpinStarted += OnSpinStarted;
            GameEvents.SpinEnded += OnSpinEnded;

            BindButtonEvents();
        }

        void OnDestroy() {
            GameEvents.BombHit -= OnBombHit;
            GameEvents.SpinStarted -= OnSpinStarted;
            GameEvents.SpinEnded -= OnSpinEnded;
        }

        #endregion

        #region UI Refresh

        /// <summary>
        /// Updates the reward panel display based on current stored rewards.
        /// </summary>
        public void RefreshRewardsUI() {
            var totals = _rewardManager.GetTotals();
            var dtoList = BuildRewardDTOs(totals);

            _rewardsUI.Populate(dtoList);
        }

        /// <summary>
        /// Refreshes the zone progress UI.
        /// </summary>
        public void RefreshZoneUI() {
            _zoneProgressUI.Refresh();
        }

        /// <summary>
        /// Updates the visibility of the exit button based on current game state.
        /// </summary>
        public void UpdateExitButtonVisibility(bool isSpinning, bool waitingForBomb = false) {
            bool show = !isSpinning && !waitingForBomb;
            _exitButton.gameObject.SetActive(show);
        }

        #endregion

        #region VFX

        /// <summary>
        /// Plays burst + fly-to-inventory VFX for reward collection.
        /// </summary>
        public void PlayVFX(Sprite icon, Transform fromPoint, string rewardId) {
            if (icon == null || fromPoint == null)
                return;

            RewardsUIItem target = _rewardsUI.GetSpawnedWithId(rewardId);
            if (target == null)
                return; // cannot animate without a target

            for (int i = 0; i < 6; i++)
                SpawnRewardIcon(icon, fromPoint, target);

            target.AnimateAmountChange();
        }

        private void SpawnRewardIcon(Sprite icon, Transform fromPoint, RewardsUIItem target) {
            const float radius = 60f;
            const float popDuration = 0.2f;
            const float flyDuration = 0.2f;

            Image inst = Instantiate(_iconPrefab, _popupRoot);
            inst.sprite = icon;

            Transform instTransform = inst.transform;

            Vector3 startWorld = fromPoint.position;
            Vector3 targetWorld = target.transform.position;

            instTransform.position = startWorld;

            Vector2 burstOffset = Random.insideUnitCircle * radius;
            Vector3 burstWorld = startWorld + new Vector3(burstOffset.x, burstOffset.y, 0);

            Sequence seq = DOTween.Sequence();
            seq.Append(instTransform.DOMove(burstWorld, popDuration).SetEase(Ease.OutQuad));
            seq.Append(instTransform.DOMove(targetWorld, flyDuration).SetEase(Ease.InQuad));

            seq.OnComplete(() => {
                target.PlayBumpEffect();
                Destroy(inst.gameObject);
            });
        }

        #endregion

        #region Event Handlers

        private void OnSpinStarted() {
            UpdateExitButtonVisibility(true);
        }

        private void OnSpinEnded() {
            UpdateExitButtonVisibility(false);
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

        #endregion

        #region Helpers

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
                    Amount = amount,
                    Id = id
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

        #endregion
    }
}
