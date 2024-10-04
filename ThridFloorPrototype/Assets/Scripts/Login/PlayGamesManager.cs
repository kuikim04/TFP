using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayGamesManager : MonoBehaviour
{
    public TextMeshProUGUI DetailsText;
    private string serverAccessCode;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SignIn();
        }
        else
        {
            DetailsText.text = "This platform is not supported for Google Play Games login.";
            Debug.Log("Platform is not Android. Skipping Google Play Games login.");
        }
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

                DetailsText.text = $"Success \nID: {id}\nName: {name}\nServerAccessCode: {serverAccessCode}";

                Debug.Log($"Server Access Code: {serverAccessCode}");

                if (!string.IsNullOrEmpty(serverAccessCode))
                {
                    StartCoroutine(SendLoginRequest(name, serverAccessCode));
                }
                else
                {
                    DetailsText.text = "Server Access Code is null or empty.";
                }
            });
        }
        else
        {
            DetailsText.text = "Sign in Failed!!";
        }
    }

    private IEnumerator SendLoginRequest(string playerName, string accessCode)
    {
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
                Debug.Log("API Response: " + response);
            }
            else
            {
                Debug.LogError("Failed to send data to API.");
            }
        });
    }
}
