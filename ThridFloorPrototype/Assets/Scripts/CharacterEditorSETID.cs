using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Collections;
using Assets.HeroEditor4D.Common.Scripts.Common;
using Assets.HeroEditor4D.Common.Scripts.Data;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using Assets.HeroEditor4D.InventorySystem.Scripts.Data;
using Assets.HeroEditor4D.InventorySystem.Scripts.Elements;
using Assets.HeroEditor4D.InventorySystem.Scripts;
using Assets.HeroEditor4D.InventorySystem.Scripts.Enums;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;
using System.Collections;

[Serializable]
public class ItemCheckNotOwner
{
    public string TapName;
    public string ItemId;
}

[Serializable]
public class BuyeEuipment
{
    public string ItemID;
    public string EquipmentPart;
    public string BodyPart;
}

namespace Assets.HeroEditor4D.Common.Scripts.EditorScripts
{
    public class CharacterEditorSETID : MonoBehaviour
    {
        [Header("BUY")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private MainMenuManager updateText;
        [SerializeField] private Transform perentNotice;
        [SerializeField] private Transform parentShowItemNotOwner;

        [SerializeField] private GameObject noticeNeedBuy;
        [SerializeField] private GameObject noticeNoMoney;
        [SerializeField] private GameObject noticeBuySuccess;
        [SerializeField] private GameObject prefabShowItemNotOwner;
        [SerializeField] private GameObject loadingObj;

        private string itemIdSelcted = "";

        [Header("Main")]
        public SpriteCollection SpriteCollection;
        public IconCollection IconCollection;
        public Character4D Character;
        public Transform Tabs;
        public ScrollInventory Inventory;

        [Header("Materials")]
        public Material DefaultMaterial;
        public Material EyesPaintMaterial;
        public Material EquipmentPaintMaterial;
        public Material HuePaintMaterial;

        [Header("Other")]
        public List<string> CollectionSorting;
        public List<CollectionBackground> CollectionBackgrounds;
        public string filePath;

        [Serializable]
        public class CollectionBackground
        {
            public string Name;
            public Sprite Sprite;
        }

        public List<ItemCheckNotOwner> itemChecks = new();
        private Dictionary<string, GameObject> shownItems = new Dictionary<string, GameObject>();


        public Action<Item> EquipCallback;
        private Toggle ActiveTab => Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);

        public void OnValidate()
        {
            if (Character == null)
            {
                Character = FindObjectOfType<Character4D>();
            }
        }
        public void Awake()
        {
            ItemCollection.Active = ScriptableObject.CreateInstance<ItemCollection>();
            ItemCollection.Active.SpriteCollections = new List<SpriteCollection> { SpriteCollection };
            ItemCollection.Active.IconCollections = new List<IconCollection> { IconCollection };
            ItemCollection.Active.BackgroundBrown = CollectionBackgrounds[0].Sprite;
            ItemCollection.Active.GetBackgroundCustom = item => CollectionBackgrounds.SingleOrDefault(i => i.Name == item.Icon?.Collection)?.Sprite;
        }

        public void Start()
        {
            Character.Initialize();
            OnSelectTab(true);

            filePath = Path.Combine(Application.persistentDataPath, "character.json");
        }


        public void OnSelectTab(bool value)
        {
            if (!value) return;

            Action<Item> equipAction;
            int equippedIndex;
            var tab = Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);

            ItemCollection.Active.Reset();

            List<ItemSprite> SortCollection(List<ItemSprite> collection)
            {
                return collection.OrderBy(i => CollectionSorting.Contains(i.Collection) ? CollectionSorting.IndexOf(i.Collection) : 999).ThenBy(i => i.Id).ToList();
            }

