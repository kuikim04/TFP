using Assets.FantasyMonsters.Scripts;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class RockPaperScissorsGame : MonoBehaviour
    {
        [SerializeField] private Monster boxAnimation;
        [SerializeField] private Character4D Character;

        public AnimationManager PlayerAnimationManager;
        public TextMeshProUGUI resultText;

        public GameObject ResultPanel;

        private Sequence imageSequence;
        public Image imgResult;
        public Image imgResultPanel;
        public Image imgResultItemRecive;

        public Sprite[] resultSprite;
        public Sprite[] resultSpritePanel;

        public Image playerHPImage;
        public Image enemyHPImage;

        private float playerHP = 1;
        private float enemyHP = 1;

        bool isSelected = false;
        bool isWinLose = false;

        [SerializeField] private AudioClip selectSoundWin;
        [SerializeField] private AudioClip selectSoundlose;
        [SerializeField] private AudioClip slashSound;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioClip deadSound;
        [SerializeField] private AudioClip openLootSound;

        #region Lose

        public GameObject ResultImageLose;


        #endregion

        [SerializeField] private Image bg;
        [SerializeField] private Sprite[] bgRegion1;
        [SerializeField] private Sprite[] bgRegion2;
        [SerializeField] private Sprite[] bgRegion3;
        [SerializeField] private Sprite[] bgRegion4;

        [SerializeField] private Image panelResultImgRewardType;
        [SerializeField] private TextMeshProUGUI textResultReward;

        [SerializeField] private GameObject tryAgainBtn;
        [SerializeField] private GameObject nextBtn;

        private int lastPlayedRegion;
        private int lastPlayedStage;

        private void Start()
        {
            boxAnimation.SetState(MonsterState.Idle);

            playerHPImage.fillAmount = playerHP;
            enemyHPImage.fillAmount = enemyHP;

            StartImageLoop();
            SetBackgroundByRegion();
        }

        private void SetBackgroundByRegion()
        {
            int currentRegion = DataCenter.Instance.GetPlayerData().region;

            switch (currentRegion)
            {
                case 1:
                    bg.sprite = GetRandomBackground(bgRegion1);
                    break;
                case 2:
                    bg.sprite = GetRandomBackground(bgRegion2);
                    break;
                case 3:
                    bg.sprite = GetRandomBackground(bgRegion3);
                    break;
                case 4:
                    bg.sprite = GetRandomBackground(bgRegion4);
                    break;
                default:
                    Debug.LogWarning("Unknown region: " + currentRegion);
                    break;
            }
        }

        private Sprite GetRandomBackground(Sprite[] backgrounds)
        {
            if (backgrounds.Length == 0)
            {
                Debug.LogError("Background array is empty.");
                return null;
            }
            int randomIndex = UnityEngine.Random.Range(0, backgrounds.Length);
            return backgrounds[randomIndex];
        }

        private void StartImageLoop()
        {
            imageSequence = DOTween.Sequence();
            imageSequence.SetAutoKill(false);

            AddImageToSequence(imageSequence, resultSprite[0], 0.1f);
            AddImageToSequence(imageSequence, resultSprite[1], 0.1f);
            AddImageToSequence(imageSequence, resultSprite[2], 0.1f);

            imageSequence.SetLoops(-1);
        }


        private void AddImageToSequence(Sequence sequence, Sprite sprite, float duration)
        {
            sequence.AppendCallback(() => imgResult.sprite = sprite)
                    .AppendInterval(duration);
        }

        public void PlayerChoice(string playerChoice)
        {
            if (!isSelected)
            {
                isSelected = true;
                StartCoroutine(ProcessBotChoice(playerChoice));
            }
        }

        private IEnumerator ProcessBotChoice(string playerChoice)
        {
            string botChoice = GetBotChoice(playerChoice);
            imgResult.sprite = GetSpriteForChoice(botChoice);

            imageSequence.Kill();

            string result = DetermineWinner(playerChoice, botChoice);
            resultText.text = $"{result}";

            yield return new WaitForSeconds(3f);

            if (!isWinLose)
            {
                PlayerAnimationManager.SetState(Enums.CharacterState.Idle);
                StartImageLoop();

                isSelected = false;
            }
        }

        private Sprite GetSpriteForChoice(string choice)
        {
            switch (choice)
            {
                case "Rock":
                    return resultSprite[1];
                case "Paper":
                    return resultSprite[2];
                case "Scissors":
                    return resultSprite[0];
                default:
                    return null;
            }
        }

        private string GetBotChoice(string playerChoice)
        {
            List<string> winOutcomes = new List<string>();
            List<string> loseOutcomes = new List<string>();
            List<string> tieOutcomes = new List<string>();

            if (playerChoice == "Rock")
            {
                winOutcomes.Add("Scissors");
                loseOutcomes.Add("Paper");
                tieOutcomes.Add("Rock");
            }
            else if (playerChoice == "Paper")
            {
                winOutcomes.Add("Rock");
                loseOutcomes.Add("Scissors");
                tieOutcomes.Add("Paper");
            }
            else if (playerChoice == "Scissors")
            {
                winOutcomes.Add("Paper");
                loseOutcomes.Add("Rock");
                tieOutcomes.Add("Scissors");
            }

            int randomOutcome = UnityEngine.Random.Range(0, 10);
            if (randomOutcome < 7)
            {
                return winOutcomes[0];
            }
            else if (randomOutcome < 8)
            {
                return tieOutcomes[0];
            }
            else
            {
                return loseOutcomes[0];
            }
        }

        private string DetermineWinner(string playerChoice, string botChoice)
        {
            if (playerChoice == botChoice)
            {
                imgResultPanel.sprite = resultSpritePanel[1];
                return "Tie";
            }
            else if ((playerChoice == "Rock" && botChoice == "Scissors") ||
                     (playerChoice == "Paper" && botChoice == "Rock") ||
                     (playerChoice == "Scissors" && botChoice == "Paper"))
            {
                enemyHP -= 0.5f;
                imgResultPanel.sprite = resultSpritePanel[0];

                UpdateHPUI();

                if (isWinLose)
                    imageSequence.Kill();

                switch (Character.WeaponType)
                {
                    case WeaponType.Melee1H:
                    case WeaponType.Melee2H:
                        SoundManager.Instance.PlayVFX(slashSound);
                        break;
                    case WeaponType.Bow:
                        SoundManager.Instance.PlayVFX(shootSound);
                        break;
                    default:
                        SoundManager.Instance.PlayVFX(slashSound);
                        break;
                }

                SoundManager.Instance.PlayVFX(selectSoundWin);

                PlayerAnimationManager.Attack();

                return "Win";
            }
            else
            {
                playerHP -= 0.5f;
                imgResultPanel.sprite = resultSpritePanel[1];

                SoundManager.Instance.PlayVFX(selectSoundlose);
                boxAnimation.Attack();

                UpdateHPUI();

                if (isWinLose)
                    PlayerAnimationManager.Die();
                else
                    PlayerAnimationManager.Hit();

                return "Lose";
            }
        }

        private void UpdateHPUI()
        {
            playerHPImage.fillAmount = playerHP;
            enemyHPImage.fillAmount = enemyHP;
            StartCoroutine(EndGameAPI());
        }

        IEnumerator EndGameAPI()
        {
            if (playerHP <= 0)
            {
                SoundManager.Instance.PlayVFX(deadSound);

                isWinLose = true;
                ResultImageLose.SetActive(true);
                imgResultItemRecive.gameObject.SetActive(false);
                panelResultImgRewardType.gameObject.SetActive(false);
                textResultReward.gameObject.SetActive(false);

                tryAgainBtn.SetActive(true);
                nextBtn.SetActive(false);  

                SoundManager.Instance.StopBGM();
                yield return new WaitForSeconds(1.5f);
                SoundManager.Instance.DefeatVFX();

                ResultPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
                yield break;
            }
            else if (enemyHP <= 0)
            {
                DataCenter.Instance.SendScoreDataToApi(DataCenter.Instance.GetPlayerData().region,
                    DataCenter.Instance.GetPlayerData().stage, 3, 0,
                    () => StartCoroutine(EndGameWin()), textResultReward, panelResultImgRewardType);

                AdvanceToNextStage();
            }
        }

        IEnumerator EndGameWin()
        {
            SoundManager.Instance.PlayVFX(openLootSound);
            boxAnimation.SetState(MonsterState.Death);

            isWinLose = true;
            ResultImageLose.SetActive(false);
            imgResultItemRecive.gameObject.SetActive(true);
            panelResultImgRewardType.gameObject.SetActive(true);
            textResultReward.gameObject.SetActive(true);


            tryAgainBtn.SetActive(true);

            SoundManager.Instance.StopBGM();
            yield return new WaitForSeconds(1.5f);
            SoundManager.Instance.VictoryVFX();

            ResultPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
            yield break;
        }

        private void AdvanceToNextStage()
        {
            lastPlayedRegion = DataCenter.Instance.GetPlayerData().region;
            lastPlayedStage = DataCenter.Instance.GetPlayerData().stage;

            DataCenter.Instance.GetPlayerData().stage++;

            if (DataCenter.Instance.GetPlayerData().stage > 35)
            {
                DataCenter.Instance.GetPlayerData().region++;
                DataCenter.Instance.GetPlayerData().stage = 1;
            }

            DataCenter.Instance.SendCurrentProgress(DataCenter.Instance.GetPlayerData().region,
                DataCenter.Instance.GetPlayerData().stage);
        }

        public void LoadSceneAgain()
        {
            SoundManager.Instance.ClickSound();

            AdsManager.Instance.ShowInterstitialAd(() =>
            {
                StartCoroutine(DataCenter.Instance.GetTypeGameFromCsv(DataCenter.Instance.Region, DataCenter.Instance.Stage, typeGame =>
                {
                    if (typeGame != -1)
                    {
                        DataCenter.Instance.LoadSceneByTypeGame(typeGame);
                    }
                }));
            });
        }

        public void LoadSceneTryAgain()
        {
            SoundManager.Instance.ClickSound();

            AdsManager.Instance.ShowInterstitialAd(() =>
            {
                StartCoroutine(DataCenter.Instance.GetTypeGameFromCsv(lastPlayedRegion, lastPlayedStage, typeGame =>
                {
                    if (typeGame != -1)
                    {
                        DataCenter.Instance.LoadSceneByTypeGame(typeGame);
                    }
                }));
            });
        }

    }
}
