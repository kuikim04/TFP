using Assets.HeroEditor4D.Common.Scripts.Collections;
using Assets.HeroEditor4D.InventorySystem.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static QuestSystem;

public class RewardHandler : MonoBehaviour
{
    [SerializeField] private IconCollection iconCollection; 
    [SerializeField] private Button btnReceiveReward;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [SerializeField] private GameObject showRewardResult;

    [SerializeField] private Image imgRewardResult;

    [SerializeField] private TextMeshProUGUI textRewardResult;

    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite gemSprite;

    [SerializeField] private GameObject playerMain;
    void Start()
    {
        btnReceiveReward.interactable = false;

        btnReceiveReward.onClick.AddListener(ShowAdForReward);
        RewardedAdsButton.OnRewardedAdCompleted += OnRewardedAdCompleted;

        CheckCooldownFromApi();
    }

    private void OnDestroy()
    {
        btnReceiveReward.onClick.RemoveAllListeners();
        RewardedAdsButton.OnRewardedAdCompleted -= OnRewardedAdCompleted;
    }

    private void ShowAdForReward()
    {
        AdsManager.Instance.RewardedAdsButton.ShowAd();
    }

    private void CheckCooldownFromApi()
    {
        string url = "https://api.thirdfloorprototype.com/get/adsrewardcooldown";

        StartCoroutine(ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (statusCode == 200 && !string.IsNullOrEmpty(response))
            {
                Debug.Log("API Response: " + response);

                CooldownResponse cooldownResponse = JsonUtility.FromJson<CooldownResponse>(response);

                long timeEndUnix = cooldownResponse.timeEnd;

                DateTime timeEnd = DateTimeOffset.FromUnixTimeSeconds(timeEndUnix).UtcDateTime;

                TimeSpan timeRemaining = timeEnd - DateTime.UtcNow;

                if (timeRemaining.TotalMilliseconds > 0)
                {
                    StartCoroutine(UpdateCooldownUI(timeRemaining));
                    btnReceiveReward.interactable = false;
                }
                else
                {
                    cooldownText.text = "Reward Available!";
                    btnReceiveReward.interactable = true;
                }

            }
            else
            {
                Debug.LogError("Failed to get cooldown. Status Code: " + statusCode);
            }
        }));
    }

    private IEnumerator UpdateCooldownUI(TimeSpan timeRemaining)
    {
        while (timeRemaining.TotalMilliseconds > 0)
        {
            cooldownText.text = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
            btnReceiveReward.interactable = false;

            yield return new WaitForSeconds(1);

            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
        }

        cooldownText.text = "Reward Available!";
        btnReceiveReward.interactable = true;
    }

    private void OnRewardedAdCompleted()
    {
        GetRewardFromApi();
    }

    private void GetRewardFromApi()
    {
        string url = "https://api.thirdfloorprototype.com/get/adsreward";

        StartCoroutine(ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (statusCode == 200 && !string.IsNullOrEmpty(response))
            {
                Debug.Log("Reward API Response: " + response);

                RewardResponseAds rewardResponse = JsonUtility.FromJson<RewardResponseAds>(response);

                RewardButtonApi reward = rewardResponse.reward;

                Debug.Log($"Received Reward: {reward.ItemType}, ID: {reward.ItemID}, Quantity: {reward.Quantity}");

                switch (reward.ItemType)
                {
                    case "Gem":
                        GiveDiamonds(reward.Quantity);
                        break;
                    case "Gold":
                        GiveGold(reward.Quantity);
                        break;
                    case "Item":
                        GiveItem(reward);
                        break;
                }

                showRewardResult.SetActive(true);

                playerMain.SetActive(false);

                CheckCooldownFromApi();

            }
            else
            {
                Debug.LogError("Failed to get reward. Status Code: " + statusCode);
            }
        }));
    }




    private void GiveDiamonds(int quantity)
    {
        imgRewardResult.sprite = gemSprite;
        textRewardResult.text = $"x {quantity.ToString()}";
        StartCoroutine(LoginManager.Instance.GetUserData());
    }

    private void GiveGold(int quantity)
    {
        imgRewardResult.sprite = goldSprite;
        textRewardResult.text = $"x {quantity.ToString()}";
        StartCoroutine(LoginManager.Instance.GetUserData());
    }

    private void GiveItem(RewardButtonApi reward)
    {
        imgRewardResult.sprite = SetItemImage(reward);
        StartCoroutine(LoginManager.Instance.GetUserData());
    }

    private Sprite SetItemImage(RewardButtonApi response)
    {
        Sprite itemSprite = null;

        string adjustedItemID = AdjustItemIDForEquipmentPart(response.EquipmentPart, response.ItemID);

        itemSprite = iconCollection.GetIcon(adjustedItemID);

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

    public void CloseRewardAds()
    {
        playerMain.SetActive(true);
        imgRewardResult.sprite =  null;
        showRewardResult.SetActive(false);
    }
}

[Serializable]
public class RewardResponseAds
{
    public string message;
    public RewardButtonApi reward;
}

[Serializable]
public class RewardButtonApi
{
    public string ItemType;
    public string ItemID;
    public string BodyPart;
    public string EquipmentPart;
    public int Quantity;
}

[Serializable]
public class CooldownResponse
{
    public string message;
    public long timeEnd;
}