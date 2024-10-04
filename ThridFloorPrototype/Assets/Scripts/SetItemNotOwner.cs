using Assets.HeroEditor4D.Common.Scripts.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetItemNotOwner : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI nameItemText;

    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private Sprite[] currencySprite; //0 = gold, 1 = Gem

    public Button btnBuy;

    public void InitItem(Sprite itemSprite, string itemName, string typeCurrency, string textPrice)
    {
        if (itemImage != null && itemSprite != null)
        {
            itemImage.sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning("ItemImage or itemSprite is null.");
        }

        if (nameItemText != null)
        {
            nameItemText.text = itemName;
        }
        else
        {
            Debug.LogWarning("NameItemText is null.");
        }

        if (currencyImage != null && currencySprite != null && currencySprite.Length > 0)
        {
            currencyImage.sprite = typeCurrency switch
            {
                "Gold" => currencySprite.Length > 0 ? currencySprite[0] : null,
                "Gem" => currencySprite.Length > 1 ? currencySprite[1] : null,
                _ => currencySprite.Length > 0 ? currencySprite[0] : null,
            };
        }
        else
        {
            Debug.LogWarning("CurrencyImage or currencySprite is null or empty.");
        }

        if (priceText != null)
        {
            priceText.text = textPrice;
        }
        else
        {
            Debug.LogWarning("PriceText is null.");
        }

    }

}
