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

        // will be set properly when animation is added
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

        private void Update() {
            HandleBombChoice();
        }

        public void RequestSpin() {
            if (!_initialized) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Spin ignored: not initialized");
                return;
            }

            if (_waitingForChoice) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Spin ignored: awaiting bomb decision");
                return;
            }

            if (_isSpinning) {
                GameLogger.Warn(this, "RequestSpin", "Guard", "Spin ignored: wheel already spinning");
                return;
            }

            // Hide exit button while spinning
            _isSpinning = true;
            _uiManager.UpdateExitButtonVisibility(isSpinning: true);

            DoSpin();
        }

        public void RequestExit() {
            if (!_initialized) return;
            if (_waitingForChoice) return;
            if (_isSpinning) return;

            var zone = _zoneManager.GetCurrentZone();

            if (zone.type == ZoneType.Normal) {
                GameLogger.Warn(this, "RequestExit", "Guard",
                    "Exit ignored: allowed only in Safe/Super zones");
                return;
            }

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

        private void HandleBombChoice() {
            if (!_waitingForChoice)
                return;

            if (Input.GetKeyDown(KeyCode.Y)) {
                GameLogger.Log(this, "HandleBombChoice", "Bomb",
                    "Player chose to PAY and continue same zone");

                _zoneManager.RemoveBombFromCurrentZone();
                _waitingForChoice = false;

                InitializeCurrentZone();
            } else if (Input.GetKeyDown(KeyCode.N)) {
                GameLogger.Log(this, "HandleBombChoice", "Bomb",
                    "Player chose NOT to pay → resetting progress");

                _rewardManager.Reset();
                _zoneManager.ResetToStart();
                _waitingForChoice = false;

                InitializeCurrentZone();
            }
        }

        private void InitializeCurrentZone() {
            var zone = _zoneManager.GetCurrentZone();

            if (zone == null) {
                GameLogger.Error(this, "InitializeCurrentZone", "ZoneNull",
                    "ZoneConfig NOT FOUND!");
                return;
            }

            // wheel not spinning on zone init
            _isSpinning = false;
            _uiManager.UpdateExitButtonVisibility(isSpinning: false);

            GameLogger.Log(this, "InitializeCurrentZone", "Zone",
                $"Initializing zone {zone.zoneId} ({zone.type})");

            _wheelView.ClearAll();

            var wheelInitData = new WheelViewInitData();
            var skin = _wheelSkinDatabase.GetSkin(zone.type);

            wheelInitData.zoneConfig = zone;
            wheelInitData.wheelSkinData = skin;

            _wheelView.SetUp(wheelInitData);
            _initialized = true;

            _uiManager.RefreshZoneUI();
            GameLogger.Log(this, "InitializeCurrentZone", "UI", "Zone UI refreshed");

            _uiManager.RefreshRewardsUI();
            GameLogger.Log(this, "InitializeCurrentZone", "UI", "Rewards UI refreshed");
        }

        private void DoSpin() {
            if (!_initialized)
                return;

            var zone = _zoneManager.GetCurrentZone();

            GameLogger.Log(this, "DoSpin", "SpinStart",
                $"Spinning wheel at zone {zone.zoneId}");

            var result = _wheelLogic.Spin(zone);

            if (result.IsBomb) {
                GameLogger.Warn(this, "DoSpin", "Bomb",
                    "Bomb hit → waiting for player decision");

                _waitingForChoice = true;
                GameEvents.BombHit?.Invoke();
                return;
            }

            _rewardManager.Add(result.RewardData, result.RewardAmount);

            GameLogger.Log(this, "DoSpin", "Reward",
                $"Gained +{result.RewardAmount} ({result.RewardData.itemId})");

            _uiManager.RefreshRewardsUI();
            GameLogger.Log(this, "DoSpin", "UI", "Rewards UI updated");

            _zoneManager.AdvanceZone();
            GameLogger.Log(this, "DoSpin", "Zone", "Advancing to next zone");

            // end spin animation — so exit button can reappear if allowed
            _isSpinning = false;

            InitializeCurrentZone();
        }
    }
}
