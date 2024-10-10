using Assets.HeroEditor4D.Common.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

[Serializable]
public class ConsumabeItem
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
}

[Serializable]
public class NonConsumabeItem
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
}

[Serializable]
public class SubscriptionItem
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
    public int TimeDuration;
}

public class IAPManager : MonoBehaviour, IStoreListener
{
    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private GameObject shopPage;
    IStoreController storeController;

    [Header("ConsumabeItem")]

    public ConsumabeItem ConsumabeItemGold3000;
    public ConsumabeItem ConsumabeItemGold5000;
    public ConsumabeItem ConsumabeItemGold15000;

    public ConsumabeItem ConsumabeItemDiamond1500;
    public ConsumabeItem ConsumabeItemDiamond3000;
    public ConsumabeItem ConsumabeItemDiamond10000;

    [Header("NonConsumabeItems")]
    public List<NonConsumabeItem> nonConsumableItems;

    [Header("SubscriptionItem")]
    public SubscriptionItem RemoveAds_SubscriptionItem;

    [Header("Notice")]
    [SerializeField] private Transform parentNotice;
    [SerializeField] private GameObject objNotice;

    [Header("Consumable Items Buttons")]
    public Button consumabeItemGold3000Button;
    public Button consumabeItemGold5000Button;
    public Button consumabeItemGold15000Button;
    public Button consumabeItemDiamond1500Button;
    public Button consumabeItemDiamond3000Button;
    public Button consumabeItemDiamond10000Button;

    [Header("Subscription Item Button")]
    [SerializeField] private Button removeAdsButton;

    [Header("Non Consumable Buttons")]
    [SerializeField] private Button reaperButton;
    [SerializeField] private Button halloweenButton;
    [SerializeField] private Button warpigButton;
    [SerializeField] private Button wingforceButton;
    [SerializeField] private Button reddragonButton;
    [SerializeField] private Button blackdragonButton;
    [SerializeField] private Button lightkeeperButton;
    [SerializeField] private Button samuraiButton;
    [SerializeField] private Button arkangleButton;
    [SerializeField] private Button gladiatorButton;
    [SerializeField] private Button babarianrageButton;
    [SerializeField] private Button leoarmorButton;
    [SerializeField] private Button hogkillerButton;
    [SerializeField] private Button knightofhammerButton;
    [SerializeField] private Button darkmageButton;


    public Data data;
    public Payload payload;
    public PayloadData payloadData;


    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject removeAdsBtnMain;
    private void Start()
    {
        SetupBuilder();

        consumabeItemGold3000Button.onClick.AddListener(ConsumableGold3000_Btn_Pressed);
        consumabeItemGold5000Button.onClick.AddListener(ConsumableGold5000_Btn_Pressed);
        consumabeItemGold15000Button.onClick.AddListener(ConsumableGold15000_Btn_Pressed);

        consumabeItemDiamond1500Button.onClick.AddListener(ConsumableDiamond1500_Btn_Pressed);
        consumabeItemDiamond3000Button.onClick.AddListener(ConsumableDiamond3000_Btn_Pressed);
        consumabeItemDiamond10000Button.onClick.AddListener(ConsumableDiamond10000_Btn_Pressed);

        removeAdsButton.onClick.AddListener(Subscription_Btn_Pressed);

        reaperButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_thereaper"));
        halloweenButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_halloween"));
        warpigButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_warpig"));
        wingforceButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_wingforce"));
        reddragonButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_reddragon"));
        blackdragonButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_blackdragon"));
        lightkeeperButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_lightkeeper"));
        samuraiButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_samurai"));
        arkangleButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_arkangle"));
        gladiatorButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_gladiator"));
        babarianrageButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_babarianrage"));
        leoarmorButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_leoarmor"));
        hogkillerButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_hogkiller"));
        knightofhammerButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_knightofhammer"));
        darkmageButton.onClick.AddListener(() => PurchaseNonConsumable("nonconsumabeitem_darkmage"));
    }