            switch (tab.name)
            {
                case "Armor":
                    {
                        var sprites = SortCollection(SpriteCollection.Armor);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Armor);
                        equippedIndex = Character.Front.Armor == null ? -1 : sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Armor.SingleOrDefault(j => j.name == "FrontBody")));
                        break;
                    }
                case "Helmet":
                    {
                        var sprites = SortCollection(SpriteCollection.Armor);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i, ".Armor.", ".Helmet.")).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Helmet);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Helmet));
                        break;
                    }
                case "Leggings":
                    {
                        string part;

                        switch (tab.name)
                        {
                            case "Vest": part = "FrontBody"; break;
                            case "Bracers": part = "FrontArmL"; break;
                            case "Leggings": part = "FrontLegL"; break;
                            default: throw new NotSupportedException(tab.name);
                        }

                        var sprites = SortCollection(SpriteCollection.Armor);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i, ".Armor.", $".{tab.name}.")).ToList();
                        equipAction = item => Character.Equip(item.Sprite, tab.name.ToEnum<EquipmentPart>());
                        equippedIndex = Character.Front.Armor == null ? -1 : sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Armor.SingleOrDefault(j => j.name == part)));
                        break;
                    }
                case "Shield":
                    {
                        var sprites = SortCollection(SpriteCollection.Shield);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Shield);
                        equippedIndex = Character.Front.Shield == null ? -1 : sprites.FindIndex(i => i.Sprites.SequenceEqual(Character.Front.Shield));
                        break;
                    }
                case "Wings":
                    {
                        var sprites = SortCollection(SpriteCollection.Wings);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Wings);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Wings));
                        break;
                    }
                case "Melee1H":
                    {
                        var sprites = SortCollection(SpriteCollection.MeleeWeapon1H);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.MeleeWeapon1H);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.PrimaryWeapon));
                        break;
                    }
                case "Melee2H":
                    {
                        var sprites = SortCollection(SpriteCollection.MeleeWeapon2H);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.MeleeWeapon2H);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.PrimaryWeapon));
                        break;
                    }
                case "Bow":
                    {
                        var sprites = SortCollection(SpriteCollection.Bow);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Bow);
                        equippedIndex = Character.Front.CompositeWeapon == null ? -1 : sprites.FindIndex(i => i.Sprites.SequenceEqual(Character.Front.CompositeWeapon));
                        break;
                    }
                case "Body":
                    {
                        var sprites = SortCollection(SpriteCollection.Body);

                        ItemCollection.Active.Items = SpriteCollection.Body.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Body);
                        equippedIndex = Character.Front.Body == null ? -1 : sprites.FindIndex(i => i.Sprites.SequenceEqual(Character.Front.Body));
                        break;
                    }
                case "Ears":
                    {
                        var sprites = SortCollection(SpriteCollection.Ears);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Ears);
                        equippedIndex = Character.Front.Ears == null ? -1 : sprites.FindIndex(i => i.Sprites.SequenceEqual(Character.Front.Ears));
                        break;
                    }
                case "Eyebrows":
                    {
                        var sprites = SortCollection(SpriteCollection.Eyebrows);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Eyebrows);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Expressions[0].Eyebrows));
                        break;
                    }
                case "Eyes":
                    {
                        var sprites = SortCollection(SpriteCollection.Eyes);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Eyes);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Expressions[0].Eyes));
                        break;
                    }
                case "Hair":
                    {
                        var sprites = SortCollection(SpriteCollection.Hair);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Hair);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Hair));
                        break;
                    }
                case "Mouth":
                    {
                        var sprites = SortCollection(SpriteCollection.Mouth);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.SetBody(item.Sprite, BodyPart.Mouth);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Expressions[0].Mouth));
                        break;
                    }
                case "Mask":
                    {
                        var sprites = SortCollection(SpriteCollection.Mask);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Mask, item.Sprite != null && item.Sprite.Tags.Contains("Paint") ? null : Color.white);
                        equippedIndex = sprites.FindIndex(i => i.Sprites.Contains(Character.Front.Mask));
                        break;
                    }
                case "Earrings":
                    {
                        var sprites = SortCollection(SpriteCollection.Earrings);

                        ItemCollection.Active.Items = sprites.Select(i => CreateFakeItemParams(new Item(i.Id), i)).ToList();
                        equipAction = item => Character.Equip(item.Sprite, EquipmentPart.Earrings);
                        equippedIndex = Character.Front.Earrings == null ? -1 : sprites.FindIndex(i => i.Sprites.SequenceEqual(Character.Front.Earrings));
                        break;
                    }
                default:
                    throw new NotImplementedException(tab.name);
            }

            var items = ItemCollection.Active.Items.Select(i => new Item(i.Id)).ToList();
            var emptyItem = new Item(null);

            ItemCollection.Active.Items.Add(CreateFakeItemParams(emptyItem, null));
            items.Insert(0, emptyItem);


            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Id != null && IconCollection.Icons.All(j => j.Id != items[i].Params.IconId))
                {
                    items.RemoveAt(i);

                    if (equippedIndex == i) equippedIndex = -1;
                    else if (equippedIndex > i) equippedIndex--;

                    i--;
                }
            }

            InventoryItem.OnLeftClick = item =>
            {
                bool isInInventory = IsItemInInventory(item.Id);
                itemIdSelcted = item.Id;
                UpdateItemChecks(tab.name, item.Id);

                CheckAndShowItemsNotInInventory();

                equipAction?.Invoke(item);
                EquipCallback?.Invoke(item);


                Debug.Log(itemIdSelcted);
            };

            Inventory.Initialize(ref items, items[equippedIndex + 1], reset: true);
            Inventory.ScrollRect.verticalNormalizedPosition = 1;

            var equipped = items.Count > equippedIndex + 1 ? items[equippedIndex + 1] : null;
        }

        private ItemParams CreateFakeItemParams(Item item, ItemSprite itemSprite, string replaceable = null, string replacement = null)
        {
            var spriteId = itemSprite?.Id;
            var iconId = itemSprite?.Id;
            var rarity = ItemRarity.Common;

            if (itemSprite != null)
            {
                switch (itemSprite.Collection)
                {
                    case "Basic":
                    case "Undead":
                        break;
                    default:
                        rarity = ItemRarity.Epic;
                        break;
                }
            }


            if (iconId != null && item.Id != null && replaceable != null && replacement != null)
            {
                iconId = iconId.Replace(replaceable, replacement);
            }


            return new ItemParams { Id = item.Id, IconId = iconId, SpriteId = spriteId, Rarity = rarity, Meta = itemSprite == null ? null : JsonConvert.SerializeObject(itemSprite.Tags) };
        }

        private void UpdateItemChecks(string tapName, string itemId)
        {
            var currentItemCheck = itemChecks.FirstOrDefault(item => item.TapName == tapName);

            if (currentItemCheck != null)
            {
                currentItemCheck.ItemId = itemId;
            }
            else
            {
                itemChecks.Add(new ItemCheckNotOwner
                {
                    TapName = tapName,
                    ItemId = itemId
                });
            }
        }

        public void CheckAndShowItemsNotInInventory()
        {
            var itemIdsInCheck = itemChecks.Select(ic => ic.ItemId).ToList();

            foreach (var itemCheck in itemChecks)
            {
                if (!IsItemInInventory(itemCheck.ItemId))
                {
                    if (shownItems.ContainsKey(itemCheck.ItemId))
                    {
                        continue; 
                    }

                    var shopItem = DataCenter.Instance.shopItems.FirstOrDefault(si => si.ItemID == itemCheck.ItemId);

                    if (shopItem != null)
                    {
                        GameObject newItem = Instantiate(prefabShowItemNotOwner, parentShowItemNotOwner, false);

                        var setItemNotOwner = newItem.GetComponent<SetItemNotOwner>();
                        if (setItemNotOwner != null)
                        {
                            var adjustedItemID = AdjustItemIDForEquipmentPart(itemCheck.TapName, itemCheck.ItemId);
                            Debug.Log(adjustedItemID);
                          
                            Sprite itemSprite = IconCollection.GetIcon(adjustedItemID);

                            string typeCurrency = shopItem.PriceType;
                            string textPrice = shopItem.Price.ToString();

                            string euipmentPart = shopItem.EquipmentPart;
                            string bodyPart = shopItem.BodyPart;

                            setItemNotOwner.InitItem(itemSprite, "Buy", typeCurrency, textPrice);

                            shownItems[itemCheck.ItemId] = newItem;

                            setItemNotOwner.btnBuy.onClick.AddListener(() => BuyItem(itemCheck.ItemId,
                                euipmentPart, bodyPart, newItem, typeCurrency, shopItem.Price));
                        }
                    }
                }
            }

            var itemIdsInCheckSet = new HashSet<string>(itemIdsInCheck);
            foreach (var shownItem in shownItems.ToList())
            {
                if (!itemIdsInCheckSet.Contains(shownItem.Key))
                {
                    Destroy(shownItem.Value);
                    shownItems.Remove(shownItem.Key);
                }
            }
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

        private bool AreAllItemsInInventory()
        {
            return itemChecks.All(ic => IsItemInInventory(ic.ItemId));
        }

        public bool IsItemInInventory(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return true;
            }

            return DataCenter.Instance.GetPlayerData().Inventory.Any(item => item.ID == itemId);
        }
       
        private void BuyItem(string itemId, string equipmentPart, string bodyPart, GameObject newItem, string typeCurrency, int itemPrice)
        {
            loadingObj.SetActive(true);

            if (!HasSufficientCurrency(typeCurrency, itemPrice))
            {
                SoundManager.Instance.CantBuyVFX();
                CreateNotice(noticeNoMoney, perentNotice);

                loadingObj.SetActive(false);
                return;
            }

            string url = "https://api.thirdfloorprototype.com/set/buyequipment";
            var buyeEuipment = new BuyeEuipment
            {
                ItemID = itemId,
                EquipmentPart = equipmentPart,
                BodyPart = bodyPart,
            };

            string jsonRequestData = JsonUtility.ToJson(buyeEuipment);
            Debug.Log(jsonRequestData);

            StartCoroutine(ApiManager.Instance.PostRequest(url, jsonRequestData, (response, statusCode) =>
            {
                HandlePurchaseResponse(response, newItem, itemId);
            }));
        }

        private bool HasSufficientCurrency(string typeCurrency, int itemPrice)
        {
            var playerData = DataCenter.Instance.GetPlayerData();

            return typeCurrency switch
            {
                "Gold" => playerData.Coin >= itemPrice,
                "Gem" => playerData.Diamond >= itemPrice,
                _ => playerData.Coin >= itemPrice,
            };
        }

        private void HandlePurchaseResponse(string response, GameObject newItem, string itemId)
        {
            if (!string.IsNullOrEmpty(response))
            {
                SoundManager.Instance.BuyFX();
                StartCoroutine(UpdateCurrencyAtBuy());

                Destroy(newItem);
                shownItems.Remove(itemId);
                loadingObj.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to buy.");
            }
        }

        private IEnumerator UpdateCurrencyAtBuy()
        {
            yield return StartCoroutine(LoginManager.Instance.GetUserData());
            CreateNotice(noticeBuySuccess, perentNotice);
        }

        public void Reset()
        {
            Character.Parts.ForEach(i => i.ResetEquipment());
            new CharacterAppearance().Setup(Character.GetComponent<Character4D>());

            ClearListItemNotOwner();
            itemChecks.Clear();
        }
        public void SaveToJson()
        {
            bool areAllItemsInInventory = AreAllItemsInInventory();

            if (!areAllItemsInInventory)
            {
                SoundManager.Instance.CantBuyVFX();
                Debug.Log("Some Item Not Have in inventory.");
                CreateNotice(noticeNeedBuy, perentNotice);
                return;
            }
            itemChecks.Clear();
            itemIdSelcted = "";
            ClearListItemNotOwner();

            string json = Character.ToJson();
            File.WriteAllText(filePath, json, Encoding.Default);
            Debug.Log($"Saved as {filePath}");

            uiManager.CloseCustomPlayer();

        }

        public void LoadFromJson()
        {
            itemChecks.Clear();
            itemIdSelcted = "";
            ClearListItemNotOwner();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath, Encoding.Default); 
                Character.FromJson(json, silent: false);
            }
            else
            {
                Debug.LogError("File not found");
            }
        }
        void ClearListItemNotOwner()
        {
            foreach(Transform Child in parentShowItemNotOwner)
            {
                Destroy(Child.gameObject);
            }
        }
        public void CreateNotice(GameObject obj, Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }

            GameObject notice = Instantiate(obj, parent, false); 
        }

    }
}