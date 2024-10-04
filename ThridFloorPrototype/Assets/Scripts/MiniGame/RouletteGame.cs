using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RouletteGame : MonoBehaviour
{
    public GameObject knifePrefab;
    public Transform spawnPoint;

    public GameObject spinningWheel;
    public float throwSpeed = 20f;
    public float wheelRotationSpeed = 100f;

    private Tween spinTween;
    public Transform[] rouletteeSlot;

    public GameObject resultPanel;
    public TextMeshProUGUI textValueReward;
    public Image imgRewardType;
    public Sprite[] spriteRewardType;

    public bool isEndGame = false;

    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI countdownText;
    public float cooldownTime = 3f;
    public float countdownTime = 30f;
    private bool canTap = false;

    private void Start()
    {
        StartCoroutine(CooldownRoutine());
    }

    public void ThrowKnife()
    {
        if (canTap && !isEndGame)
        {
            GameObject newKnife = Instantiate(knifePrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = newKnife.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * throwSpeed;
        }
    }

    public void StopWheel()
    {
        if (spinTween != null)
        {
            spinTween.Kill();
        }
    }

    public void EndGame()
    {
        StartCoroutine(EndGameShowResult());
    }

    private IEnumerator EndGameShowResult()
    {
        SoundManager.Instance.StopBGM();
        yield return new WaitForSeconds(1.5f);
        SoundManager.Instance.VictoryVFX();
        // AdvanceToNextStage();
        resultPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
        isEndGame = true;
    }

    private IEnumerator CooldownRoutine()
    {
        cooldownText.gameObject.SetActive(true);
        float remainingTime = cooldownTime;

        while (remainingTime > 0)
        {
            cooldownText.text = $"Start in: {Mathf.Ceil(remainingTime)}";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        cooldownText.gameObject.SetActive(false);

        spinTween = spinningWheel.transform.DORotate(new Vector3(0, 0, 360), 1 / (wheelRotationSpeed / 360), RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        canTap = true;

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);
        float remainingTime = countdownTime;

        while (remainingTime > 0)
        {
            countdownText.text = $"{Mathf.Ceil(remainingTime)}s";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "0s"; 
        textValueReward.text = "x0"; 
        EndGame(); 
        countdownText.gameObject.SetActive(false);
    }
    private void AdvanceToNextStage()
    {
        DataCenter.Instance.GetPlayerData().stage++;

        if (DataCenter.Instance.GetPlayerData().stage > 30)
        {
            DataCenter.Instance.GetPlayerData().region++;
            DataCenter.Instance.GetPlayerData().stage = 1;
        }
    }

    public void LoadSceneAgain()
    {
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
   
}

