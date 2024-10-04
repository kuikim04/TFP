using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

[Serializable]
public class Progress
{
    public int Region;
    public int Stage;
}

[Serializable]
public class InventoryItems
{
    public string ID;
    public string Name;
}

[Serializable]
public class Quest
{
    public string ID;
    public string Name;
    public string Description;
    public int Progress;
    public int Quantity;
    public bool RewardReceived;
}

[Serializable]
public class Reward
{
    public string ID;
    public string ItemType;
    public string ItemID;
    public int Quantity;
    public bool RewardReceived;
    public long Date; 
}

[Serializable]
public class Player
{
    public string ID;
    public int UserID;
    public string Name;
    public int Gold;
    public int Gem;
    public List<InventoryItems> Inventory;
    public List<Quest> QuestList;
    public List<Reward> RewardList;
    public long QuestGeneratedTime;
    public long WeeklyRewardGeneratedTime;
    public Progress Progress = new Progress();
    public bool AdRemoved;
}

[Serializable]
public class ResponseData
{
    public Player Player;
    public string message;

    public ResponseData()
    {
        Player = new Player();
    }
}


[Serializable]
public class RequestDataLogin
{
    public string Platform;
    public string Code;
    public string Name;
}

[Serializable]
public class VersionResponse
{
    public string Message;
    public string Version; 
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [SerializeField] private StartMenuScene startMenuScene;

    [SerializeField] private GameObject LoginFail;
    [SerializeField] private GameObject checkversionFail;
    [SerializeField] private TextMeshProUGUI textLoginFail;
    [SerializeField] private TextMeshProUGUI textCheckVersionFail;

    private string serverAccessCode;
    private string currentVersion;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentVersion = Application.version; 
        StartCoroutine(CheckGameVersion());
    }
    private IEnumerator CheckGameVersion()
    {
        string versionCheckUrl = "https://api.thirdfloorprototype.com/get/version";

        yield return ApiManager.Instance.GetRequest(versionCheckUrl, (response, statusCode) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to fetch version from API.");
                ShowVersionMismatchAlert("Failed to fetch version. Please try again later.");
                return;
            }

            var json = JsonUtility.FromJson<VersionResponse>(response);
            string latestVersion = json.Version.Trim();

            if (currentVersion != latestVersion)
            {
                ShowVersionMismatchAlert($"The Game has been updated, Press Confirm to proceed to store.");
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    SignIn();
                }
                else
                {
                    Debug.Log($"Platform is not Android. Skipping Google Play Games login. {currentVersion}");
                }
            }

            Debug.Log($"Last version:{latestVersion}, Current version: {currentVersion}");
        });
    }

    void ShowVersionMismatchAlert(string message)
    {
        if (textCheckVersionFail != null)
            textCheckVersionFail.text = message;

        if (checkversionFail != null)
            checkversionFail.SetActive(true);
    }
    public void OpenStoreForUpdate()
    {
        string storeUrl = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            storeUrl = "https://play.google.com/store/apps/details?id=com.ThirdFloorPrototype.CastleHero"; 
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            storeUrl = "https://apps.apple.com/app/idXXXXXXXXXX"; // 
        }
        else
        {
            Debug.LogWarning("Store update not supported on this platform.");
            return;
        }

        Application.OpenURL(storeUrl);
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();

            PlayGamesPlatform.Instance.RequestServerSideAccess(false, (code) =>
            {
                serverAccessCode = code;
                Debug.Log($"Server Access Code: {serverAccessCode}");

                if (!string.IsNullOrEmpty(serverAccessCode))
                {
                    StartCoroutine(SendLoginRequest(name, serverAccessCode));
                }
                else
                {
                    if (LoginFail != null)
                        LoginFail.SetActive(true);

                    if (textLoginFail != null)
                        textLoginFail.text = "Server Access Code is null or empty.";
                }
            });
        }
        else
        {
            if (LoginFail != null)
                LoginFail.SetActive(true);

            if (textLoginFail != null)
                textLoginFail.text = "Sign in Failed!!";
        }
    }

    private IEnumerator SendLoginRequest(string playerName, string accessCode)
    {
        if (DataCenter.Instance == null)
        {
            Debug.LogError("DataCenter.Instance is null in Login.");
            yield break;
        }

        string url = "https://api.thirdfloorprototype.com/login";

        RequestDataLogin requestData = new()
        {
            Platform = "Android",
            Code = accessCode,
            Name = playerName
        };

        string jsonData = JsonUtility.ToJson(requestData);

        yield return ApiManager.Instance.PostRequest(url, jsonData, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                StartCoroutine(DataCenter.Instance.GetShopData());
                StartCoroutine(startMenuScene.LoadMainMenuAsync());

                Debug.Log("API Response: " + response);
            }
            else
            {
                if(LoginFail != null)
                    LoginFail.SetActive(true);

                if (textLoginFail != null)
                    textLoginFail.text = "Failed to send data to API.";
            }
        });
    }

    public IEnumerator GetUserData()
    {
        string url = "https://api.thirdfloorprototype.com/get/playerdata";

        yield return ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (response != null)
            {
                try
                {
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);

                    if (responseData != null && responseData.Player != null)
                    {
                        Debug.Log("ResponseData.Player is not null.");
                        UpdateDataCenter(responseData.Player);
                    }
                    else
                    {
                        Debug.LogError("ResponseData or Player is null after deserialization.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception during JSON deserialization: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to receive response from GetUserData.");
            }
        });
    }

    private void UpdateDataCenter(Player player)
    {
        PlayerData playerData = DataCenter.Instance.GetPlayerData();

        if (playerData != null)
        {
            playerData.ID = player.ID;
            playerData.UserID = player.UserID;
            playerData.region = player.Progress.Region;
            playerData.stage = player.Progress.Stage;
            playerData.Coin = player.Gold;
            playerData.Diamond = player.Gem;
            playerData.Inventory = player.Inventory;
            playerData.AdsRemove = player.AdRemoved;

            Debug.Log("DataCenter updated successfully.");
        }
        else
        {
            Debug.LogError("PlayerData is not set in DataCenter.");
        }
    }
}
