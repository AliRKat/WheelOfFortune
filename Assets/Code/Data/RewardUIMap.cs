using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "UI/Reward UI Map", fileName = "RewardUIMap")]
public class RewardUIMap : ScriptableObject {
    [System.Serializable]
    public struct Entry {
        public string itemId;
        public Sprite icon;
    }

    public List<Entry> entries;

    private Dictionary<string, Sprite> _lookup;

    public Sprite GetIcon(string id) {
        if (_lookup == null) {
            _lookup = new Dictionary<string, Sprite>();
            foreach (var e in entries)
                _lookup[e.itemId] = e.icon;
        }

        return _lookup.TryGetValue(id, out var icon) ? icon : null;
    }
}

