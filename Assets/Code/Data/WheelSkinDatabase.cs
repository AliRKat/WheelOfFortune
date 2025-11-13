using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Wheel Skin Database", fileName = "WheelSkinDatabase")]
public class WheelSkinDatabase : ScriptableObject {
    public WheelSkinData bronzeSkin;
    public WheelSkinData silverSkin;
    public WheelSkinData goldSkin;

    public WheelSkinData GetSkin(ZoneType type) {
        switch (type) {
            case ZoneType.Safe:
                return silverSkin;
            case ZoneType.Super:
                return goldSkin;
            default:
                return bronzeSkin;
        }
    }
}

[Serializable]
public class WheelSkinData {
    public Sprite baseWheel;
    public Sprite indicator;
}