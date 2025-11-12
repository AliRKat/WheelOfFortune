using Code.UI;
using UnityEngine;

namespace Code.Managers {
    public class GameManager : MonoBehaviour {
        [Header("Core References")]
        [SerializeField] private WheelView _wheelView;
        private WheelLogic _wheelLogic;
        private ZoneManager _zoneManager;
        private RewardManager _rewardManager;
        private bool _waitingForChoice = false;
        private bool _initialized = false;

        [Header("UI References - Placeholder")]
        [SerializeField] private ZoneProgressUI _zoneProgressUI;

        private void Awake() {
            _wheelLogic = new WheelLogic();
            _zoneManager = new ZoneManager();
            _rewardManager = new RewardManager();
        }

        private void Start() {
            _zoneProgressUI.Init(_zoneManager);
            InitializeCurrentZone();
        }

        private void Update() {
            // Simulate bomb choice input
            if (_waitingForChoice) {
                if (Input.GetKeyDown(KeyCode.Y)) {
                    Debug.Log("Player paid coins. Continuing from same zone.");
                    _zoneManager.RemoveBombFromCurrentZone();
                    _waitingForChoice = false;
                    InitializeCurrentZone();
                } else if (Input.GetKeyDown(KeyCode.N)) {
                    Debug.Log("Player declined. Resetting progress to Zone 1.");
                    _rewardManager.Reset();
                    _zoneManager.ResetToStart();
                    _waitingForChoice = false;
                    InitializeCurrentZone();
                }
                return;
            }

            // Spin trigger
            if (Input.GetKeyDown(KeyCode.Space)) {
                DoSpin();
            }
        }

        private void InitializeCurrentZone() {
            var zone = _zoneManager.GetCurrentZone();
            if (zone == null) {
                Debug.LogError("No ZoneConfig found for current zone!");
                return;
            }

            _wheelView.ClearAll();
            _wheelView.SetUp(zone);

            Debug.Log($"Initialized Wheel for {zone.zoneId} ({zone.type})");
            _initialized = true;
            _zoneProgressUI.Refresh();
        }

        private void DoSpin() {
            if (!_initialized) return;

            var zone = _zoneManager.GetCurrentZone();
            var result = _wheelLogic.Spin(zone);

            if (result.IsBomb) {
                Debug.Log($"Bomb hit in {zone.zoneId}! Waiting for player decision...");
                _waitingForChoice = true;
                return;
            }

            _rewardManager.Add(result.RewardData, result.RewardAmount);

            string earnings = _rewardManager.GetEarningsString();
            Debug.Log($"Zone: {_zoneManager.CurrentZone} | Type: {zone.type} | " +
                      $"Gained: +{result.RewardAmount} {result.RewardData.category} | " +
                      $"Earnings: {earnings}");

            _zoneManager.AdvanceZone();
            InitializeCurrentZone();
        }
    }
}
