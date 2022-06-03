using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public enum FiveAdFormat {
  INTERSTITIAL_LANDSCAPE = 1,
  INTERSTITIAL_PORTRAIT = 2,
  IN_FEED = 3,
  W320_H180 = 5,
  CUSTOM_LAYOUT = 7,
  VIDEO_REWARD = 8,
}

public enum FiveAdState {
  NOT_LOADED = 1,
  LOADING = 2,
  LOADED = 3,
  SHOWING = 4,
  CLOSED = 5,
  ERROR = 6,
}

public class FiveAdConfig {
  public readonly string appId;

  [Obsolete("This field is deprecated. You don't have to set this field.")]
  public HashSet<FiveAdFormat> formats = new HashSet<FiveAdFormat>();
  public bool isTest = true;

  public bool? soundEnabled = null;

  public FiveAdConfig(string appId) {
    this.appId = appId;
  }

  public void EnableSoundByDefault(bool enabled) {
    this.soundEnabled = enabled;
  }
}

public static class FiveAd {
#if UNITY_IPHONE
  [DllImport("__Internal")]
  private static extern void _FADSettings_initialize(
    string appId,
    bool isTest,
    string gameObject,
    int unityScreenWidth,
    int unityScreenHeight,
    bool enableSoundByDefaultCalled,
    bool soundEnabled
  );
  [DllImport("__Internal")]
  private static extern void _FADSettings_enableSound(bool enabled);
  [DllImport("__Internal")]
  private static extern bool _FADSettings_isSoundEnabled();
#elif UNITY_ANDROID
  private static readonly AndroidJavaClass java = new AndroidJavaClass ("com.five_corp.ad.unity.FiveAdUnityPlugin");
#endif
  public static void Initialize(FiveAdConfig config, GameObject gameObject) {
#if UNITY_IPHONE
    _FADSettings_initialize(
      config.appId,
      config.isTest,
      gameObject.name,
      Screen.width,
      Screen.height,
      config.soundEnabled != null,
      config.soundEnabled ?? false
    );
#elif UNITY_ANDROID
    java.CallStatic (
      "initialize",
      config.appId,
      config.isTest,
      gameObject.name,
      config.soundEnabled != null,
      config.soundEnabled ?? false
    );
#endif
  }

  [Obsolete("Use `FiveAdConfig.EnableSoundByDefault(bool)` instead.")]
  public static void EnableSound(bool enabled) {
#if UNITY_IPHONE
    _FADSettings_enableSound (enabled);
#elif UNITY_ANDROID
    java.CallStatic ("enableSound", enabled);
#endif
  }

  public static bool IsSoundEnabled() {
#if UNITY_IPHONE
    return _FADSettings_isSoundEnabled ();
#elif UNITY_ANDROID
    return java.CallStatic<bool> ("isSoundEnabled");
#else
    return false;
#endif
  }

  internal static void OpenClickedAdOnUnityMode(string slotId) {
#if UNITY_ANDROID
    java.CallStatic ("openClickedAdOnUnityMode", slotId);
#else
    // not support unity mode.
#endif
  }
}

public enum FiveAdErrorCode {
  NETWORK_ERROR = 1,
  NO_AD = 2,

  [Obsolete("No more used this enum.")]
  NO_FILL = 3,
  BAD_APP_ID = 4,
  STORAGE_ERROR = 5,
  INTERNAL_ERROR = 6,

  [Obsolete("No more used this enum.")]
  UNSUPPORTED_OS_VERSION = 7,
  INVALID_STATE = 8,
  BAD_SLOT_ID = 9,
  SUPPRESSED = 10,
  PLAYER_ERROR = 12,
};

public interface FiveAdListener {
  void OnFiveAdLoad(FiveAdInterface f);
  void OnFiveAdError(FiveAdInterface f, FiveAdErrorCode errorCode);
  void OnFiveAdClick(FiveAdInterface f);
  void OnFiveAdClose(FiveAdInterface f);
  void OnFiveAdStart(FiveAdInterface f);
  void OnFiveAdPause(FiveAdInterface f);
  void OnFiveAdResume(FiveAdInterface f);
  void OnFiveAdViewThrough(FiveAdInterface f);
  void OnFiveAdReplay(FiveAdInterface f);
  void OnFiveAdImpressionImage(FiveAdInterface f);
}

public interface FiveAdLoadListener {
  void OnFiveAdLoad(FiveAdInterface f);
  void OnFiveAdLoadError(FiveAdInterface f, FiveAdErrorCode errorCode);
}

