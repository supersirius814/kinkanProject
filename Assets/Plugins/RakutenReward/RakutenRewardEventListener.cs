using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class RakutenRewardEventListener : MonoBehaviour {
    public static event Action<RakutenRewardStatus> NotifyStateChanged;
    public static event Action<IDictionary<string, object>> NotifyUserInfoChanged;
    public static event Action<IAchievementData> NotifyAchievement;

    private IRakutenReward nativeParent;

    public void SetNativeParent(IRakutenReward nativeParent) {
        this.nativeParent = nativeParent;
    }

    private void _RakutenReward_didUpdateUser(string message) {
        Debug.Log("# didUpdateUser: " + message);
        
        Dictionary<string, object> userInfo = Json.Deserialize(message) as Dictionary<string, object>;

        if (NotifyUserInfoChanged != null) {
            NotifyUserInfoChanged(userInfo);
        }
    }
    
    public void _RakutenReward_didSDKStateChange(string message) {
        Debug.Log("# didSDKStateChange: " + message);
        RakutenRewardStatus status = (RakutenRewardStatus)int.Parse(message);

        if (NotifyStateChanged != null) {
            Debug.Log("# call NotifyStateChanged: ");
            NotifyStateChanged(status);
        } else {
            Debug.Log("# NotifyStateChanged is null");
        }
    }

    public void _RakutenReward_didUpdateUnclaimedAchievement(string message) {
        Debug.Log("# didUpdateUnclaimedAchievement: " + message);
        IAchievementData achievementData = RakutenReward.GetAchievementData(message);

        if (NotifyAchievement != null) {
            Debug.Log("# call NotifyAchievement.");
            NotifyAchievement(achievementData);
        } else {
            Debug.Log("# NotifyAchievement is null");
        }
    }
}
