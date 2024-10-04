using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    public InterstitialAds InterstitialAds;
    public BannerAds BannerAds;
    public RewardedAdsButton RewardedAdsButton;

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
        RewardedAdsButton.LoadAd();

        if (DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            BannerAds.HideBannerAd();

            Debug.Log("Ads disabled for this session");
            return;
        }

        BannerAds.LoadBanner();
        InterstitialAds.LoadInterstitialAd();

        StartCoroutine(DisplayBannerWithDelay());
    }

    public void ShowInterstitialAd(System.Action onAdClosed)
    {
        if (DataCenter.Instance.GetPlayerData().AdsRemove)
        {
            onAdClosed?.Invoke();
            return;
        }

        if (!InterstitialAds.IsAdReady())
        {
            InterstitialAds.LoadInterstitialAd();
            StartCoroutine(WaitForAdLoadAndShow(onAdClosed));
            return;
        }

        InterstitialAds.ShowInterstitialAd(onAdClosed);
    }
    private IEnumerator WaitForAdLoadAndShow(System.Action onAdClosed)
    {
        while (!InterstitialAds.IsAdReady())
        {
            yield return null;
        }

        InterstitialAds.ShowInterstitialAd(onAdClosed);
    }

    IEnumerator DisplayBannerWithDelay()
    {
        yield return new WaitForSeconds(1f);
        AdsManager.Instance.BannerAds.ShowBannerAd();

    }
}

