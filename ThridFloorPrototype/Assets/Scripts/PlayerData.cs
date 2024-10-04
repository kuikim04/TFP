using Assets.HeroEditor4D.InventorySystem.Scripts.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string ID;
    public int UserID;
    public string Name;

    public int Coin;
    public int Diamond;

    public int region;
    public int stage;

    public List<InventoryItems> Inventory;

    public bool AdsRemove;

}