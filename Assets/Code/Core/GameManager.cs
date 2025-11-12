using Code.Managers;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private ZoneManager _zoneManager;
    private WheelLogic _wheelController;
    private RewardManager _rewardManager;
    private bool _waitingForChoice = false;

    private void Awake() {
        _zoneManager = new ZoneManager();
        _wheelController = new WheelLogic();
        _rewardManager = new RewardManager();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            DoSpin();

        if (Input.GetKeyDown(KeyCode.L)) {
            _rewardManager.Collect();
        }

        if (_waitingForChoice) {
            if (Input.GetKeyDown(KeyCode.Y)) {
                Debug.Log("Paid coins. Continuing from current zone.");
                _zoneManager.RemoveBombFromCurrentZone();
                _waitingForChoice = false;
            } else if (Input.GetKeyDown(KeyCode.N)) {
                Debug.Log("Declined. Progress reset to Zone 1.");
                _rewardManager.Reset();
                _zoneManager.ResetToStart();
                _waitingForChoice = false;
            }
            return; // skip other input while waiting
        }
    }

    private void DoSpin() {
        ZoneConfig zone = _zoneManager.GetCurrentZone();
        if (zone == null) {
            Debug.LogError("Zone data not found.");
            return;
        }

        var result = _wheelController.Spin(zone);

        if (result.IsBomb) {
            Debug.Log($"💣 BOMB in zone {_zoneManager.CurrentZone}. Rewards lost.\n" +
                      "Press Y to pay coins and retry, N to decline and reset.");

            // Temporarily stop advancing zone until choice is made
            _waitingForChoice = true;
            return;
        }


        _rewardManager.Add(result.RewardData, result.RewardAmount);

        string earnings = _rewardManager.GetEarningsString();

        Debug.Log(
            $"Zone {_zoneManager.CurrentZone} | Type: {zone.type} | " +
            $"Gained: +{result.RewardAmount} {result.RewardData.category} | " +
            $"Earnings: {earnings}"
        );

        _zoneManager.AdvanceZone();
    }

}
