using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestSystem;

public class QuestSystem : MonoBehaviour
{
    private List<Quest> questList = new List<Quest>();

    [SerializeField] private GameObject questPage;
    [SerializeField] private GameObject LoadingObj;
    [SerializeField] private Button buttonQuest;

    [SerializeField] private TextMeshProUGUI quest1Text;
    [SerializeField] private TextMeshProUGUI quest2Text;
    [SerializeField] private TextMeshProUGUI quest3Text;
    [SerializeField] private TextMeshProUGUI quest4Text;
    [SerializeField] private TextMeshProUGUI quest5Text;

    [SerializeField] private Button quest1Btn;
    [SerializeField] private Button quest2Btn;
    [SerializeField] private Button quest3Btn;
    [SerializeField] private Button quest4Btn;
    [SerializeField] private Button quest5Btn;

    [SerializeField] private Image quest1Image;
    [SerializeField] private Image quest2Image;
    [SerializeField] private Image quest3Image;
    [SerializeField] private Image quest4Image;
    [SerializeField] private Image quest5Image;

    [SerializeField] private TextMeshProUGUI quest1TextReward;
    [SerializeField] private TextMeshProUGUI quest2TextReward;
    [SerializeField] private TextMeshProUGUI quest3TextReward;
    [SerializeField] private TextMeshProUGUI quest4TextReward;
    [SerializeField] private TextMeshProUGUI quest5TextReward;

    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite gemSprite;

    private void Start()
    {
        buttonQuest.onClick.AddListener(FetchQuestData);
    }

    private void OnDestroy()
    {
        buttonQuest.onClick.RemoveAllListeners();
    }

    private void FetchQuestData()
    {
        LoadingObj.SetActive(true);
        questList.Clear();

        string url = "https://api.thirdfloorprototype.com/get/dailyquest";

        StartCoroutine(ApiManager.Instance.GetRequest(url, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                LoadingObj.SetActive(false);

                var questData = JsonUtility.FromJson<QuestResponse>(response);
                questList = questData.QuestList;
                UpdateQuestUI();

                Debug.Log(questData);
            }
            else
            {
                Debug.LogError("Failed to fetch quest data.");
            }
        }));
    }

    private void UpdateQuestUI()
    {
        if (questList.Count > 0)
        {
            SetQuestUI(quest1Text, quest1Btn, quest1Image, quest1TextReward, questList[0]);
        }

        if (questList.Count > 1)
        {
            SetQuestUI(quest2Text, quest2Btn, quest2Image, quest2TextReward, questList[1]);
        }

        if (questList.Count > 2)
        {
            SetQuestUI(quest3Text, quest3Btn, quest3Image, quest3TextReward, questList[2]);
        }

        if (questList.Count > 3)
        {
            SetQuestUI(quest4Text, quest4Btn, quest4Image, quest4TextReward, questList[3]);
        }
        if (questList.Count > 4)
        {
            SetQuestUI(quest5Text, quest5Btn, quest5Image, quest5TextReward, questList[4]);
        }
    }

    private void SetQuestUI(TextMeshProUGUI questText, Button questButton, Image questRewardImage, TextMeshProUGUI questRewardText, Quest quest)
    {
        questText.text = $"{quest.Name}\nProgress: {quest.Progress}/{quest.Quantity}";

        bool isQuestComplete = quest.Progress >= quest.Quantity;
        bool isRewardReceived = quest.RewardReceived;

        questButton.interactable = isQuestComplete && !isRewardReceived;
        questButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isRewardReceived ? "Claimed" : (isQuestComplete ? "Claim" : "Claim");

        if (!isRewardReceived && isQuestComplete)
        {
            questButton.onClick.RemoveAllListeners();
            questButton.onClick.AddListener(() => ClaimReward(quest.ID));
        }

        var rewardSprite = GetRewardSprite(quest.Reward.ItemType);
        if (rewardSprite != null)
        {
            questRewardImage.sprite = rewardSprite;
        }
        questRewardText.text = $"{quest.Reward.Quantity}";
    }


    private void ClaimReward(string questId)
    {
        LoadingObj.SetActive(true);

        string url = "https://api.thirdfloorprototype.com/set/claimquestreward";

        RewardRequest requestData = new RewardRequest
        {
            QuestID = questId
        };

        string jsonRequestData = JsonUtility.ToJson(requestData);

        Debug.Log(jsonRequestData);

        StartCoroutine(ApiManager.Instance.PostRequest(url, jsonRequestData, (response, statusCode) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                LoadingObj.SetActive(false);
                Debug.Log("Reward claimed successfully.");
                FetchQuestData(); 
            }
            else
            {
                LoadingObj.SetActive(false);
                Debug.LogError("Failed to claim reward.");
            }
        }));
    }

    private Sprite GetRewardSprite(string itemType)
    {
        switch (itemType)
        {
            case "Gold":
                return goldSprite;
            case "Gem":
                return gemSprite;
            // Add more cases for other item types if needed
            default:
                return null;
        }
    }

    public void ClosePageQuest()
    {
        StartCoroutine(LoginManager.Instance.GetUserData());
        questPage.GetComponent<OpenObjAnimation>().CloseGameObject();
    }


    [System.Serializable]
    public class RewardRequest
    {
        public string QuestID;
    }

    [System.Serializable]
    public class Quest
    {
        public string ID;
        public string Name;
        public string Description;
        public int Progress;
        public int Quantity;
        public bool RewardReceived; 
        public Reward Reward;
    }
    [System.Serializable]
    public class Reward
    {
        public string ItemType;
        public string ItemID;
        public int Quantity;
        public string EquipmentPart;
        public string BodyPart;
    }

    [System.Serializable]
    public class QuestResponse
    {
        public List<Quest> QuestList;
    }

}
