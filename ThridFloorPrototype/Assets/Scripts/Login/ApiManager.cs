using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [SerializeField] private GameObject sectionNoticeObj;
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
    private void ShowSessionExpiredNotice()
    {
        sectionNoticeObj.SetActive(true); 
        exitBtn.onClick.AddListener(() =>
        {
            ExitGame();
        });
    }

    public IEnumerator PostRequest(string url, string jsonData, Action<string, long> callback)
    {
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(postData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        long statusCode = www.responseCode; // ดึงสถานะ HTTP

        if (statusCode == 401)
        {
            ShowSessionExpiredNotice();
            yield break; 
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error + " | Status Code: " + statusCode);
            callback?.Invoke(null, statusCode); // ส่ง callback พร้อมสถานะ HTTP
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text + " | Status Code: " + statusCode);
            callback?.Invoke(www.downloadHandler.text, statusCode); // ส่ง callback พร้อม response และสถานะ HTTP
        }
    }


    public IEnumerator GetRequest(string url, Action<string, long> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        long statusCode = www.responseCode;

        if (statusCode == 401)
        {
            ShowSessionExpiredNotice();
            yield break;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error + " | Status Code: " + statusCode);
            callback?.Invoke(null, statusCode); 
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text + " | Status Code: " + statusCode);
            callback?.Invoke(www.downloadHandler.text, statusCode); 
        }
    }


    public IEnumerator PutRequest(string url, string jsonData, Action<string, long> callback)
    {
        byte[] putData = Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest www = new UnityWebRequest(url, "PUT");
        www.uploadHandler = new UploadHandlerRaw(putData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        long statusCode = www.responseCode;

        if (statusCode == 401)
        {
            ShowSessionExpiredNotice();
            yield break;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error + " | Status Code: " + statusCode);
            callback?.Invoke(null, statusCode);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text + " | Status Code: " + statusCode);
            callback?.Invoke(www.downloadHandler.text, statusCode); 
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
