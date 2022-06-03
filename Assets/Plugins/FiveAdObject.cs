using UnityEngine;
using System;
using System.Collections.Generic;

public class FiveAdObject : MonoBehaviour {
  internal static Dictionary<string, FiveAdInterface> objIdMap = new Dictionary<string, FiveAdInterface>();

  void Start () {}
  void Update () {}

  private static FiveAdInterface GetInstance(string objId) {
    FiveAdInterface i = null;
    if (objIdMap.TryGetValue (objId, out i) && i != null) {
      return i;
    }
    return null;
  }
  void OnFiveAdLoad(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdLoad(i); }
      FiveAdLoadListener loadListener = i.GetLoadListener();
      if (loadListener != null) {
        loadListener.OnFiveAdLoad(i);
      }
    }
  }
  void OnFiveAdLoadError(string message) {
    string separator = ":";
    int index = message.LastIndexOf (separator);
    if (index == -1) { return; }
    string objId = message.Substring (0, index);
    int errorCodeInt = Convert.ToInt32 (message.Substring(index+separator.Length));
    FiveAdErrorCode errorCode = (FiveAdErrorCode)errorCodeInt;
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdError (i, errorCode); }
      FiveAdLoadListener loadListener = i.GetLoadListener();
      if (loadListener != null) {
        loadListener.OnFiveAdLoadError(i, errorCode);
      }
    }
  }
  void OnFiveAdViewError(string message) {
    string separator = ":";
    int index = message.LastIndexOf (separator);
    if (index == -1) { return; }
    string objId = message.Substring (0, index);
    int errorCodeInt = Convert.ToInt32 (message.Substring(index+separator.Length));
    FiveAdErrorCode errorCode = (FiveAdErrorCode)errorCodeInt;
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdError (i, errorCode); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdViewError(i, errorCode);
      }
    }
  }
  void OnFiveAdClick(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
#if UNITY_ANDROID
      FiveAd.OpenClickedAdOnUnityMode(i.GetSlotId()); // DO NOT REMOVE THIS.
#endif
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdClick(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdClick(i);
      }
    }
  }
  void OnFiveAdClose(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdClose(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdClose(i);
      }
    }
  }
  void OnFiveAdStart(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdStart(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdStart(i);
      }
    }
  }
  void OnFiveAdPause(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdPause(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdPause(i);
      }
    }
  }
  void OnFiveAdResume(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdResume(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdResume(i);
      }
    }
  }
  void OnFiveAdViewThrough(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdViewThrough(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdViewThrough(i);
      }
    }
  }
  void OnFiveAdReplay(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdReplay(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdReplay(i);
      }
    }
  }
  void OnFiveAdImpressionImage(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdListener l = i.GetListener();
      if (l != null) { l.OnFiveAdImpressionImage(i); }
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdImpression(i);
      }
    }
  }
  void OnFiveAdImpressionVideo(string objId) {
    FiveAdInterface i = GetInstance (objId);
    if (i != null) {
      FiveAdViewEventListener viewEventListener = i.GetViewEventListener();
      if (viewEventListener != null) {
        viewEventListener.OnFiveAdImpression(i);
      }
    }
  }

}
