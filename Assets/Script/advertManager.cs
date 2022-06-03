using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;

public class advertManager : MonoBehaviour
{
    public static bool tap_advert;

    public static bool clear_advert;

    public static bool total_advert;

    public BannerView

            bannerView_50,
            bannerView_250;

    public InterstitialAd interstitial;

    public RewardedAd rewardedAd;

    public AdSize adSize;

    // Start is called before the first frame update
    void Start()
    {
        tap_advert = false;
        clear_advert = false;
        total_advert = false;


#if UNITY_EDITOR
        string appId = "unexpected";
#elif UNITY_ANDROID
        string appId = "ca-app-pub-1049023444437572~7504850650";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-1049023444437572~5070259007";
#else
        string appId = "unexpected_platform";
#endif


        MobileAds
            .Initialize(initStatus =>
            {
            });
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RequestBanner_50()
    {
      

#if UNITY_ANDROID
        string adUnitId_50 = "ca-app-pub-1049023444437572/1978898094";
#elif UNITY_IPHONE
        string adUnitId_50 = "ca-app-pub-1049023444437572/1595754715";
#else
        string adUnitId_50 = "unexpected_platform";
#endif

        clear_all_advert();

        // Create a 320x50 banner at the bottom of the screen.

         AdSize adaptedSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerView_50 =
            new BannerView(adUnitId_50, adaptedSize, 0,-1360);
        
        // bannerView_50 =
        //     new BannerView(adUnitId_50, AdSize.Banner, 0, 50);        

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView_50.LoadAd (request);
        print("vanner:" + adUnitId_50);
    }

    public void RequestBanner_250()
    {
        print("banner250");

#if UNITY_ANDROID
        string adUnitId_250 = "ca-app-pub-1049023444437572/4605061436";
#elif UNITY_IPHONE
        string adUnitId_250 ="ca-app-pub-1049023444437572/4221918051"; 
#else
        string adUnitId_250 = "unexpected_platform";
#endif

        clear_all_advert();

        // Create a 320x50 banner at the bottom of the screen.
        //  adSize=new AdSize(350,350);
        bannerView_250 =
            new BannerView(adUnitId_250, AdSize.MediumRectangle, 0, 150);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView_250.LoadAd (request);
    }

    public void popup_banner_250()
    {
        StartCoroutine(moveup());
    }

    IEnumerator moveup()
    {
        float
            timeElapsed = 0,
            duration = 2.0f;

        // transform.eulerAngles = new Vector3(0, y_degree, 0);
        while (timeElapsed < duration)
        {
            bannerView_250
                .SetPosition(0, (int)(150 * (1 + 3f * timeElapsed / duration)));

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void clear_all_advert()
    {
        if (bannerView_250 != null)
        {
            bannerView_250.Destroy();
            print("destroy250");
        }
        if (bannerView_50 != null)
        {
            bannerView_50.Destroy();
            print("destroy250");
        }
        if (interstitial != null)
        {
            interstitial.Destroy();
            print("destroy250");
        }
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            print("destroy250");
        }
    }

    public void true_false_adverting()
    {
        bannerView_250.SetPosition(0, 150);
    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-1049023444437572/7294803953";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-1049023444437572/5343428037";
#else
        string adUnitId = "unexpected_platform";
#endif


        clear_all_advert();

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }

        this.interstitial.OnAdClosed+=this.HandleOnInterAdClosed;       
    }

    public void RequestRewarded()
    {
        string adUnitId;


#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        clear_all_advert();

        rewardedAd = new RewardedAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the rewarded ad with the request.
        rewardedAd.LoadAd (request);

        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }

        rewardedAd.OnAdClosed+=HandleOnAdClosed;
    }

    public void banner_50_close()
    {
        bannerView_50.Destroy();
    }

    public void banner_250_close()
    {
        bannerView_250.Destroy();
    }

    public void interstitial_close()
    {
        interstitial.Destroy();
    }

    public void rewarded_close()
    {
        rewardedAd.Destroy();
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(
        object sender,
        AdFailedToLoadEventArgs args
    )
    {
        MonoBehaviour
            .print("HandleFailedToReceiveAd event received with message: " +
            args.ToString());
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
       
        SendMessage("next_stage_starting");      
    } 

       public void HandleOnInterAdClosed(object sender, EventArgs args)
    {
        print("HandleOnInterAdClosed event received");
       
        RequestBanner_250();      
    } 
}