    void OnDestroy()
    {
        // Remove button click listeners for consumable items
        consumabeItemGold3000Button.onClick.RemoveListener(ConsumableGold3000_Btn_Pressed);
        consumabeItemGold5000Button.onClick.RemoveListener(ConsumableGold5000_Btn_Pressed);
        consumabeItemGold15000Button.onClick.RemoveListener(ConsumableGold15000_Btn_Pressed);

        consumabeItemDiamond1500Button.onClick.RemoveListener(ConsumableDiamond1500_Btn_Pressed);
        consumabeItemDiamond3000Button.onClick.RemoveListener(ConsumableDiamond3000_Btn_Pressed);
        consumabeItemDiamond10000Button.onClick.RemoveListener(ConsumableDiamond10000_Btn_Pressed);

        // Remove button click listener for subscription item
        removeAdsButton.onClick.RemoveListener(Subscription_Btn_Pressed);

        // Remove button click listeners for non-consumable items
        reaperButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_thereaper"));
        halloweenButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_halloween"));
        warpigButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_warpig"));
        wingforceButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_wingforce"));
        reddragonButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_reddragon"));
        blackdragonButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_blackdragon"));
        lightkeeperButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_lightkeeper"));
        samuraiButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_samurai"));
        arkangleButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_arkangle"));
        gladiatorButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_gladiator"));
        babarianrageButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_babarianrage"));
        leoarmorButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_leoarmor"));
        hogkillerButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_hogkiller"));
        knightofhammerButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_knightofhammer"));
        darkmageButton.onClick.RemoveListener(() => PurchaseNonConsumable("nonconsumabeitem_darkmage"));
    }

    #region setup and initialize

    public void CloseShop()
    {
        loadingObj.SetActive(true);
        StartCoroutine(CloseShopCoroutine());
    }

    private IEnumerator CloseShopCoroutine()
    {
        yield return StartCoroutine(LoginManager.Instance.GetUserData());

        shopPage.GetComponent<OpenObjAnimation>().CloseGameObject();
        loadingObj.SetActive(false);
    }


    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(ConsumabeItemGold3000.Id, ProductType.Consumable);
        builder.AddProduct(ConsumabeItemGold5000.Id, ProductType.Consumable);
        builder.AddProduct(ConsumabeItemGold15000.Id, ProductType.Consumable);
        builder.AddProduct(ConsumabeItemDiamond1500.Id, ProductType.Consumable);
        builder.AddProduct(ConsumabeItemDiamond3000.Id, ProductType.Consumable);
        builder.AddProduct(ConsumabeItemDiamond10000.Id, ProductType.Consumable);

        foreach (var nonConsumableItem in nonConsumableItems)
        {
            builder.AddProduct(nonConsumableItem.Id, ProductType.NonConsumable);
        }

        builder.AddProduct(RemoveAds_SubscriptionItem.Id, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Success");

        storeController = controller;

        foreach(NonConsumabeItem ncItem in nonConsumableItems)
        { 
            CheckNonConsumable(ncItem.Id);
        }

        CheckSubscription(RemoveAds_SubscriptionItem.Id);
    }

