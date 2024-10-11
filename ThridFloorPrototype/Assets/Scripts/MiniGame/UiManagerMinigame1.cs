using DG.Tweening;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerMinigame1 : MonoBehaviour
{
    [SerializeField] private KnifeThrower knifeThrower;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI knifeCountText;
    public TextMeshProUGUI resultHeaderText;
    [SerializeField] private Image resultImageReward;

    public GameObject knifeIconPrefab;
    public GameObject ResultPanel;

    public GameObject KnifeCenter;

    public Transform knifeIconContainer;

    private float timer = 20.0f;
    private bool isFlashing = false;
    private bool isEnd = false;

    [SerializeField] private Image bg;
    [SerializeField] private Sprite[] bgRegion1;
    [SerializeField] private Sprite[] bgRegion2;
    [SerializeField] private Sprite[] bgRegion3;
    [SerializeField] private Sprite[] bgRegion4;


    [SerializeField] private SpriteRenderer circleImg;
    [SerializeField] private Sprite[] circleSpriter;

    private bool isSendApi = false;
    private int realScore = 0;

    private int lastPlayedRegion;
    private int lastPlayedStage;
    void Start()
    {
        AdsManager.Instance.InCreaseAds();

        lastPlayedRegion = DataCenter.Instance.InitialRegion;
        lastPlayedStage = DataCenter.Instance.InitialStage;

        UpdateScoreText();
        UpdateKnifeCountText();
        UpdateKnifeIcons();
        StartCoroutine(StartCountdown());
        SetBackgroundByRegion();

        circleImg.sprite = circleSpriter[UnityEngine.Random.Range(0, circleSpriter.Length)];        
    }
    private void SetBackgroundByRegion()
    {
        int currentRegion = DataCenter.Instance.GetPlayerData().region;

        switch (currentRegion)
        {
            case 1:
                bg.sprite = GetRandomBackground(bgRegion1);
                break;
            case 2:
                bg.sprite = GetRandomBackground(bgRegion2);
                break;
            case 3:
                bg.sprite = GetRandomBackground(bgRegion3);
                break;
            case 4:
                bg.sprite = GetRandomBackground(bgRegion4);
                break;
            default:
                Debug.LogWarning("Unknown region: " + currentRegion);
                break;
        }
    }

    private Sprite GetRandomBackground(Sprite[] backgrounds)
    {
        if (backgrounds.Length == 0)
        {
            Debug.LogError("Background array is empty.");
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, backgrounds.Length);
        return backgrounds[randomIndex];
    }

    void UpdateScoreText()
    {
        scoreText.text = knifeThrower.ScoreMiniGame.ToString();
    }

    void UpdateKnifeCountText()
    {
        knifeCountText.text = knifeThrower.TotalKnife.ToString();
    }

    void UpdateKnifeIcons()
    {
        foreach (Transform child in knifeIconContainer)
        {
            Destroy(child.gameObject);
        }

        int iconsToShow = Mathf.Min(knifeThrower.TotalKnife, 5);  

        for (int i = 0; i < iconsToShow; i++)
        {
            Instantiate(knifeIconPrefab, knifeIconContainer);
        }
    }

    IEnumerator StartCountdown()
    {
        while (timer > 0 && !isEnd)
        {
            timer -= Time.deltaTime;
            int seconds = Mathf.CeilToInt(timer);
            timerText.text = seconds + "s";

            if (seconds <= 10 && !isFlashing)
            {
                isFlashing = true;
                FlashTimerText();
            }

            yield return null;
        }

        if (!isEnd)
        {
            timer = 0;
            timerText.text = "0s";

            if(knifeThrower.TotalKnife != 0)
            {
                if (!isSendApi)
                {
                    isSendApi = true;
                    realScore = Mathf.CeilToInt((float)knifeThrower.ScoreMiniGame / 5);

                    DataCenter.Instance.SendScoreDataToApi(lastPlayedRegion, lastPlayedStage,
                      realScore, knifeThrower.ScoreMiniGame, EndGame, resultHeaderText, resultImageReward);
                }
            }
        }
    }

    void FlashTimerText()
    {
        timerText.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void IncreaseScore(int amount)
    {
        knifeThrower.ScoreMiniGame += amount;
        UpdateScoreText();
    }
    public void DecreaseScore(int amount)
    {
        if (knifeThrower.ScoreMiniGame <= 0)
            return;

        knifeThrower.ScoreMiniGame -= amount;
        UpdateScoreText();
    }


    public void DecreaseKnifeCount()
    {
        knifeThrower.TotalKnife--;
        UpdateKnifeCountText();
        UpdateKnifeIcons();

        if (knifeThrower.TotalKnife <= 0)
            StartCoroutine(SetEndGame());
    }
    IEnumerator SetEndGame()
    {
        KnifeCenter.SetActive(false);
        SoundManager.Instance.StopBGM();
        yield return new WaitForSeconds(1.5f);

        if (!isSendApi)
        {
            isSendApi = true;
            realScore = Mathf.CeilToInt((float)knifeThrower.ScoreMiniGame / 5);

            DataCenter.Instance.SendScoreDataToApi(lastPlayedRegion, lastPlayedStage,
              realScore, knifeThrower.ScoreMiniGame, EndGame, resultHeaderText, resultImageReward);

            AdvanceToNextStage();
        }
        
    }
    public void EndGame()
    {
        SoundManager.Instance.VictoryVFX();

        isEnd = true;

        ResultPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
    }

    private void AdvanceToNextStage()
    {
        if (!DataCenter.Instance.IsTryAgain)
        {
            DataCenter.Instance.GetPlayerData().stage++;

            if (DataCenter.Instance.GetPlayerData().stage > 35)
            {
                DataCenter.Instance.GetPlayerData().region++;
                DataCenter.Instance.GetPlayerData().stage = 1;
            }
        }

        DataCenter.Instance.SendCurrentProgress(DataCenter.Instance.GetPlayerData().region,
           DataCenter.Instance.GetPlayerData().stage);

    }

    public void LoadSceneAgain()
    {
        DataCenter.Instance.IsTryAgain = false;

        DataCenter.Instance.InitialRegion = DataCenter.Instance.GetPlayerData().region;
        DataCenter.Instance.InitialStage = DataCenter.Instance.GetPlayerData().stage;

        SoundManager.Instance.ClickSound();

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            StartCoroutine(DataCenter.Instance.GetTypeGameFromCsv(DataCenter.Instance.Region, DataCenter.Instance.Stage, typeGame =>
            {
                if (typeGame != -1)
                {
                    DataCenter.Instance.LoadSceneByTypeGame(typeGame);
                }
            }));
        });
    }

    public void LoadSceneTryAgain()
    {
        DataCenter.Instance.IsTryAgain = true;

        SoundManager.Instance.ClickSound();

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            StartCoroutine(DataCenter.Instance.GetTypeGameFromCsv(lastPlayedRegion, lastPlayedStage, typeGame =>
            {
                if (typeGame != -1)
                {
                    DataCenter.Instance.LoadSceneByTypeGame(typeGame);
                }
            }));
        });
    }

}
