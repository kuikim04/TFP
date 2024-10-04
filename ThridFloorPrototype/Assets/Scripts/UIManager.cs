using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.EditorScripts;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterEditorSETID characterEditor;

    public Character4D Character;
    private string filePath;

    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject playerObjShow;
    [SerializeField] private GameObject panelCustom;

    [SerializeField] private Button btnOpenPage;
    [SerializeField] private Button btnClosePage;

    [Header("Button")]
    [SerializeField] private Button BuyBuntton;
    [SerializeField] private Button SaveBuntton;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "character.json");
        LoadFromJson();

        btnOpenPage.onClick.AddListener(OpenCustomPlayer);
        btnClosePage.onClick.AddListener(CloseCustomPlayer);

        playerObj.SetActive(false);
        panelCustom.SetActive(false);

        SoundManager.Instance.PlayMusicMainMenu();
    }

    private void OnDestroy()
    {
        btnOpenPage.onClick.RemoveAllListeners();
        btnClosePage.onClick.RemoveAllListeners();
    }

    public void OpenCustomPlayer()
    {
        SoundManager.Instance.ClickSound();

        BuyBuntton.gameObject.SetActive(false);
        SaveBuntton.gameObject.SetActive(true);

        playerObjShow.SetActive(false);
        OnLoadPlayerCustom();

        playerObj.SetActive(true);
        panelCustom.SetActive(true);

        Toggle[] toggles = characterEditor.Tabs.GetComponentsInChildren<Toggle>(true);

        if (toggles.Length > 0)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                if (i == 0)
                {
                    toggles[i].isOn = true;
                }
                else
                {
                    toggles[i].isOn = false;
                }
            }
        }
        characterEditor.OnSelectTab(true);
    }

    public void LoadFromJson()
    {
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

    public void CloseCustomPlayer()
    {
        SoundManager.Instance.ClickSound();
        playerObjShow.SetActive(true);

        playerObj.SetActive(false);
        panelCustom.SetActive(false);

        OnLoadPlayerCustom();
    }

    public void OnSaveButtonClicked()
    {
        characterEditor.SaveToJson();
    }

    public void OnLoadPlayerCustom()
    {
        characterEditor.LoadFromJson();
        LoadFromJson();
    }
}
