using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAchievementData {
    string GetActionCode();
    string GetName();
    string GetInstruction();
    string GetIconUrl();
    string GetNotificationType();
    string GetAchievedDate();
    int GetPoint();
    bool GetIsCustom();
}
