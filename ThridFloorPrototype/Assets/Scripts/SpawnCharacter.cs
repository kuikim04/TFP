using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.EditorScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    public Character4D Character;
    private string filePath;

    
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "character.json");
        LoadFromJson();
    }
    public void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath, Encoding.Default);
            Character.FromJson(json, silent: false);
            Debug.Log($"Loaded from {filePath}");
        }
        else
        {
            Debug.LogError("File not found");
        }
    }
}