public interface FiveAdViewEventListener {
  void OnFiveAdViewError(FiveAdInterface f, FiveAdErrorCode errorCode);
  void OnFiveAdClick(FiveAdInterface f);
  void OnFiveAdClose(FiveAdInterface f);
  void OnFiveAdStart(FiveAdInterface f);
  void OnFiveAdPause(FiveAdInterface f);
  void OnFiveAdResume(FiveAdInterface f);
  void OnFiveAdViewThrough(FiveAdInterface f);
  void OnFiveAdReplay(FiveAdInterface f);
  void OnFiveAdImpression(FiveAdInterface f);
}

public interface FiveAdInterface {
  FiveAdListener GetListener();
  FiveAdLoadListener GetLoadListener();
  FiveAdViewEventListener GetViewEventListener();

  [Obsolete("Use `FiveAdLoadListener` and `FiveAdViewEventListener` instead.")]
  void SetListener(FiveAdListener listener);
  void SetLoadListener(FiveAdLoadListener loadListener);
  void SetViewEventListener(FiveAdViewEventListener viewEventListener);
  string GetSlotId();
  bool IsSoundEnabled();
  void EnableSound(bool enabled);

  FiveAdState GetState();

  [ObsoleteAttribute("LoadAd is obsolete.  Use LoadAdAsync instead.", false)]
  void LoadAd();
}

public abstract class FiveAdBase : FiveAdInterface, IDisposable {
#if UNITY_IPHONE
  [DllImport("__Internal")]
  protected static extern void _FAD_Interface_remove(string objId);
  [DllImport("__Internal")]
  protected static extern void _FAD_Interface_enableSound(string objId, bool enabled);
  [DllImport("__Internal")]
  protected static extern bool _FAD_Interface_isSoundEnaled(string objId);
  [DllImport("__Internal")]
  protected static extern int  _FAD_Interface_getState(string objId);
  [DllImport("__Internal")]
  protected static extern void _FAD_Interface_loadAd(string objId);
#elif UNITY_ANDROID
  protected static readonly AndroidJavaClass java = new AndroidJavaClass ("com.five_corp.ad.unity.FiveAdUnityInterface");
#endif
  protected bool disposed;
  protected readonly string slotId;
  protected readonly string objId;
  protected FiveAdListener listener;
  protected FiveAdLoadListener loadListener;
  protected FiveAdViewEventListener viewEventListener;

  public FiveAdBase(string slotId) {
    disposed = false;
    objId = Guid.NewGuid().ToString();
    this.slotId = slotId;
    FiveAdObject.objIdMap.Add (objId, this);
  }
  ~FiveAdBase() {
    Dispose (false);
  }
  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
  protected virtual void Dispose(bool disposing) {
    if (disposed)
      return;

    if (disposing) {
      FiveAdObject.objIdMap.Remove (objId);
#if UNITY_IPHONE
      _FAD_Interface_remove (objId);
#elif UNITY_ANDROID
      java.CallStatic ("remove", objId);
#endif
    }

    disposed = true;
  }

  public FiveAdListener GetListener() {
    return listener;
  }
  public FiveAdLoadListener GetLoadListener() {
    return loadListener;
  }
  public FiveAdViewEventListener GetViewEventListener() {
    return viewEventListener;
  }
  public void SetListener(FiveAdListener listener) {
    this.listener = listener;
  }
  public void SetLoadListener(FiveAdLoadListener loadListener) {
    this.loadListener = loadListener;
  }
  public void SetViewEventListener(FiveAdViewEventListener viewEventListener) {
    this.viewEventListener = viewEventListener;
  }
  public string GetSlotId() {
    return slotId;
  }
  public bool IsSoundEnabled() {
#if UNITY_IPHONE
    return _FAD_Interface_isSoundEnaled (objId);
#elif UNITY_ANDROID
    return java.CallStatic<bool> ("isSoundEnabled", objId);
#else
    return false;
#endif
  }
  public void EnableSound(bool enabled) {
#if UNITY_IPHONE
    _FAD_Interface_enableSound (objId, enabled);
#elif UNITY_ANDROID
    java.CallStatic ("enableSound", objId, enabled);
#endif
  }
  public FiveAdState GetState() {
    int state = 8;
#if UNITY_IPHONE
    state = _FAD_Interface_getState (objId);
#elif UNITY_ANDROID
    state = java.CallStatic<int> ("getState", objId);
#endif
    return (FiveAdState)state;
  }

  [ObsoleteAttribute("LoadAd is obsolete.  Use LoadAdAsync instead.", false)]
  public void LoadAd() {
#if UNITY_IPHONE
    _FAD_Interface_loadAd (objId);
#elif UNITY_ANDROID
    java.CallStatic ("loadAd", objId);
#endif
  }
}

