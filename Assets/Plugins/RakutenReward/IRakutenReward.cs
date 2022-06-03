using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RakutenRewardStatus {
    Online = 0,
    Offline = 1,
    AppCodeInvalid = 2
}

public interface IRakutenReward {
    void StartSession(string appCode);
    void OpenPortal();
	void OpenSignin();
    void LogAction(string actionCode);
    void Claim(IAchievementData achievementData);
    void SetUIEnabled(bool enabled);
    RakutenRewardStatus GetStatus();
    int GetUnclaimedCount();
    int GetPoint();
    bool IsSignin();
    bool IsOptedOut();
    bool IsUIEnabled();
    string GetVersion();
    void UpdateMissionList();
}
