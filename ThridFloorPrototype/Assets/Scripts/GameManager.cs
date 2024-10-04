using Assets.HeroEditor4D.Common.Scripts;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int scoreNumber;
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private GameObject[] bossPrefab;

    private int totalEnemies;
    private int currentEnemyCount;

    [SerializeField] private Transform[] enemyPosF1;
    [SerializeField] private Transform[] enemyPosF2;
    [SerializeField] private Transform[] enemyPosF3;
    [SerializeField] private Transform[] enemyPosF4;
    [SerializeField] private Transform[] enemyPosF5;
    [SerializeField] private Transform[] enemyPosF6;
    [SerializeField] private Transform[] enemyPosF7;

    [SerializeField] private bool isTestTower;

    public bool isWin = false;
    public bool isLose = false;

    [SerializeField] private GameObject panelResult;
    [SerializeField] private Image panelResultImg; 
    [SerializeField] private Image panelResultImgRewardType;
    [SerializeField] private Sprite[] panelResultSprite;
    [SerializeField] private Image imgButtonWinLose;
    [SerializeField] private TextMeshProUGUI textResultWinLose;
    [SerializeField] private TextMeshProUGUI textResultReward;

    [SerializeField] private GameObject btnResultTryAgain;
    [SerializeField] private GameObject btnResultNext;

    [SerializeField] private Image imgLose;

    public List<GameObject> EnemyGroup1 = new();
    public List<GameObject> EnemyGroup2 = new();
    public List<GameObject> EnemyGroup3 = new();
    public List<GameObject> EnemyGroup4 = new();
    public List<GameObject> EnemyGroup5 = new();
    public List<GameObject> EnemyGroup6 = new();
    public List<GameObject> EnemyGroup7 = new();

    [SerializeField] private GameObject specialPrefab;

    private int lastPlayedRegion;
    private int lastPlayedStage;

    #region Star

    [SerializeField] private GameObject[] starImg;
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private float timeLimit = 60f;
    private float timeRemaining;
    private bool gameActive = false;
    #endregion

    public int KillCount;
    private int StarCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        CheckLevel();
    }

    private void Start()
    {
        foreach (GameObject img in starImg)
        {
            img.SetActive(false);
        }

        timeRemaining = timeLimit;

        gameActive = true;
        isWin = false;
        isLose = false;

        panelResult.SetActive(false); 
        StartCoroutine(GameTimer());
    }
    private void Update()
    {
        if (gameActive)
        {
            timerText.text = $"Time: {Mathf.Ceil(timeRemaining)}";
        }

        if(timeRemaining <= 0)
        {
            timeRemaining = 0;
        }
    }

    #region Game Timer

    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && 
        gameActive)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        if(timeRemaining == 0)
        {
            EndGameTimeOut();
        }
    }

    private void EndGameTimeOut()
    {
        isWin = false;
        isLose = true;

        CheckGameWins();
    }
    IEnumerator EndGameCheckStar()
    {
        int stars = CalculateStarsBasedOnTime();
        yield return new WaitForSeconds(0.5f);
        ShowStars(stars);
    }
    private int CalculateStarsBasedOnTime()
    {
        if (timeRemaining > 40)
            return 3;
        if (timeRemaining > 20)
            return 2;
        return 1;
    }
    private void ShowStars(int stars)
    {
        Sequence sequence = DOTween.Sequence();

        float scaleDuration = 0.3f; 

        for (int i = 0; i < starImg.Length; i++)
        {
            if (i < stars)
            {
                starImg[i].SetActive(true);
                starImg[i].transform.localScale = Vector3.zero;

                sequence.Append(starImg[i].transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));

                if (i < stars - 1) 
                {
                    sequence.AppendInterval(0.1f); 
                }
            }
            else
            {
                starImg[i].SetActive(false);
            }
        }
    }


    #endregion

    public void CheckGameWins()
    {
        if (isWin && !isLose)
        {
            StarCount = CalculateStarsBasedOnTime();

            DataCenter.Instance.SendScoreDataToApi(DataCenter.Instance.GetPlayerData().region, DataCenter.Instance.GetPlayerData().stage,
                StarCount, KillCount, WinCondition, textResultReward, panelResultImgRewardType);

            AdvanceToNextStage();
        }

        if (!isWin && isLose)
        {
            LoseCondition();
        }
    }
    private void AdvanceToNextStage()
    {
        isWin = true;
        
        lastPlayedRegion = DataCenter.Instance.GetPlayerData().region;
        lastPlayedStage = DataCenter.Instance.GetPlayerData().stage;

        DataCenter.Instance.GetPlayerData().stage++;

        if (DataCenter.Instance.GetPlayerData().stage > 35)
        {
            DataCenter.Instance.GetPlayerData().region++;
            DataCenter.Instance.GetPlayerData().stage = 1;
        }

        DataCenter.Instance.SendCurrentProgress(DataCenter.Instance.GetPlayerData().region,
            DataCenter.Instance.GetPlayerData().stage);
    }

    private void WinCondition()
    {
        gameActive = false;

        SoundManager.Instance.StopBGM();
        SoundManager.Instance.VictoryVFX();

        panelResult.GetComponent<OpenObjAnimation>().OpenGameObject();
        StartCoroutine(EndGameCheckStar());

        panelResultImg.sprite = panelResultSprite[0];
        textResultWinLose.text = "Win";

        textResultReward.gameObject.SetActive(true); 
        panelResultImgRewardType.gameObject.SetActive(true);
        imgLose.gameObject.SetActive(false);
    }
    private void LoseCondition()
    {
        gameActive = false;

        SoundManager.Instance.StopBGM();
        SoundManager.Instance.DefeatVFX();

        panelResult.GetComponent<OpenObjAnimation>().OpenGameObject();
        panelResultImg.sprite = panelResultSprite[1];
        textResultReward.gameObject.SetActive(false);
        panelResultImgRewardType.gameObject.SetActive(false);
        imgLose.gameObject.SetActive(true);
        textResultWinLose.text = "Lose";

        btnResultTryAgain.SetActive(true);
        btnResultNext.SetActive(false);
    }
   
    #region CHECK ENEMY

    public void CheckScore()
    {
        if (EnemyGroup1.Count == 0 &&
            EnemyGroup2.Count == 0 &&
            EnemyGroup3.Count == 0 &&
            EnemyGroup4.Count == 0 &&
            EnemyGroup5.Count == 0 &&
            EnemyGroup6.Count == 0 &&
            EnemyGroup7.Count == 0 )
        {
            isWin = true;
            isLose = false;
        }

        CheckGameWins();
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (EnemyGroup1.Contains(enemy))
        {
            EnemyGroup1.Remove(enemy);
        }
        else if (EnemyGroup2.Contains(enemy))
        {
            EnemyGroup2.Remove(enemy);
        }
        else if (EnemyGroup3.Contains(enemy))
        {
            EnemyGroup3.Remove(enemy);
        }
        else if (EnemyGroup4.Contains(enemy))
        {
            EnemyGroup4.Remove(enemy);
        }
        else if (EnemyGroup5.Contains(enemy))
        {
            EnemyGroup5.Remove(enemy);
        }
        else if (EnemyGroup6.Contains(enemy))
        {
            EnemyGroup6.Remove(enemy);
        }
        else if (EnemyGroup7.Contains(enemy))
        {
            EnemyGroup7.Remove(enemy);
        }
        else
        {
            Debug.LogWarning("Trying to remove an enemy that is not in any enemy group.");
        }

        CheckScore();
    }



    #endregion


    public void CheckLevel()
    {
        int regionToCheck = lastPlayedRegion;
        int stageToCheck = lastPlayedStage;

        StartCoroutine(GetLevelDataFromCsv(regionToCheck, stageToCheck, (levelData) =>
        {
            if (levelData == null)
            {
                Debug.LogError("Failed to load level data.");
                return;
            }

            Debug.Log($"Loaded LevelData: StartNumber = {levelData.StartNumber}");

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            List<string> allNumbers = new List<string>
        {
            levelData.Number1, levelData.Number2, levelData.Number3,
            levelData.Number4, levelData.Number5, levelData.Number6,
            levelData.Number7, levelData.Number8, levelData.Number9,
            levelData.Number10, levelData.Number11, levelData.Number12,
            levelData.Number13, levelData.Number14, levelData.Number15,
            levelData.Number16, levelData.Number17, levelData.Number18,
            levelData.Number19, levelData.Number20, levelData.Number21
        };

            totalEnemies = GetNonEmptyCount(allNumbers) - 1;
            Debug.Log($"Total enemies: {totalEnemies}");
            currentEnemyCount = 0;

            for (int i = 0; i < allNumbers.Count; i++)
            {
                string number = allNumbers[i];
                if (!string.IsNullOrEmpty(number))
                {
                    if (number.Contains("*") || number.Contains("/") || number.Contains("-") || (number.Contains("+")))
                    {
                        currentEnemyCount++;
                        InstantiateSpecialObject(number, i);
                    }
                    else
                    {
                        if (int.TryParse(number, out int numericValue))
                        {
                            Transform[] enemyPosArray = GetEnemyPosArray(i + 1);
                            int posIndex = i % 3;
                            currentEnemyCount++;
                            bool isLast = (currentEnemyCount == totalEnemies);
                            InstantiateEnemyAtPosition(enemyPosArray, posIndex, numericValue, isLast);
                        }
                    }
                }
            }

            scoreNumber = levelData.StartNumber;
            Debug.Log($"Score number set to: {scoreNumber}");
        }));
    }


    private int GetNonEmptyCount(List<string> list)
    {
        int count = 0;
        foreach (string item in list)
        {
            if (!string.IsNullOrEmpty(item))
            {
                count++;
            }
        }
        return count;
    }

    private Transform[] GetEnemyPosArray(int number)
    {
        if (number >= 1 && number <= 3) return enemyPosF1;
        if (number >= 4 && number <= 6) return enemyPosF2;
        if (number >= 7 && number <= 9) return enemyPosF3;
        if (number >= 10 && number <= 12) return enemyPosF4;
        if (number >= 13 && number <= 15) return enemyPosF5;
        if (number >= 16 && number <= 18) return enemyPosF6;
        if (number >= 19 && number <= 21) return enemyPosF7;

        return null;
    }

    private void InstantiateEnemyAtPosition(Transform[] enemyPosArray, int posIndex, int enemyNumber, bool isLast)
    {
        if (enemyPosArray == null || posIndex < 0 || posIndex >= enemyPosArray.Length)
        {
            Debug.LogError("Invalid enemy position array or position index.");
            return;
        }

        Transform pos = enemyPosArray[posIndex];
        GameObject prefabToInstantiate = isLast ? bossPrefab[UnityEngine.Random.Range(0, bossPrefab.Length)] : enemyPrefab[UnityEngine.Random.Range(0, enemyPrefab.Length)];
        GameObject enemy = Instantiate(prefabToInstantiate, pos.position, pos.rotation);
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.numberEnemy = enemyNumber;

            if (enemyPosArray == enemyPosF1)
            {
                EnemyGroup1.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF2)
            {
                EnemyGroup2.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF3)
            {
                EnemyGroup3.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF4)
            {
                EnemyGroup4.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF5)
            {
                EnemyGroup5.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF6)
            {
                EnemyGroup6.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF7)
            {
                EnemyGroup7.Add(enemy);
            }
        }
    }

    private void InstantiateSpecialObject(string number, int index)
    {
        Transform[] enemyPosArray = GetEnemyPosArray(index + 1);
        int posIndex = index % 3;

        if (enemyPosArray == null || posIndex < 0 || posIndex >= enemyPosArray.Length)
        {
            Debug.LogError("Invalid enemy position array or position index.");
            return;
        }

        Transform pos = enemyPosArray[posIndex];
        GameObject enemy = Instantiate(specialPrefab, pos.position, pos.rotation);
        SpecialEnemy enemyComponent = enemy.GetComponent<SpecialEnemy>(); 

        if (enemyComponent != null)
        {
            enemyComponent.numberEnemy = number;

            if (enemyPosArray == enemyPosF1)
            {
                EnemyGroup1.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF2)
            {
                EnemyGroup2.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF3)
            {
                EnemyGroup3.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF4)
            {
                EnemyGroup4.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF5)
            {
                EnemyGroup5.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF6)
            {
                EnemyGroup6.Add(enemy);
            }
            else if (enemyPosArray == enemyPosF7)
            {
                EnemyGroup7.Add(enemy);
            }
        }

    }

    public void LoadSceneAgain()
    {
        SoundManager.Instance.ClickSound();

        isWin = false;
        isLose = false;

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            StartCoroutine(DataCenter.Instance.GetTypeGameFromCsv(DataCenter.Instance.Region, DataCenter.Instance.Stage, typeGame =>
            {
                if (typeGame != -1)
                {
                    lastPlayedRegion = DataCenter.Instance.Region;
                    lastPlayedStage = DataCenter.Instance.Stage;

                    DataCenter.Instance.LoadSceneByTypeGame(typeGame);
                }
            }));
        });
    }

    public void LoadSceneTryAgain()
    {
        SoundManager.Instance.ClickSound();

        isWin = false;
        isLose = false;

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



    private IEnumerator GetLevelDataFromCsv(int region, int stage, System.Action<LevelData> callback)
    {
        var csvFile = DataCenter.Instance.GetCsvFile();
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found.");
            callback(null);
            yield break;
        }

        string[] lines = csvFile.text.Split('\n');
        foreach (var line in lines)
        {
            string[] columns = line.Split(',');

            if (columns.Length > 0 && int.TryParse(columns[0], out int csvRegion) &&
                csvRegion == region &&
                int.TryParse(columns[1], out int csvStage) &&
                csvStage == stage)
            {
                if (!int.TryParse(columns[4], out int startNumber))
                {
                    Debug.LogError($"Invalid start number for region {region}, stage {stage}. Value: {columns[4]}");
                    callback(null);
                    yield break;
                }

                LevelData levelData = new LevelData
                {
                    Number1 = columns[5],
                    Number2 = columns[6],
                    Number3 = columns[7],
                    Number4 = columns[8],
                    Number5 = columns[9],
                    Number6 = columns[10],
                    Number7 = columns[11],
                    Number8 = columns[12],
                    Number9 = columns[13],
                    Number10 = columns[14],
                    Number11 = columns[15],
                    Number12 = columns[16],
                    Number13 = columns[17],
                    Number14 = columns[18],
                    Number15 = columns[19],
                    Number16 = columns[20],
                    Number17 = columns[21],
                    Number18 = columns[22],
                    Number19 = columns[23],
                    Number20 = columns[24],
                    Number21 = columns[25],
                    StartNumber = startNumber
                };

                Debug.Log($"LevelData retrieved: StartNumber = {startNumber}");
                callback(levelData);
                yield break;
            }
        }

        Debug.LogError($"No data found for region {region}, stage {stage}");
        callback(null);
    }


    public bool IsInGroup(Transform target, List<GameObject> group)
    {
        foreach (GameObject enemy in group)
        {
            if (enemy.transform == target)
            {
                return true;
            }
        }
        return false;
    }

}