public class FiveAdInterstitial : FiveAdBase {
#if UNITY_IPHONE
  [DllImport("__Internal")]
  private static extern void _FAD_Interstitial_put(string objId, string slotId);
  [DllImport("__Internal")]
  private static extern bool _FAD_Interstitial_show(string objId);
  [DllImport("__Internal")]
  protected static extern void _FAD_Interstitial_loadAdAsync(string objId);
#endif
  public FiveAdInterstitial(string slotId) : base(slotId) {
#if UNITY_IPHONE
    _FAD_Interstitial_put (objId, slotId);
#elif UNITY_ANDROID
    java.CallStatic ("putInterstitial", objId, slotId);
#endif
  }
  public bool Show() {
#if UNITY_IPHONE
    return _FAD_Interstitial_show (objId);
#elif UNITY_ANDROID
    return java.CallStatic<bool> ("showInterstitial", objId);
#else
    return false;
#endif
  }

  public void LoadAdAsync() {
#if UNITY_IPHONE
    _FAD_Interstitial_loadAdAsync (objId);
#elif UNITY_ANDROID
    java.CallStatic ("loadAdAsyncInterstitial", objId);
#endif
  }
}

public class FiveAdCustomLayout : FiveAdBase {
#if UNITY_IPHONE
  [DllImport("__Internal")]
  private static extern void _FAD_CustomLayout_put(string objId, string slotId, int width);
  [DllImport("__Internal")]
  private static extern bool _FAD_CustomLayout_add(string objId, int x, int y);
  [DllImport("__Internal")]
  private static extern int _FAD_CustomLayout_width(string objId);
  [DllImport("__Internal")]
  private static extern int _FAD_CustomLayout_height(string objId);
  [DllImport("__Internal")]
  private static extern bool _FAD_CustomLayout_remove(string objId);
  [DllImport("__Internal")]
  protected static extern void _FAD_CustomLayout_loadAdAsync(string objId);
#endif
  public FiveAdCustomLayout(string slotId, int width) : base(slotId) {
#if UNITY_IPHONE
    _FAD_CustomLayout_put (objId, slotId, width);
#elif UNITY_ANDROID
    java.CallStatic ("putCustomLayout", objId, slotId, width);
#endif
  }
  public void Add(int x, int y) {
#if UNITY_IPHONE
    _FAD_CustomLayout_add(objId, x, y);
#elif UNITY_ANDROID
    java.CallStatic ("addCustomLayout", objId, x, y);
#endif
  }
  public int GetWidth() {
#if UNITY_IPHONE
    return _FAD_CustomLayout_width(objId);
#elif UNITY_ANDROID
    return java.CallStatic<int> ("getWidth", objId);
#else
    return -1;
#endif
  }
  public int GetHeight() {
#if UNITY_IPHONE
    return _FAD_CustomLayout_height(objId);
#elif UNITY_ANDROID
    return java.CallStatic<int> ("getHeight", objId);
#else
    return -1;
#endif
  }
  public void Remove() {
#if UNITY_IPHONE
    _FAD_CustomLayout_remove(objId);
#elif UNITY_ANDROID
    java.CallStatic ("removeCustomLayout", objId);
#endif
  }

  public void LoadAdAsync() {
#if UNITY_IPHONE
    _FAD_CustomLayout_loadAdAsync (objId);
#elif UNITY_ANDROID
    java.CallStatic ("loadAdAsyncCustomLayout", objId);
#endif
  }
}

public class FiveAdVideoReward : FiveAdBase {
#if UNITY_IPHONE
  [DllImport("__Internal")]
  private static extern void _FAD_VideoReward_put(string objId, string slotId);
  [DllImport("__Internal")]
  private static extern bool _FAD_VideoReward_show(string objId);
  [DllImport("__Internal")]
  protected static extern void _FAD_VideoReward_loadAdAsync(string objId);
#endif
  public FiveAdVideoReward(string slotId) : base(slotId) {
#if UNITY_IPHONE
    _FAD_VideoReward_put (objId, slotId);
#elif UNITY_ANDROID
    java.CallStatic ("putVideoReward", objId, slotId);
#endif
  }
  public bool Show() {
#if UNITY_IPHONE
    return _FAD_VideoReward_show (objId);
#elif UNITY_ANDROID
    return java.CallStatic<bool> ("showVideoReward", objId);
#else
    return false;
#endif
  }

  public void LoadAdAsync() {
#if UNITY_IPHONE
    _FAD_VideoReward_loadAdAsync (objId);
#elif UNITY_ANDROID
    java.CallStatic ("loadAdAsyncVideoReward", objId);
#endif
  }
}