    public void OnProductFetched(Product product)
    {
        if (product.definition.id == ConsumabeItemGold3000.Id)
        {
            UpdateButtonPrice(consumabeItemGold3000Button, product);
        }
        else if (product.definition.id == ConsumabeItemGold5000.Id)
        {
            UpdateButtonPrice(consumabeItemGold5000Button, product);
        }
        else if (product.definition.id == ConsumabeItemGold15000.Id)
        {
            UpdateButtonPrice(consumabeItemGold15000Button, product);
        }
        else if (product.definition.id == ConsumabeItemDiamond1500.Id)
        {
            UpdateButtonPrice(consumabeItemDiamond1500Button, product);
        }
        else if (product.definition.id == ConsumabeItemDiamond3000.Id)
        {
            UpdateButtonPrice(consumabeItemDiamond3000Button, product);
        }
        else if (product.definition.id == ConsumabeItemDiamond10000.Id)
        {
            UpdateButtonPrice(consumabeItemDiamond10000Button, product);
        }
        else if (product.definition.id == RemoveAds_SubscriptionItem.Id)
        {
            UpdateButtonPrice(removeAdsButton, product);
        }
        else if(product.definition.id.StartsWith("nonconsumabeitem"))
        {
            switch (product.definition.id)
            {
                case "nonconsumabeitem_thereaper":
                    UpdateButtonPrice(reaperButton, product);
                    break;
                case "nonconsumabeitem_halloween":
                    UpdateButtonPrice(halloweenButton, product);
                    break;
                case "nonconsumabeitem_warpig":
                    UpdateButtonPrice(warpigButton, product);
                    break;
                case "nonconsumabeitem_wingforce":
                    UpdateButtonPrice(wingforceButton, product);
                    break;
                case "nonconsumabeitem_reddragon":
                    UpdateButtonPrice(reddragonButton, product);
                    break;
                case "nonconsumabeitem_blackdragon":
                    UpdateButtonPrice(blackdragonButton, product);
                    break;
                case "nonconsumabeitem_lightkeeper":
                    UpdateButtonPrice(lightkeeperButton, product);
                    break;
                case "nonconsumabeitem_samurai":
                    UpdateButtonPrice(samuraiButton, product);
                    break;
                case "nonconsumabeitem_arkangle":
                    UpdateButtonPrice(arkangleButton, product);
                    break;
                case "nonconsumabeitem_gladiator":
                    UpdateButtonPrice(gladiatorButton, product);
                    break;
                case "nonconsumabeitem_babarianrage":
                    UpdateButtonPrice(babarianrageButton, product);
                    break;
                case "nonconsumabeitem_leoarmor":
                    UpdateButtonPrice(leoarmorButton, product);
                    break;
                case "nonconsumabeitem_hogkiller":
                    UpdateButtonPrice(hogkillerButton, product);
                    break;
                case "nonconsumabeitem_knightofhammer":
                    UpdateButtonPrice(knightofhammerButton, product);
                    break;
                case "nonconsumabeitem_darkmage":
                    UpdateButtonPrice(darkmageButton, product);
                    break;
                default:
                    Debug.LogWarning("Product ID not recognized: " + product.definition.id);
                    break;
            }
        }
    }


    #endregion

    #region button clicks 

    #region button clicks for Gold

