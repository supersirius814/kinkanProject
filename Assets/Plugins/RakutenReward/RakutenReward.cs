using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using MiniJSON;

public class RakutenReward : MonoBehaviour {
    private IRakutenReward rakutenRewardNative;
    public string iosAppCode;
    public string androidAppCode;
    public Boolean manualLoationPermissionRequest;          // This is for iOS
    private static RakutenReward instance;

    public static RakutenReward GetInstance() {
        if (instance == null) {
            RakutenReward existingInstance = GameObject.FindObjectOfType<RakutenReward>();

            if (existingInstance == null) {
                Debug.LogError("There is no GameObject set up in the scene.  Please add one and set it up as per the Plug-In Documentation.");
                return null;
            }

            existingInstance.SetRakutenRewardNative();
            instance = existingInstance;
        }

        return instance;
    }

    private void Awake() {
        if (instance != null) {
            GameObject.Destroy(this);
            GameObject.Destroy(this.gameObject);
            return;
        }

        SetRakutenRewardNative();
        GameObject.DontDestroyOnLoad(this.gameObject);

        instance = this;
    }

    private void SetRakutenRewardNative() {
        if (rakutenRewardNative != null) {
            return;
        }

        #if UNITY_IOS
            rakutenRewardNative = new IRakutenReward_iOS(this, manualLoationPermissionRequest);
        #elif UNITY_ANDROID
            rakutenRewardNative = new IRakutenReward_Android(this);
        #endif
    }

    public void OpenPortal() {
        rakutenRewardNative.OpenPortal();
    }

	public void OpenSignin() {
		rakutenRewardNative.OpenSignin ();
	}

    public void LogAction(string actionCode) {
        rakutenRewardNative.LogAction(actionCode);
    }

    public void Claim(IAchievementData achievementData) {
        rakutenRewardNative.Claim(achievementData);
    }

    public void SetUIEnabled(bool enabled) {
        rakutenRewardNative.SetUIEnabled(enabled);
    }

    public RakutenRewardStatus GetStatus() {
        return rakutenRewardNative.GetStatus();
    }

    public int GetUnclaimedCount() {
        return rakutenRewardNative.GetUnclaimedCount();
    }

    public int GetPoint() {
        return rakutenRewardNative.GetPoint();
    }

    public bool IsSignin() {
        return rakutenRewardNative.IsSignin();
    }

    public bool IsOptedOut() {
        return rakutenRewardNative.IsOptedOut();
    }

    public bool IsUIEnabled() {
        return rakutenRewardNative.IsUIEnabled();
    }

    public string GetVersion() {
        return rakutenRewardNative.GetVersion();
    }

    // Method for sample app
    public void UpdateMissionList() {
        rakutenRewardNative.UpdateMissionList();
    }

    public static IAchievementData GetAchievementData(string jsonString) {
        Dictionary<string, object> achievementDict = Json.Deserialize(jsonString) as Dictionary<string,object>;

        string actionCode = (string)achievementDict["action"];
        string name = (string)achievementDict["name"];
        string instruction = (string)achievementDict["instruction"];
        string iconUrl = (string)achievementDict["iconUrl"];
        string notificationType = (string)achievementDict["notificationType"];
        string achievedDate = (string)achievementDict["achievedDate"];
        int point = int.Parse(achievementDict["point"].ToString());
        bool isCustom = (bool)achievementDict["isCustom"];

        IAchievementData achievementData = new AchievementData(actionCode, name, instruction, iconUrl, notificationType, achievedDate, point, isCustom);

        return achievementData;
    }

    public IRakutenReward RakutenRewardNative {
        get {
            return rakutenRewardNative;
        }
    }
}
