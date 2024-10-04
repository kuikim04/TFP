using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScene : MonoBehaviour
{
    [SerializeField] private float duration = 1f;

    [SerializeField] private TextMeshProUGUI textLoading;
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private Slider loadingSlider;
    void Start()
    {
        if (loadingObj != null)
        {
            loadingObj.SetActive(true);
        }

    }
    public IEnumerator LoadMainMenuAsync()
    {
        if (loadingObj != null)
        {
            loadingObj.SetActive(true);
            StartLoadingTextAnimation();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");

        while (asyncLoad.progress < 0.9f) 
        {
            if (loadingSlider != null)
            {
                loadingSlider.value = asyncLoad.progress;
            }
            yield return null;
        }

        if (loadingSlider != null)
        {
            loadingSlider.value = 1f;
        }

        bool adClosed = false;

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            adClosed = true;
        });

        yield return new WaitUntil(() => adClosed);

        while (!asyncLoad.isDone)
        {
            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.Clamp01(asyncLoad.progress);
            }
            yield return null;
        }

        if (loadingObj != null)
        {
            loadingObj.SetActive(false);
        }
    }


    void StartLoadingTextAnimation()
    {
        if (textLoading != null)
        {
            Sequence loadingSequence = DOTween.Sequence();
            loadingSequence.AppendCallback(() => textLoading.text = "Loading game .");
            loadingSequence.AppendInterval(0.5f);
            loadingSequence.AppendCallback(() => textLoading.text = "Loading game ..");
            loadingSequence.AppendInterval(0.5f);
            loadingSequence.AppendCallback(() => textLoading.text = "Loading game ...");
            loadingSequence.AppendInterval(0.5f);
            loadingSequence.SetLoops(-1, LoopType.Restart);
        }
    }
    public void ShowAdAndLoadMainMenu()
    {
       
    }
}
