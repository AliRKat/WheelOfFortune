using Code.UI;
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

        private bool _waitingForChoice = false;
        private bool _initialized = false;
        private bool _isSpinning = false;

        private WheelSkinDatabase _wheelSkinDatabase;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

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

        // ---------------------------------------------------------
        // REQUESTS FROM UI
        // ---------------------------------------------------------
        public void RequestSpin() {
            if (_isSpinning) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Wheel is already spinning");
                return;
            }

            if (!_initialized || _waitingForChoice) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Spin blocked");
                return;
            }

            PerformSpin();
        }

        public void RequestExit() {
            if (!_initialized || _waitingForChoice || _isSpinning)
                return;

            var collectedString = _rewardManager.GetEarningsString();
            GameLogger.Log(this, "RequestExit", "Collect",
                $"Player exited. Earnings: {collectedString}");

            _rewardManager.Reset();
            _zoneManager.ResetToStart();

            InitializeCurrentZone();
            GameLogger.Log(this, "RequestExit", "Flow", "Game reset after exit.");
        }

        public void ResolveBombChoice(bool payToContinue) {
            _waitingForChoice = false;

            if (payToContinue) {
                _zoneManager.RemoveBombFromCurrentZone();
                GameLogger.Log(this, "ResolveBombChoice", "Bomb", "Player continued (paid)");
            } else {
                _rewardManager.Reset();
                _zoneManager.ResetToStart();
                GameLogger.Log(this, "ResolveBombChoice", "Bomb", "Player quit (lost rewards)");
            }

            InitializeCurrentZone();
        }

        // ---------------------------------------------------------
        // SPIN FLOW
        // ---------------------------------------------------------
        private void PerformSpin() {
            if (!_initialized) {
                GameLogger.Warn(this, "PerformSpin", "Guard", "Spin ignored: not initialized");
                return;
            }

            if (_waitingForChoice) {
                GameLogger.Warn(this, "PerformSpin", "Guard", "Spin ignored: awaiting bomb decision");
                return;
            }

            if (_isSpinning) {
                GameLogger.Warn(this, "PerformSpin", "Guard", "Spin ignored: wheel already spinning");
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

            // Wheel animates to the EXACT winning index
            _wheelView.SpinToIndex(result.WinningIndex, 2f, () => ResolveSpin(result));
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

            // NON-BOMB → apply reward AFTER animation
            _rewardManager.Add(result.RewardData, result.RewardAmount);

            GameLogger.Log(this, "ResolveSpin", "Reward",
                $"Gained +{result.RewardAmount} ({result.RewardData.itemId})");

            ;

            _uiManager.RefreshRewardsUI();

            float vfxDelay = 0.15f;
            DG.Tweening.DOVirtual.DelayedCall(vfxDelay, () => {
                var winningSlot = _wheelView.GetWinningSlot().transform;
                _uiManager.PlayVFX(result.RewardData.icon, winningSlot, result.RewardData.itemId);
            });

            // delay before next zone
            float postDelay = 0.8f;
            DG.Tweening.DOVirtual.DelayedCall(postDelay, () => {
                // advance to next zone
                _zoneManager.AdvanceZone();
                InitializeCurrentZone();
            });
        }

        // ---------------------------------------------------------
        // ZONE INIT
        // ---------------------------------------------------------
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
    }
}
