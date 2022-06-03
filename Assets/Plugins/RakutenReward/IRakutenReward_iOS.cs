using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_IOS

public class IRakutenReward_iOS : IRakutenReward {
    private RakutenReward rakutenRewardGameObject;
    private RakutenRewardEventListener listener;

    [DllImport ("__Internal")]
    protected static extern void RWSetCallbackGameObjectName(string gameObjectName);

    public IRakutenReward_iOS(RakutenReward rakutenRewardParent, bool manualLoationPermissionRequest) {
        rakutenRewardGameObject = rakutenRewardParent;
        SetManualLoationPermissionRequest(manualLoationPermissionRequest);
        if (rakutenRewardParent.iosAppCode != null) {
            StartSession(null);
        }

        CreateListenerObject();
    }

    private void CreateListenerObject()	{
        listener = rakutenRewardGameObject.gameObject.AddComponent<RakutenRewardEventListener>();
        listener.SetNativeParent(this);

        RWSetCallbackGameObjectName(rakutenRewardGameObject.gameObject.name);
        Debug.Log("Setting Callback Object: " + rakutenRewardGameObject.gameObject.name);
    }

    [DllImport("__Internal")]
    private static extern void RWStartSession(string appCode);
    public void StartSession(string appCode) {
        if (rakutenRewardGameObject.iosAppCode != null) {
            Debug.Log("# appCode: " + rakutenRewardGameObject.iosAppCode);
            RWStartSession(rakutenRewardGameObject.iosAppCode);
        } else if (appCode != null) {
            Debug.Log("# appCode: " + appCode);
            RWStartSession(appCode);
        } else {
            Debug.Log("# AppCode is not set, please check.");
        }
    }

    [DllImport("__Internal")]
    private static extern void RWOpenPortal();
    public void OpenPortal() {
        RWOpenPortal();
    }

	[DllImport("__Internal")]
	private static extern void RWOpenSignin();
	public void OpenSignin() {
		RWOpenSignin();
	}

    [DllImport("__Internal")]
    private static extern void RWLogAction(string actionCode);
    public void LogAction(string actionCode) {
        RWLogAction(actionCode);
    }

    [DllImport("__Internal")]
    private static extern void RWClaim(string actionCode, string achievedDate);
    public void Claim(IAchievementData achievementData) {
        RWClaim(achievementData.GetActionCode(), achievementData.GetAchievedDate());
    }

    [DllImport("__Internal")]
    private static extern void RWSetUIEnabled(bool enabled);
    public void SetUIEnabled(bool enabled) {
        RWSetUIEnabled(enabled);
    }

    [DllImport("__Internal")]
    private static extern int RWGetStatus();
    public RakutenRewardStatus GetStatus() {
        return (RakutenRewardStatus)RWGetStatus();
    }

    [DllImport("__Internal")]
    private static extern int RWGetUnclaimedCount();
    public int GetUnclaimedCount() {
        return RWGetUnclaimedCount();
    }

    [DllImport("__Internal")]
    private static extern int RWGetPoint();
    public int GetPoint() {
        return RWGetPoint();
    }

    [DllImport("__Internal")]
    private static extern bool RWIsSignin();
    public bool IsSignin() {
        return RWIsSignin();
    }

    [DllImport("__Internal")]
    private static extern bool RWIsOptedOut();
    public bool IsOptedOut() {
        return RWIsOptedOut();
    }

    [DllImport("__Internal")]
    private static extern bool RWIsUIEnabled();
    public bool IsUIEnabled() {
        return RWIsUIEnabled();
    }

    [DllImport("__Internal")]
    private static extern string RWGetVersion();
    public string GetVersion() {
        return RWGetVersion();
    }

    [DllImport("__Internal")]
    private static extern void RWUpdateMissionList();
    public void UpdateMissionList() {
        RWUpdateMissionList();
    }

    [DllImport("__Internal")]
    private static extern void RWSetManualLoationPermissionRequest(bool enabled);
    public void SetManualLoationPermissionRequest(bool enabled) {
        RWSetManualLoationPermissionRequest(enabled);
    }
}

#endif