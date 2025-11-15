using UnityEngine;
using Code.Core;

namespace Code.Managers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        [Header("Core References")]
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private UIManager _uiManager;

        private WheelLogic _wheelLogic;
        private ZoneManager _zoneManager;
        private RewardManager _rewardManager;
        private WheelSkinDatabase _wheelSkinDatabase;

        private bool _waitingForChoice = false;
        private bool _initialized = false;
        private bool _isSpinning = false;

        #region Unity Lifecycle

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _wheelLogic = new WheelLogic();
            _zoneManager = new ZoneManager();
            _rewardManager = new RewardManager();

            _wheelSkinDatabase = Resources.Load<WheelSkinDatabase>("WheelSkinDatabase");

            GameLogger.Log(this, "Awake", "Init",
                "Constructed WheelLogic / ZoneManager / RewardManager");
        }

        private void Start() {
            _uiManager.Init(_rewardManager, _zoneManager);

            GameLogger.Log(this, "Start", "UIManager",
                "UIManager initialized");

            InitializeCurrentZone();
        }

        #endregion

        #region Public UI Requests

        /// <summary>
        /// Requests a wheel spin if the game state allows it.
        /// </summary>
        public void RequestSpin() {
            if (!CanSpin()) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Spin blocked");
                return;
            }

            PerformSpin();
        }

        /// <summary>
        /// Attempts to exit the current run and reset the game.
        /// </summary>
        public void RequestExit() {
            if (!_initialized || _waitingForChoice || _isSpinning)
                return;

            var collectedString = _rewardManager.GetEarningsString();

            GameLogger.Log(this, "RequestExit", "Collect",
                $"Player exited. Earnings: {collectedString}");

            _rewardManager.Reset();
            _zoneManager.ResetToStart();

            InitializeCurrentZone();

            GameLogger.Log(this, "RequestExit", "Flow",
                "Game reset after exit.");
        }

        /// <summary>
        /// Resolves the bomb choice and continues or resets the game.
        /// </summary>
        public void ResolveBombChoice(bool payToContinue) {
            _waitingForChoice = false;

            if (payToContinue) {
                GameLogger.Log(this, "ResolveBombChoice", "Bomb",
                    "Player continued (paid)");
            } else {
                _rewardManager.Reset();
                _zoneManager.ResetToStart();
                GameLogger.Log(this, "ResolveBombChoice", "Bomb",
                    "Player quit (lost rewards)");
            }

            InitializeCurrentZone();
        }

        #endregion

        #region Spin Flow

        /// <summary>
        /// Returns true if all conditions allow a spin action.
        /// </summary>
        private bool CanSpin() {
            if (!_initialized) return false;
            if (_waitingForChoice) return false;
            if (_isSpinning) return false;
            return true;
        }

        private void PerformSpin() {
            if (!CanSpin()) {
                GameLogger.Warn(this, "PerformSpin", "Guard", "Spin ignored due to state");
                return;
            }

            _isSpinning = true;
            _uiManager.UpdateExitButtonVisibility(true);

            var zone = _zoneManager.GetCurrentZone();

            GameLogger.Log(this, "PerformSpin", "Start",
                $"Logic spin for {zone.zoneId}");

            var result = _wheelLogic.Spin(zone);
            int winIndex = result.WinningIndex;

            GameLogger.Log(this, "PerformSpin", "Result",
                $"Logic selected slice index = {winIndex}");

            _wheelView.SpinToIndex(winIndex, 2f, () => ResolveSpin(result));
        }

        private void ResolveSpin(SpinResult result) {
            GameLogger.Log(this, "ResolveSpin", "AnimDone",
                "Spin animation finished");

            if (result.IsBomb) {
                GameLogger.Warn(this, "ResolveSpin", "Bomb",
                    "Bomb hit → waiting for UI decision");

                _waitingForChoice = true;
                GameEvents.BombHit?.Invoke();
                return;
            }

            _rewardManager.Add(result.RewardData, result.RewardAmount);

            GameLogger.Log(this, "ResolveSpin", "Reward",
                $"Gained +{result.RewardAmount} ({result.RewardData.itemId})");

            _uiManager.RefreshRewardsUI();

            const float vfxDelay = 0.15f;
            DG.Tweening.DOVirtual.DelayedCall(vfxDelay, () => {
                var slotTransform = _wheelView.GetWinningSlot().transform;
                _uiManager.PlayVFX(result.RewardData.icon, slotTransform, result.RewardData.itemId);
            });

            const float postDelay = 0.8f;
            DG.Tweening.DOVirtual.DelayedCall(postDelay, () => {
                _zoneManager.AdvanceZone();
                InitializeCurrentZone();
            });
        }

        #endregion

        #region Zone Initialization

        private void InitializeCurrentZone() {
            var zone = _zoneManager.GetCurrentZone();

            if (zone == null) {
                GameLogger.Error(this, "InitializeCurrentZone", "ZoneNull",
                    "ZoneConfig NOT FOUND!");
                return;
            }

            _uiManager.UpdateExitButtonVisibility(false);

            GameLogger.Log(this, "InitializeCurrentZone", "Zone",
                $"Initializing zone {zone.zoneId} ({zone.type})");

            _wheelView.ClearAll();

            var wheelInitData = new WheelViewInitData {
                zoneConfig = zone,
                wheelSkinData = _wheelSkinDatabase.GetSkin(zone.type)
            };

            _wheelView.SetUp(wheelInitData);

            _initialized = true;

            _uiManager.RefreshZoneUI();
            _uiManager.RefreshRewardsUI();

            _isSpinning = false;
        }

        #endregion
    }
}
