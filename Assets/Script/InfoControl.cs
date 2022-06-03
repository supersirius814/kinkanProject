using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoControl : MonoBehaviour
{
    // Start is called before the first frame update
    string info_url;
    void Start()
    {
        info_url="https://sp.i-appli.net/media/policy.html";        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
#elif UNITY_ANDROID
#elif UNITY_IOS
    [DllImport("__Internal")]
    static extern void launchURL(string url);
#endif

    void LaunchURL(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var intentBuilder = new AndroidJavaObject("android.support.customtabs.CustomTabsIntent$Builder"))
        using (var intent = intentBuilder.Call<AndroidJavaObject>("build"))
        using (var uriClass = new AndroidJavaClass("android.net.Uri"))
        using (var uri = uriClass.CallStatic<AndroidJavaObject>("parse", url))
        intent.Call("launchUrl", activity, uri);
#elif UNITY_IOS
        launchURL(url);
#endif
    }
}