    public void ConsumableGold3000_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemGold3000.Id);
    }

    public void ConsumableGold5000_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemGold5000.Id);
    }

    public void ConsumableGold15000_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemGold15000.Id);
    }

    #endregion

    #region button clicks for Diamond

    public void ConsumableDiamond1500_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemDiamond1500.Id);
    }

    public void ConsumableDiamond3000_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemDiamond3000.Id);
    }

    public void ConsumableDiamond10000_Btn_Pressed()
    {
        storeController.InitiatePurchase(ConsumabeItemDiamond10000.Id);
    }

    #endregion

    public void PurchaseNonConsumable(string productId)
    {
        var item = nonConsumableItems.Find(x => x.Id == productId);

        if (item != null)
        {
            storeController.InitiatePurchase(item.Id);
        }
    }

    public void Subscription_Btn_Pressed()
    {
        storeController.InitiatePurchase(RemoveAds_SubscriptionItem.Id);
    }

    private void UpdateButtonPrice(Button button, Product product)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            if(product.definition.id == RemoveAds_SubscriptionItem.Id)
            {
                if (product.hasReceipt)
                {
                    buttonText.text = "Active";
                }
                else
                {
                    buttonText.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
                }
            }
            else if (nonConsumableItems.Any(item => item.Id == product.definition.id))
            {
                if (product.hasReceipt)
                {
                    buttonText.text = "Owner";
                }
                else
                {
                    buttonText.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
                }
            }
            else
            {
                buttonText.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            }

        }
    }

    #endregion


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log($"Purchase Complete {product.definition.id}");

        SendRecipt(purchaseEvent);  

        if (product.definition.id == RemoveAds_SubscriptionItem.Id)
        {
            UpdateButtonPrice(removeAdsButton, product);
        }

        return PurchaseProcessingResult.Complete;
    }


    private string GetPurchaseNoticeMessage(Product product)
    {
        if (product.definition.id == ConsumabeItemGold3000.Id)
        {
            return $"You have successfully purchased {ConsumabeItemGold3000.Name}!";
        }
        else if (product.definition.id == ConsumabeItemGold5000.Id)
        {
            return $"You have successfully purchased {ConsumabeItemGold5000.Name}!";
        }
        else if (product.definition.id == ConsumabeItemGold15000.Id)
        {
            return $"You have successfully purchased {ConsumabeItemGold15000.Name}!";
        }
        else if (product.definition.id == ConsumabeItemDiamond1500.Id)
        {
            return $"You have successfully purchased {ConsumabeItemDiamond1500.Name}!";
        }
        else if (product.definition.id == ConsumabeItemDiamond3000.Id)
        {
            return $"You have successfully purchased {ConsumabeItemDiamond3000.Name}!";
        }
        else if (product.definition.id == ConsumabeItemDiamond10000.Id)
        {
            return $"You have successfully purchased {ConsumabeItemDiamond10000.Name}!";
        }
        else if (product.definition.id.StartsWith("nonconsumabeitem"))
        {
            switch (product.definition.id)
            {
                case "nonconsumabeitem_thereaper":
                    return "You have unlocked The Reaper!";
                case "nonconsumabeitem_halloween":
                    return "You have unlocked Halloween!";
                case "nonconsumabeitem_warpig":
                    return "You have unlocked War Pig!";
                case "nonconsumabeitem_wingforce":
                    return "You have unlocked Wing Force!";
                case "nonconsumabeitem_reddragon":
                    return "You have unlocked Red Dragon!";
                case "nonconsumabeitem_blackdragon":
                    return "You have unlocked Black Dragon!";
                case "nonconsumabeitem_lightkeeper":
                    return "You have unlocked Light Keeper!";
                case "nonconsumabeitem_samurai":
                    return "You have unlocked Samurai!";
                case "nonconsumabeitem_arkangle":
                    return "You have unlocked Ark Angle!";
                case "nonconsumabeitem_gladiator":
                    return "You have unlocked Gladiator!";
                case "nonconsumabeitem_babarianrage":
                    return "You have unlocked Babarian Rage!";
                case "nonconsumabeitem_leoarmor":
                    return "You have unlocked Leo Armor!";
                case "nonconsumabeitem_hogkiller":
                    return "You have unlocked Hog Killer!";
                case "nonconsumabeitem_knightofhammer":
                    return "You have unlocked Knight Of Hammer!";
                case "nonconsumabeitem_darkmage":
                    return "You have unlocked Dark Mage!";
                default:
                    Debug.LogWarning("Item ID not recognized.");
                    return "You have unlocked a new item!";
            }
        }
        else if (product.definition.id == RemoveAds_SubscriptionItem.Id)
        {
            return "Ads have been removed. Thank you for your purchase!";
        }
        else
        {
            return "Purchase complete, but product ID not recognized.";
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == ConsumabeItemGold3000.Id)
        {
            Debug.Log("Purchase of Gold 3000 complete!");
        }
        else if (product.definition.id == ConsumabeItemGold5000.Id)
        {
            Debug.Log("Purchase of Gold 5000 complete!");
        }
        else if (product.definition.id == ConsumabeItemGold15000.Id)
        {
            Debug.Log("Purchase of Gold 15000 complete!");
        }
        else if (product.definition.id == ConsumabeItemDiamond1500.Id)
        {
            Debug.Log("Purchase of Diamond 1500 complete!");
        }
        else if (product.definition.id == ConsumabeItemDiamond3000.Id)
        {
            Debug.Log("Purchase of Diamond 3000 complete!");
        }
        else if (product.definition.id == ConsumabeItemDiamond10000.Id)
        {
            Debug.Log("Purchase of Diamond 10000 complete!");
        }
        else if (product.definition.id == RemoveAds_SubscriptionItem.Id)
        {
            Debug.Log("Subscription purchase complete!");
            removeAdsBtnMain.SetActive(false);
        }
        else if (product.definition.id.StartsWith("nonconsumabeitem"))
        {
            switch (product.definition.id)
            {
                case "nonconsumabeitem_thereaper":
                    break;
                case "nonconsumabeitem_halloween":
                    break;
                case "nonconsumabeitem_warpig":
                    break;
                case "nonconsumabeitem_wingforce":
                    break;
                case "nonconsumabeitem_reddragon":
                    break;
                case "nonconsumabeitem_blackdragon":
                    break;
                case "nonconsumabeitem_lightkeeper":
                    break;
                case "nonconsumabeitem_samurai":
                    break;
                case "nonconsumabeitem_arkangle":
                    break;
                case "nonconsumabeitem_gladiator":
                    break;
                case "nonconsumabeitem_babarianrage":
                    break;
                case "nonconsumabeitem_leoarmor":
                    break;
                case "nonconsumabeitem_hogkiller":
                    break;
                case "nonconsumabeitem_knightofhammer":
                    break;
                case "nonconsumabeitem_darkmage":
                    break;
                default:
                    Debug.LogWarning("Item ID not recognized.");
                    break;
            }
        }
    }



    #region error handeling

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase Failed for {product.definition.id}: {failureReason}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"Initialize Failed {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"Initialize Failed {error}{message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        Debug.Log($"Purchase failed for product: {product.definition.id}, Reason: {description.reason}, Message: {description.message}");    
    }

    #endregion

    void CheckNonConsumable(string id)
    {
        if (storeController != null)
        {
            var product = storeController.products.WithID(id);

            if (product != null)
            {
                if (product.hasReceipt)
                {
                    switch (product.definition.id)
                    {
                        case "nonconsumabeitem_thereaper":
                            SetButtonInteractable(reaperButton, false);
                            break;
                        case "nonconsumabeitem_halloween":
                            SetButtonInteractable(halloweenButton, false);
                            break;
                        case "nonconsumabeitem_warpig":
                            SetButtonInteractable(warpigButton, false);
                            break;
                        case "nonconsumabeitem_wingforce":
                            SetButtonInteractable(wingforceButton, false);
                            break;
                        case "nonconsumabeitem_reddragon":
                            SetButtonInteractable(reddragonButton, false);
                            break;
                        case "nonconsumabeitem_blackdragon":
                            SetButtonInteractable(blackdragonButton, false);
                            break;
                        case "nonconsumabeitem_lightkeeper":
                            SetButtonInteractable(lightkeeperButton, false);
                            break;
                        case "nonconsumabeitem_samurai":
                            SetButtonInteractable(samuraiButton, false);
                            break;
                        case "nonconsumabeitem_arkangle":
                            SetButtonInteractable(arkangleButton, false);
                            break;
                        case "nonconsumabeitem_gladiator":
                            SetButtonInteractable(gladiatorButton, false);
                            break;
                        case "nonconsumabeitem_babarianrage":
                            SetButtonInteractable(babarianrageButton, false);
                            break;
                        case "nonconsumabeitem_leoarmor":
                            SetButtonInteractable(leoarmorButton, false);
                            break;
                        case "nonconsumabeitem_hogkiller":
                            SetButtonInteractable(hogkillerButton, false);
                            break;
                        case "nonconsumabeitem_knightofhammer":
                            SetButtonInteractable(knightofhammerButton, false);
                            break;
                        case "nonconsumabeitem_darkmage":
                            SetButtonInteractable(darkmageButton, false);
                            break;
                        default:
                            Debug.LogWarning($"Item with ID {product.definition.id} is not recognized.");
                            break;
                    }
                }
                else
                {
                    Debug.Log($"{product.definition.id} has no receipt, item not purchased.");
                }
            }
        }
    }

    private void SetButtonInteractable(Button button, bool interactable)
    {
        button.interactable = interactable;
        if (button.transform.parent != null)
        {
            button.transform.parent.GetComponent<Button>().interactable = interactable;
        }
    }

    void CheckSubscription(string id)
    {
        var subProduct = storeController.products.WithID(id);
        if (subProduct != null)
        {
            try
            {
                if (subProduct.hasReceipt)
                {
                    var subManager = new SubscriptionManager(subProduct, null);
                    var info = subManager.getSubscriptionInfo();


                    if (info.isSubscribed() == Result.True)
                    {
                        RemoveAds();
                        print("We are subscribed");
                    }
                    else
                    {
                        ShowAds();
                        print("Un subscribed");
                    }
                }
                else
                {
                    print("receipt not found !!");
                }
            }
            catch (Exception)
            {

                print("It only work for Google store, app store, amazon store");
            }
        }
        else
        {
            print("product not found !!");
        }
    }


    #region extra 

    #region Ads Sub
    void RemoveAds()
    {
        DisplayAds(false);
    }
    void ShowAds()
    {
        DisplayAds(true);
    }
    void DisplayAds(bool x)
    {
        if (!x)
        {
            removeAdsBtnMain.SetActive(false);
        }
        else
        {
            removeAdsBtnMain.SetActive(true);
        }
    }
    #endregion

    #endregion

    private void CreateNotice(string message)
    {
        foreach (Transform child in parentNotice)
        {
            Destroy(child.gameObject);
        }

        var notice = Instantiate(objNotice, parentNotice, false);
        if (notice.transform.GetChild(1).TryGetComponent(out TextMeshProUGUI textComponent))
        {
            textComponent.text = message;
        }
    }

    private bool IsItemInInventory(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return true;
        }

        return DataCenter.Instance.GetPlayerData().Inventory.Any(item => item.ID == itemId);
    }


    private void SendRecipt(PurchaseEventArgs purchaseEvent)
    {
        loadingObj.SetActive(true);

        string url = "https://api.thirdfloorprototype.com/set/inapppurchase";

        var product = purchaseEvent.purchasedProduct;
        string receipt = product.receipt;

        Data data = JsonUtility.FromJson<Data>(receipt);
        Payload payload = JsonUtility.FromJson<Payload>(data.Payload);
        PayloadData payloadData = JsonUtility.FromJson<PayloadData>(payload.json);

        PayloadData requestData = new PayloadData
        {
            orderId = payloadData.orderId,
            packageName = payloadData.packageName,
            productId = payloadData.productId,
            purchaseTime = payloadData.purchaseTime,
            purchaseState = payloadData.purchaseState,
            purchaseToken = payloadData.purchaseToken,
            quantity = payloadData.quantity,
            acknowledged = payloadData.acknowledged
        };

        string jsonRequestData = JsonUtility.ToJson(requestData);

        Debug.Log($"JSON Request Data: {jsonRequestData}");

        StartCoroutine(ApiManager.Instance.PostRequest(url, jsonRequestData, (response, statusCode) =>
        {
            Debug.Log($"API Response: {response}");
            Debug.Log($"HTTP Status Code: {statusCode}");

            if (statusCode == 200)
            {
                string noticeMessage = GetPurchaseNoticeMessage(product);
                CreateNotice(noticeMessage);

                Debug.Log("Payment successful.");
            }
            else if (statusCode >= 300 && statusCode < 400)
            {
                CreateNotice("Payment failed.");
                Debug.Log("Payment failed.");
            }
            else
            {
                Debug.LogError("Unexpected error occurred.");
            }
        }));

        loadingObj.SetActive(false);
    }

}

[Serializable]
public class SkuDetails
{
    public string productId;
    public string type;
    public string title;
    public string name;
    public string iconUrl;
    public string description;
    public string price;
    public long price_amount_micros;
    public string price_currency_code;
    public string skuDetailsToken;
}

[Serializable]
public class PayloadData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}

[Serializable]
public class Payload
{
    public string json;
    public string signature;
    public List<SkuDetails> skuDetails;
    public PayloadData payloadData;
}

[Serializable]
public class Data
{
    public string Payload;
    public string Store;
    public string TransactionID;
}

