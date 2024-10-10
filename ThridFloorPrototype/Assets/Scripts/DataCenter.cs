using Assets.HeroEditor4D.Common.Scripts.Collections;
using Assets.HeroEditor4D.Common.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[System.Serializable]
public class ProgressData
{
    public int Region;
    public int Stage;
}

[Serializable]
public class ProgressResponse
{
    public List<RegionProgress> Progress;
}

[Serializable]
public class RegionProgress
{
    public int Region;
    public List<int> Score;
}

[Serializable]
public class SendProgress
{
    public int Region;
    public int Stage;
    public int Score;
    public int Kill;
}

[Serializable]
public class ShopItem
{
    public string ItemID;
    public string PriceType;
    public int Price;
    public string ItemName;
    public string EquipmentPart;
    public string BodyPart;
}

[Serializable]
public class ShopResponse
{
    public ShopItem[] Items;
    public string message;
}

[Serializable]
public class RewardReciver
{
    public string ItemType;
    public string ItemID;
    public string EquipmentPart;
    public string BodyPart;
    public int Quantity;
}

[Serializable]
public class RewardResponse
{
    public string message;
    public RewardReciver Reward;
}


public class DataCenter : MonoBehaviour
{
    public IconCollection IconCollection;

    private static DataCenter instance;
    public List<ShopItem> shopItems = new();

    public static DataCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataCenter>();
                if (instance == null)
                {
                    GameObject go = new GameObject("DataCenter");
                    instance = go.AddComponent<DataCenter>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [SerializeField] private PlayerData playerData;

    [SerializeField] private TextAsset csvFile;

    [SerializeField]
    private Sprite itemTypeGold;
    [SerializeField]
    private Sprite itemTypeGem;


    public int InitialRegion;
    public int InitialStage;

    public bool IsTryAgain;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public int Region
    {
        get { return playerData.region; }
    }

    public int Stage
    {
        get { return playerData.stage; }
    }




    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    public TextAsset GetCsvFile()
    {
        return csvFile;
    }

    public void SaveRegion()
    {
        PlayerPrefs.SetInt(Key.KEY_REGION, DataCenter.Instance.Region);
        PlayerPrefs.Save();
    }

    public void LoadRegion()
    {
        int savedStage = PlayerPrefs.GetInt(Key.KEY_REGION, 1);
        GetPlayerData().stage = savedStage;
    }

    public void SaveStage()
    {
        PlayerPrefs.SetInt(Key.KEY_STAGE, DataCenter.Instance.Stage);
        PlayerPrefs.Save();
    }

    public void LoadStage()
    {
        int savedStage = PlayerPrefs.GetInt(Key.KEY_STAGE, 1);
        GetPlayerData().stage = savedStage;
    }

    public void LoadSceneByTypeGame(int typeGame)
    {
        switch (typeGame)
        {
            case 0:
                switch (Region)
                {
                    case 1:
                        SoundManager.Instance.PlayMusicRegion1();
                        break;
                    case 2:
                        SoundManager.Instance.PlayMusicRegion2();
                        break;
                    case 3:
                        SoundManager.Instance.PlayMusicRegion3();
                        break;
                    case 4:
                        SoundManager.Instance.PlayMusicRegion4();
                        break;
                }
                SceneManager.LoadScene("GamePlay");
                break;
            case 1:
                SoundManager.Instance.PlayMusicMiniGameRPS();
                SceneManager.LoadScene("MiniGameID1");
                break;
            case 2:
                SoundManager.Instance.PlayMusicMiniGameArrow();
                SceneManager.LoadScene("MiniGameID2");
                break;
            case 3:
                SoundManager.Instance.PlayMusicMiniGameTap();
                SceneManager.LoadScene("MiniGameID3");
                break;
            default:
                Debug.LogError($"Unknown typegame: {typeGame}");
                break;
        }
    }

    public IEnumerator GetTypeGameFromCsv(int region, int stage, Action<int> callback)
    {
        if (GetCsvFile() == null)
        {
            Debug.LogError("CSV file not assigned in the inspector");
            callback(-1);
            yield break;
        }

        string[] lines = DataCenter.Instance.GetCsvFile().text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');

            if (values.Length >= 5)
            {
                if (int.TryParse(values[0].Trim(), out int csvRegion) &&
                    int.TryParse(values[1].Trim(), out int csvStage) &&
                    int.TryParse(values[3].Trim(), out int typeGame))
                {
                    if (csvRegion == region && csvStage == stage)
                    {
                        callback(typeGame);
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to parse CSV values: {line}");
                }
            }
        }

        Debug.LogError($"No matching type game found for region {region} and stage {stage} in CSV.");
        callback(-1);
    }

    public void LoadStagesForRegion(int region, GameObject stagePrefab, Transform stageTransform, GameObject loading)
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file not assigned.");
            return;
        }

