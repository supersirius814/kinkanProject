using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementData : IAchievementData {
    private string actionCode = null;
    private string name = null;
    private string instruction = null;
    private string iconUrl = null;
    private string notificationType = null;
    private string achievedDate;
    private int point;
    private bool isCustom;

    public AchievementData(string actionCode, string name, string instruction, string iconUrl, string notificationType, string achievedDate, int point, bool isCustom) {
        this.actionCode = actionCode;
        this.name = name;
        this.instruction = instruction;
        this.iconUrl = iconUrl;
        this.notificationType = notificationType;
        this.achievedDate = achievedDate;
        this.point = point;
        this.isCustom = isCustom;
    }

    public string GetActionCode() {
        return actionCode;
    }

    public string GetName() {
        return name;
    }
    public string GetIconUrl() {
        return iconUrl;
    }

    public string GetInstruction() {
        return instruction;
    }

    public string GetNotificationType() {
        return notificationType;
    }

    public string GetAchievedDate() {
        return achievedDate;
    }

    public int GetPoint() {
        return point;
    }

    public bool GetIsCustom() {
        return isCustom;
    }
}
