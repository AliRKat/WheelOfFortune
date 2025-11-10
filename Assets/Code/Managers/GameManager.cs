using System.Collections.Generic;
using Code.Managers;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private ZoneManager _zoneManager;
    private WheelController _wheelController;
    private RewardManager _rewardBank;

    private void Awake() {
        _zoneManager = new ZoneManager();
        _wheelController = new WheelController();
        _rewardBank = new RewardManager();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            DoSpin();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            int collected = _rewardBank.Collect();
            Debug.Log($"Player left. Collected: {collected}");
        }
    }

    private void DoSpin() {
        var zoneType = _zoneManager.GetCurrentZoneType();
        int baseReward = 10 + (_zoneManager.CurrentZone * 2);

        List<WheelSlice> slices = WheelSliceFactory.Build(zoneType, baseReward);

        var result = _wheelController.Spin(slices);

        if (result.IsBomb) {
            Debug.Log($"BOMB in zone {_zoneManager.CurrentZone}. Rewards lost.");
            _rewardBank.Reset();
            _zoneManager.AdvanceZone();
            return;
        } else {
            _rewardBank.Add(result.RewardAmount);
            Debug.Log($"Zone: {_zoneManager.CurrentZone} | Gained: {result.RewardAmount} | Total: {_rewardBank.Total}");
        }

        _zoneManager.AdvanceZone();
    }
}
