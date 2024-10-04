using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.IO;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private VerticalScrollMap mapImg;

    [SerializeField] private Button startGameBtn;

    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI enegyText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;

    [SerializeField] private TextMeshProUGUI[] regionProgressTexts;
    private const int stagesPerRegion = 35;

    [SerializeField] private GameObject panelMap;
    [SerializeField] private GameObject panelSelectStage;
    [SerializeField] private GameObject playerObjShow;

    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Transform stageTransform;

    [SerializeField] private GameObject loadingObject;

    [Header("SHOP")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject shopPageItem;
    [SerializeField] private GameObject shopPageCurrency;

    [SerializeField] private Button shopBtnItem;
    [SerializeField] private Button shopBtnCurrency;

    [SerializeField] private ScrollRect listSkin;

    [Header("DELETE DATA")]
    [SerializeField] private GameObject deleteDataPageBfConfirm;
    [SerializeField] private GameObject deleteDataPageAtConfirm;
    [SerializeField] private TextMeshProUGUI deleteDataText;

    private void Awake()
    {
        StartScene();
    }
    private void Start()
    {
        startGameBtn.onClick.AddListener(StartGameSelectMap);
    }

    private void StartScene()
    {
        loadingObject.SetActive(true);
        StartCoroutine(InitializeScene());
    }

    private IEnumerator InitializeScene()
    {
        yield return StartCoroutine(LoginManager.Instance.GetUserData());

        loadingObject.SetActive(false);

        UpdateStage();
    }

    private void Update()
    {
        UpdateCurrency();
    }

    public void UpdateCurrency()
    {
        UpdateCoinText();
        UpdateDiamondText();
    }

    private void OnDestroy()
    {
        startGameBtn.onClick.RemoveAllListeners();
    }

    public void StartGameSelectMap()
    {
        loadingObject.SetActive(true);


        for (int region = 0; region < regionProgressTexts.Length; region++)
        {
            if (region + 1 < DataCenter.Instance.Region)
            {
                regionProgressTexts[region].text = "100%";
            }
            else if (region + 1 == DataCenter.Instance.Region)
            {
                if (DataCenter.Instance.Stage == 1)
                {
                    regionProgressTexts[region].text = "0%";
                }
                else if (DataCenter.Instance.Stage == stagesPerRegion)
                {
                    regionProgressTexts[region].text = "99%";
                }
                else
                {
                    float progressPercent = ((float)(DataCenter.Instance.Stage - 1) / stagesPerRegion) * 100f;
                    regionProgressTexts[region].text = progressPercent.ToString("F0") + "%";
                }
            }
            else
            {
                regionProgressTexts[region].text = "0%";
            }
        }

        loadingObject.SetActive(false);
        mapImg.ResetPosition();

        playerObjShow.SetActive(false);
        SoundManager.Instance.ClickSound();
        panelMap.SetActive(true);
    }

    public void CloseSelectMap()
    {
        playerObjShow.SetActive(true);
        SoundManager.Instance.ClickSound();
        panelMap.SetActive(false);
    }

    public void SelectRegion(int number)
    {
        panelMap.SetActive(false);
        panelSelectStage.SetActive(true);

        DataCenter.Instance.LoadStagesForRegion(number, stagePrefab, stageTransform, loadingObject);
    }

    public void CloseStagePanel()
    {
        panelMap.SetActive(true);
        panelSelectStage.SetActive(false);

        foreach (Transform Child in stageTransform)
        {
            Destroy(Child.gameObject);
        }
    }

    private void UpdateStage()
    {
        stageText.text = $"Stage {DataCenter.Instance.GetPlayerData().region}-{DataCenter.Instance.GetPlayerData().stage}";
    }

    public void UpdateCoinText()
    {
        coinText.text = FormatCurrency(DataCenter.Instance.GetPlayerData().Coin);
    }
    public void UpdateDiamondText()
    {
        diamondText.text = FormatCurrency(DataCenter.Instance.GetPlayerData().Diamond);
    }

    private string FormatCurrency(int currency)
    {
        if (currency >= 1000000)
        {
            return Math.Floor(currency / 1000000.0 * 10) / 10 + "M";
        }
        else if (currency >= 10000)
        {
            return Math.Floor(currency / 1000.0 * 10) / 10 + "K";
        }
        else
        {
            return currency.ToString("N0");
        }
    }


    public void OpenShopPage()
    {
        shopPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
        OpenShopPageItem();
    }
    public void OpenShopPageFromCurrency()
    {
        shopPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
        OpenShopPageCurrency();
    }

    public void OpenShopPageItem()
    {
        listSkin.verticalNormalizedPosition = 1;

        shopBtnItem.enabled = false;
        shopBtnCurrency.enabled = true;

        shopPageItem.SetActive(true);
        shopPageCurrency.SetActive(false);

    }
    public void OpenShopPageCurrency()
    {
        shopBtnItem.enabled = true;
        shopBtnCurrency.enabled = false;

        shopPageItem.SetActive(false);
        shopPageCurrency.SetActive(true);
    }

    public void DeleteDataBtn()
    {
        deleteDataPageBfConfirm.SetActive(false);
        DeleteData();
    }

    private void DeleteData()
    {
        string url = "https://api.thirdfloorprototype.com/get/datadelete";

        StartCoroutine(ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (statusCode == 200 && !string.IsNullOrEmpty(response))
            {
                deleteDataPageAtConfirm.GetComponent<OpenObjAnimation>().OpenGameObject();

                DeleteResponse deleteResponse = JsonUtility.FromJson<DeleteResponse>(response);

                if (deleteResponse != null)
                {
                    deleteDataText.text = deleteResponse.message;
                }
            }
            else
            {
                Debug.LogError("Failed to delete data. Status Code: " + statusCode);
            }
        }));
    }
}

[Serializable]
public class DeleteResponse
{
    public string message;
}