        FetchProgressForRegion(region, (scores) =>
        {
            if (scores == null)
            {
                Debug.LogError("Failed to load scores for the region.");
                return;
            }

            string[] lines = csvFile.text.Split('\n');
            Dictionary<int, string> stageData = new Dictionary<int, string>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                if (columns.Length > 0 && int.TryParse(columns[0], out int csvRegion) && csvRegion == region)
                {
                    if (int.TryParse(columns[1], out int stage))
                    {
                        stageData[stage] = line;
                    }
                    else
                    {
                        Debug.LogError($"Invalid stage number in line: {line}");
                    }
                }
            }

            int currentRegion = GetPlayerData().region;
            int currentStage = GetPlayerData().stage;

            foreach (var stageEntry in stageData)
            {
                int stage = stageEntry.Key;
                string lineData = stageEntry.Value;
                string[] columns = lineData.Split(',');

                GameObject stageInstance = Instantiate(stagePrefab, stageTransform);
                TextMeshProUGUI stageText = stageInstance.GetComponentInChildren<TextMeshProUGUI>();

                if (stageText != null)
                {
                    stageText.text = $"{region}-{stage}";
                }

                if (columns.Length > 3 && int.TryParse(columns[3], out int typeGame) && typeGame != 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        stageInstance.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (scores != null && stage - 1 < scores.Count)
                    {
                        int score = scores[stage - 1];

                        for (int i = 0; i < 3; i++)
                        {
                            Transform childTransform = stageInstance.transform.GetChild(i);

                            if (childTransform.childCount > 0)
                            {
                                childTransform.GetChild(0).gameObject.SetActive(i < score);
                            }
                            else
                            {
                                Debug.LogWarning($"No child found at index {i} for stage {stage}.");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No score found for stage {stage}.");
                    }
                }

                Button stageButton = stageInstance.GetComponent<Button>();

                if (stageButton != null)
                {
                    int capturedRegion = region;
                    int capturedStage = stage;

                    bool isStageAccessible = (capturedRegion < currentRegion) || (capturedRegion == currentRegion && capturedStage <= currentStage);

                    if (!isStageAccessible)
                    {
                        stageButton.interactable = false;
                    }
                    else
                    {
                        stageButton.onClick.AddListener(() =>
                        {
                            loading.SetActive(true);

                            StartCoroutine(GetTypeGameFromCsv(capturedRegion, capturedStage, typeGame =>
                            {
                                if (typeGame != -1)
                                {
                                    GetPlayerData().region = capturedRegion;
                                    GetPlayerData().stage = capturedStage;

                                    InitialRegion = capturedRegion;
                                    InitialStage = capturedStage;

                                    IsTryAgain = false;

                                    LoadSceneByTypeGame(typeGame);
                                }
                            }));
                        });
                    }
                }
            }
        });
    }



    public void FetchProgressForRegion(int region, Action<List<int>> onProgressFetched)
    {
        string url = $"https://api.thirdfloorprototype.com/get/progress";

        StartCoroutine(ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                var progressData = JsonUtility.FromJson<ProgressResponse>(response);
                var regionProgress = progressData.Progress.Find(p => p.Region == region);

                if (regionProgress != null)
                {
                    onProgressFetched?.Invoke(regionProgress.Score);
                }
                else
                {
                    Debug.LogError("Region progress not found.");
                    onProgressFetched?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch progress data.");
                onProgressFetched?.Invoke(null);
            }
        }));
    }


    public void SendCurrentProgress(int region, int stage)
    {
        string url = "https://api.thirdfloorprototype.com/set/currentprogress";

        ProgressData progressData = new ProgressData
        {
            Region = region,
            Stage = stage
        };

        string jsonData = JsonUtility.ToJson(progressData);

        StartCoroutine(ApiManager.Instance.PostRequest(url, jsonData, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                Debug.Log("Progress successfully sent: " + response);
            }
            else
            {
                Debug.LogError("Failed to send progress.");
            }
        }));
    }

    public void SendScoreDataToApi(int region, int stage, int score, int kill, Action onComplete, TextMeshProUGUI rewardText, Image rewardImage)
    {
        SendProgress scoreData = new()
        {
            Region = region,
            Stage = stage,
            Score = score,
            Kill = kill
        };

        string jsonData = JsonUtility.ToJson(scoreData);
        string url = "https://api.thirdfloorprototype.com/set/score";

        StartCoroutine(ApiManager.Instance.PostRequest(url, jsonData, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                RewardResponse rewardResponse = JsonUtility.FromJson<RewardResponse>(response);

                if (rewardResponse != null)
                {
                    if (rewardImage != null)
                    {
                        SetRewardImage(rewardImage, rewardResponse.Reward.ItemType, rewardResponse);
                    }

                    if (rewardText != null)
                    {
                        rewardText.text = $"x{rewardResponse.Reward.Quantity}";
                    }

                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError("Failed to parse reward data.");
                }
            }
            else
            {
                Debug.LogError("Failed to send score data. Empty response.");
            }
        }));
    }

    public void SetRewardImage(Image rewardImg, string typeReward, RewardResponse rewardData)
    {
        rewardImg.sprite = typeReward switch
        {
            "Gold" => itemTypeGold,
            "Gem" => itemTypeGem,
            "Item" => SetItemImage(rewardData.Reward),
            null => itemTypeGold,
            _ => itemTypeGold,
        };
    }

    private Sprite SetItemImage(RewardReciver response)
    {
        Sprite itemSprite = null;

        string adjustedItemID = AdjustItemIDForEquipmentPart(response.EquipmentPart, response.ItemID);

        itemSprite = IconCollection.GetIcon(adjustedItemID);

        return itemSprite;
    }

    private string AdjustItemIDForEquipmentPart(string equipmentPart, string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) return itemID;

        return equipmentPart switch
        {
            "Helmet" => itemID.Replace(".Armor.", ".Helmet."),
            "Leggings" => itemID.Replace(".Armor.", ".Leggings."),
            _ => itemID,
        };
    }

    public IEnumerator GetShopData()
    {    
        string apiUrl = "https://api.thirdfloorprototype.com/get/equipmentshop";


        yield return StartCoroutine(ApiManager.Instance.GetRequest(apiUrl, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                ShopResponse shopResponse = JsonUtility.FromJson<ShopResponse>(response);
                if (shopResponse != null && shopResponse.Items != null)
                {
                    shopItems = new List<ShopItem>(shopResponse.Items);
                }
                else
                {
                    Debug.LogError("Error parsing shop data.");
                }
            }
            else
            {
                Debug.LogError("Failed to get shop data.");
            }
        }));
    }

}

