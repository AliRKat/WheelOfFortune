using Code.UI;
using UnityEngine;
using Code.Core;

namespace Code.Managers {
    public class GameManager : MonoBehaviour {
        [Header("Core References")]
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private UIManager _uiManager;

        private WheelLogic _wheelLogic;
        private ZoneManager _zoneManager;
        private RewardManager _rewardManager;

        private bool _waitingForChoice = false;
        private bool _initialized = false;

        private void Awake() {
            _wheelLogic = new WheelLogic();
            _zoneManager = new ZoneManager();
            _rewardManager = new RewardManager();

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

            if (Input.GetKeyDown(KeyCode.Space))
                DoSpin();
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

            GameLogger.Log(this, "InitializeCurrentZone", "Zone",
                $"Initializing zone {zone.zoneId} ({zone.type})");

            _wheelView.ClearAll();
            _wheelView.SetUp(zone);

            _initialized = true;

            _uiManager.RefreshZoneUI();
            GameLogger.Log(this, "InitializeCurrentZone", "UI",
                "Zone UI refreshed");

            _uiManager.RefreshRewardsUI();
            GameLogger.Log(this, "InitializeCurrentZone", "UI",
                "Rewards UI refreshed");
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
                return;
            }

            // Reward
            _rewardManager.Add(result.RewardData, result.RewardAmount);

            GameLogger.Log(this, "DoSpin", "Reward",
                $"Gained +{result.RewardAmount} ({result.RewardData.itemId})");

            // UI
            _uiManager.RefreshRewardsUI();
            GameLogger.Log(this, "DoSpin", "UI",
                "Rewards UI updated");

            // Next zone
            _zoneManager.AdvanceZone();
            GameLogger.Log(this, "DoSpin", "Zone",
                "Advancing to next zone");

            InitializeCurrentZone();
        }
    }
}
