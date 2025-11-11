using Code.Managers;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private ZoneManager _zoneManager;
    private WheelController _wheelController;
    private RewardManager _rewardManager;

    private void Awake() {
        _zoneManager = new ZoneManager();
        _wheelController = new WheelController();
        _rewardManager = new RewardManager();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            DoSpin();

        if (Input.GetKeyDown(KeyCode.L)) {
            _rewardManager.Collect();
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
            Debug.Log($"💣 BOMB in zone {_zoneManager.CurrentZone}. Rewards lost.");
            _rewardManager.Reset();
            _zoneManager.AdvanceZone();
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
