using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour , IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOSAdUnitId = "Interstitial_iOS";
    private string _interstitialAdUnitId;

    private System.Action _onAdClosed;

    private bool _isAdReady = false;

    void Awake()
    {
        _interstitialAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;
    }

    public void LoadInterstitialAd()
    {
        Debug.Log("Loading Interstitial Ad: " + _interstitialAdUnitId);
        Advertisement.Load(_interstitialAdUnitId, this);
    }

    public void ShowInterstitialAd(System.Action onAdClosed)
    {
        if (!_isAdReady)
        {
            LoadInterstitialAd();      
            StartCoroutine(WaitForAdLoadAndShow());
            _onAdClosed = onAdClosed;
            return;
        }

        Advertisement.Show(_interstitialAdUnitId, this);
        _onAdClosed = onAdClosed;
    }

    public bool IsAdReady()
    {
        return _isAdReady;
    }

    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad");
        Advertisement.Load(RewardedAdsButton.Instance.GetAdUnitId(), this);
    }

    public void ShowRewardedAd()
    {
        Debug.Log("Showing Rewarded Ad");
        Advertisement.Show(RewardedAdsButton.Instance.GetAdUnitId(), this);
    }
    private IEnumerator WaitForAdLoadAndShow()
    {
        while (!_isAdReady)
        {
            yield return null;
        }

        Debug.Log("Ad loaded, now showing...");
        Advertisement.Show(_interstitialAdUnitId, this);
    }
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId == _interstitialAdUnitId)
        {
            _isAdReady = true;
            Debug.Log("Ad Loaded: " + adUnitId);
        }
        Debug.Log("Ad Loaded: " + adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        if (adUnitId == _interstitialAdUnitId)
        {
            _isAdReady = false;
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }

        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        if (adUnitId == _interstitialAdUnitId)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            _onAdClosed?.Invoke();
        }

        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        _onAdClosed?.Invoke(); 
        _isAdReady = false;
        _onAdClosed = null;
    }
}
