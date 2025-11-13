using System.Collections.Generic;
using Code.UI;
using UnityEngine;

namespace Code.Managers {
    public class UIManager : MonoBehaviour {
        [Header("UI Elements")]
        [SerializeField] private RewardsUI _rewardsUI;
        [SerializeField] private ZoneProgressUI _zoneProgressUI;

        [Header("Mapping")]
        [SerializeField] private RewardUIMap _rewardMap;

        private RewardManager _rewardManager;
        private ZoneManager _zoneManager;

        public void Init(RewardManager rewardManager, ZoneManager zoneManager) {
            _rewardManager = rewardManager;
            _zoneManager = zoneManager;

            _zoneProgressUI.Init(zoneManager);
        }

        // Called by GameManager whenever rewards change
        public void RefreshRewardsUI() {
            var totals = _rewardManager.GetTotals();
            var dtoList = BuildRewardDTOs(totals);

            _rewardsUI.Populate(dtoList);
        }

        // Called by GameManager whenever zone changes
        public void RefreshZoneUI() {
            _zoneProgressUI.Refresh();
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
    }
}
