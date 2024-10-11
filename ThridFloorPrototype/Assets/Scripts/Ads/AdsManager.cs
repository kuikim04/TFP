using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    public static event Action OnRewardedAdCompleted;
    private Action onInterstitialAdClosedCallback;

    int adsCount = 0;

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
        adsCount = 0;

#if UNITY_ANDROID
        string appKey = "1fc7ba505";
#elif UNITY_IPHONE
        string appKey = "";
#else
        string appKey = "unexpected_platform";
#endif

        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);

        if (!DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            LoadBanner();
            LoadInterstitial(); 
        }
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitialized;
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

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

    void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void InCreaseAds()
    {
        adsCount++;
    }
    public void ResetAds()
    {
        adsCount = 0;
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
        Debug.Log("Load Interstitial!!");
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitialAd(Action onAdClosed = null)
    {
        if (adsCount != 4)
        {
            Debug.Log("Ads not shown, adsCount: " + adsCount);
            onAdClosed?.Invoke();
            return;
        }

        if (DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            Debug.Log("You Have Remove Ads!!");
            onAdClosed?.Invoke();
            return;
        }

        if (IronSource.Agent.isInterstitialReady())
        {
            Debug.Log("Interstitial Ready!!");
            StartCoroutine(WaitForInterstitialAndShow(onAdClosed));
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

        Debug.Log("Show Ads!!");
        onInterstitialAdClosedCallback = onAdClosed;
        IronSource.Agent.showInterstitial();
    }

    /************* Interstitial AdInfo Delegates *************/
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial Ad Closed");
        onInterstitialAdClosedCallback?.Invoke();
        onInterstitialAdClosedCallback = null;

        if (adsCount == 4)
        {
            Debug.Log("Ads shown, resetting adsCount.");
            ResetAds();
        }

        LoadInterstitial();
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

