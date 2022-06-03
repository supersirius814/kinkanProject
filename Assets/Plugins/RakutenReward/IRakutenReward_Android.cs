using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IRakutenReward_Android : IRakutenReward {
    private RakutenReward rakutenRewardGameObject;
    private RakutenRewardEventListener listener;
    private static AndroidJavaObject androidInstance;

    public IRakutenReward_Android(RakutenReward rakutenRewardParent) {
        rakutenRewardGameObject = rakutenRewardParent;

        InitAndroidInstance();
        CreateListenerObject();

        if (rakutenRewardParent.androidAppCode != null) {
            StartSession(null);
        }
    }

    private void CreateListenerObject() {
        listener = rakutenRewardGameObject.gameObject.AddComponent<RakutenRewardEventListener>();

        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            activityObject.CallStatic("setCallbackGameObjectName", rakutenRewardGameObject.gameObject.name);
        }

        listener.SetNativeParent(this);
    }

    public void StartSession(string appCode) {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            if (rakutenRewardGameObject.androidAppCode != null) {
                Debug.Log("# appCode: " + rakutenRewardGameObject.androidAppCode);
                activityObject.Call("startSession", rakutenRewardGameObject.androidAppCode);
            } else if (appCode != null) {
                Debug.Log("# appCode: " + appCode);
                activityObject.Call("startSession", appCode);
            } else {
                Debug.Log("# AppCode is not set, please check.");
            }
        }
    }

    public void OpenPortal() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            activityObject.Call("openPortal");
        }
    }

	public void OpenSignin() {
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("openSignin");
		}
	}

    public void LogAction(string actionCode) {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            activityObject.Call("logAction", actionCode);
        }
    }

    public void Claim(IAchievementData achievementData) {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            activityObject.Call("claim", achievementData.GetActionCode(), achievementData.GetAchievedDate());
        }
    }

    public void SetUIEnabled(bool enabled) {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            activityObject.Call("setUIEnabled", enabled);
        }
    }

    public int GetPoint() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            return activityObject.Call<int>("getPoint");
        }
    }

    public RakutenRewardStatus GetStatus() {
        RakutenRewardStatus status = RakutenRewardStatus.Offline;

        using (AndroidJavaObject statusObject = androidInstance.Call<AndroidJavaObject>("getStatus")) {
            string statusName = statusObject.Call<string>("name");
            if (statusName.Equals("OFFLINE")) {
                status = RakutenRewardStatus.Offline;
            } else if(statusName.Equals("ONLINE")) {
                status = RakutenRewardStatus.Online;
            } else if(statusName.Equals("APPCODEINVALID")) {
                status = RakutenRewardStatus.AppCodeInvalid;
            }
        }

        return status;
    }

    public int GetUnclaimedCount() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            return activityObject.Call<int>("getUnclaimedCount");
        }
    }

    public string GetVersion() {
        return androidInstance.Call<string>("getVersion");
    }

    public bool IsOptedOut() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            return activityObject.Call<bool>("isOptedOut");
        }
    }

    public bool IsSignin() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            return activityObject.Call<bool>("isSignin");
        }
    }

    public bool IsUIEnabled() {
        using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            return activityObject.Call<bool>("isUIEnabled");
        }
    }

    public void UpdateMissionList() {
    }

    public AndroidJavaObject GetCurrentActivity() {
        using (AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            return playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    protected static void InitAndroidInstance() {
        using (AndroidJavaClass rakutenRewardClass = new AndroidJavaClass("jp.co.rakuten.reward.rewardsdk.api.RakutenReward")) {
            androidInstance = rakutenRewardClass.CallStatic<AndroidJavaObject>("getInstance"); 
        }
    }
}
