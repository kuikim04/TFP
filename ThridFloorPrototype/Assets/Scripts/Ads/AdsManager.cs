using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{

#if UNITY_ANDROID
    string appKey = "1fc7ba505";
#elif UNITY_IPHONE
    string appKey = "";
#else
    string appKey = "unexpected_platform";
#endif
    public static AdsManager Instance { get; private set; }

    public static event Action OnRewardedAdCompleted;
    private Action onInterstitialAdClosedCallback; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        IronSource.Agent.validateIntegration();

        IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO);
        IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);
        IronSource.Agent.init(appKey, IronSourceAdUnits.BANNER);

        if (DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            DestroyBanner();
            return;
        }

        LoadBanner();

    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitialized; 

        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

    }

    void SdkInitialized()
    {
        print("SDK is Initialized !!");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region banner 
    public void LoadBanner()
    {
        Debug.Log("Loading banner...");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void DestroyBanner()
    {
        Debug.Log("Destroying banner...");
        IronSource.Agent.destroyBanner();
    }

    /************* Banner AdInfo Delegates *************/

    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner ad loaded successfully.");
        // แสดงแบนเนอร์ (IronSource จะจัดการแสดงอัตโนมัติเมื่อถูกโหลด)
    }

    // เรียกเมื่อแบนเนอร์โหลดไม่สำเร็จ
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        Debug.Log("Banner ad failed to load: " + ironSourceError.getDescription());
    }

    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner ad clicked.");
    }

    // Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner ad screen presented.");
    }

    // Notifies the presented screen has been dismissed
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner ad screen dismissed.");
    }

    // Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner ad left application.");
    }
    #endregion

    #region interstitial

    public void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitialAd(Action onAdClosed = null)
    {
        if (DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            onAdClosed?.Invoke();
            return;
        }

        if (IronSource.Agent.isInterstitialReady())
        {
            onInterstitialAdClosedCallback = onAdClosed;
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("Interstitial not ready!! Loading interstitial...");
            LoadInterstitial();
            StartCoroutine(WaitForInterstitialAndShow(onAdClosed));
        }
    }

    private IEnumerator WaitForInterstitialAndShow(Action onAdClosed)
    {
        while (!IronSource.Agent.isInterstitialReady())
        {
            yield return null;
        }

        onInterstitialAdClosedCallback = onAdClosed;
        IronSource.Agent.showInterstitial();
    }

    /************* Interstitial AdInfo Delegates *************/

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Closed");

        onInterstitialAdClosedCallback?.Invoke();
        onInterstitialAdClosedCallback = null;
    }

    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Ready");
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.Log("Interstitial Ad Load Failed: " + ironSourceError.getDescription());
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Opened");
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Clicked");
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Show Failed: " + ironSourceError.getDescription());
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Show Succeeded");
    }

    #endregion

    #region rewarded

    public void LoadRewarded()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    public void ShowRewarded()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            LoadRewarded();
            Debug.Log("rewarded not ready!!");
        }
    }


    /************* Rewarded Video AdInfo Delegates *************/

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video available.");
    }

    void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("Rewarded video unavailable.");
    }

    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video opened.");
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video closed.");
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log($"User rewarded with: {placement.getRewardName()} amount: {placement.getRewardAmount()}");

        OnRewardedAdCompleted?.Invoke();
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video show failed: " + error.getDescription());
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded video clicked.");
    }

    #endregion
}

