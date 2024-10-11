using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using Assets.FantasyMonsters.Scripts;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class TapGameController : MonoBehaviour
    {
        [SerializeField] private Character4D Character;
        [SerializeField] private GameObject[] bossPrefabs; //18 - 0 
        [SerializeField] private Transform bossTransform; //18 - 0 

        [SerializeField] private AnimationManager PlayerAnimationManager;
        [SerializeField] private Monster EnemyAnimationManager;

        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI ResultTextNameItem;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI countdownText;

        [SerializeField] private Button tapButton;
        [SerializeField] private Slider progressBar;

        [SerializeField] private GameObject ResultPanel;
        [SerializeField] private GameObject ResultImageLose;

        [SerializeField] private Image imgResultPanel;
        [SerializeField] private Image imgResultItemRecive;
        [SerializeField] private Sprite[] resultSpritePanel;

        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform enemyTransform;

        private float gameTime = 10.0f;

        private bool gameActive = false;
        private bool isLose = false;

        private float progressIncrement = 0.02f;

        private float botTapSpeedMin = 0.3f;
        private float botTapSpeedMax = 0.5f;


        [SerializeField] private AudioClip[] tapSound;
        [SerializeField] private AudioClip[] tapSoundArrow;

        [SerializeField] private AudioClip deadSound;

        [SerializeField] private RectTransform winIndicator;

        [SerializeField] private Image bg;
        [SerializeField] private Sprite[] bgRegion1;
        [SerializeField] private Sprite[] bgRegion2;
        [SerializeField] private Sprite[] bgRegion3;
        [SerializeField] private Sprite[] bgRegion4;

        [SerializeField] private Image panelResultImgRewardType;
        [SerializeField] private TextMeshProUGUI textResultReward;

        [SerializeField] GameObject[] regionFloors;

        bool scoreSent = false;

        [SerializeField] private GameObject tryAgainBtn; 
        [SerializeField] private GameObject nextBtn; 
        
        private int lastPlayedRegion;
        private int lastPlayedStage;

        void Start()
        {
            AdsManager.Instance.InCreaseAds();

            lastPlayedRegion = DataCenter.Instance.InitialRegion;
            lastPlayedStage = DataCenter.Instance.InitialStage;

            SetActiveFloor(DataCenter.Instance.Region);
            SpawnBoss();

            tapButton.onClick.AddListener(PlayerTapped);
            progressBar.value = 0.5f;

            PositionWinIndicator();
            StartCoroutine(StartCountdown());
            SetBackgroundByRegion();

        }
        private void SpawnBoss()
        {
            int region = DataCenter.Instance.Region;
            int stage = DataCenter.Instance.Stage;

            int prefabIndex = GetBossPrefabIndex(region, stage);

            if (prefabIndex >= 0 && prefabIndex < bossPrefabs.Length)
            {
                GameObject boss = Instantiate(bossPrefabs[prefabIndex], bossTransform.position, Quaternion.identity);
                EnemyAnimationManager = boss.GetComponent<Monster>();
                enemyTransform = boss.transform;
            }
            else
            {
                Debug.LogError("Invalid boss prefab index: " + prefabIndex);
            }
        }

        private int GetBossPrefabIndex(int region, int stage)
        {
            if (region == 1)
            {
                if (stage <= 5) return 0;
                if (stage <= 10) return 1;
                if (stage <= 15) return 2;
                if (stage <= 21) return 3;
                if (stage <= 29) return 4;
            }
            else if (region == 2)
            {
                if (stage <= 5) return 5;
                if (stage <= 10) return 6;
                if (stage <= 15) return 7;
                if (stage <= 21) return 8;
                if (stage <= 29) return 9;
                if (stage <= 35) return 10;
            }
            else if (region == 3)
            {
                if (stage <= 5) return 11;
                if (stage <= 10) return 12;
                if (stage <= 18) return 13;
                if (stage <= 26) return 14;
                if (stage <= 35) return 15;
            }
            else if (region == 4)
            {
                if (stage <= 5 || stage <= 10 || stage <= 18) return 16;
                if (stage <= 26 || stage <= 35) return 17;
            }

            return -1; 
        }

        void SetActiveFloor(int activeRegion)
        {
            for (int i = 0; i < regionFloors.Length; i++)
            {
                regionFloors[i].SetActive(i == activeRegion - 1);
            }
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

        void PositionWinIndicator()
        {
            float targetPosition = 0.9f;
            Vector3 indicatorPosition = progressBar.fillRect.localPosition;
            indicatorPosition.x = progressBar.fillRect.rect.width * targetPosition;
            winIndicator.localPosition = indicatorPosition;
        }

        void PlayerTapped()
        {
            if (gameActive)
            {
                switch (Character.WeaponType)
                {
                    case WeaponType.Melee1H:
                    case WeaponType.Melee2H:
                        SoundManager.Instance.PlayRandomVFX(tapSound);
                        break;
                    case WeaponType.Bow:
                        SoundManager.Instance.PlayRandomVFX(tapSoundArrow);
                        break;
                    default:
                        SoundManager.Instance.PlayRandomVFX(tapSound);
                        break;
                }
                UpdateProgressBar(progressIncrement);
                PlayerAnimationManager.Attack();
            }
        }

        IEnumerator BotTapRoutine()
        {
            while (gameActive)
            {
                EnemyAnimationManager.Attack();
                float botTapSpeed = UnityEngine.Random.Range(botTapSpeedMin, botTapSpeedMax);

                yield return new WaitForSeconds(botTapSpeed);
                if (gameActive)
                {
                    UpdateProgressBar(-progressIncrement);
                }
            }
        }

        IEnumerator GameTimer()
        {
            float timeRemaining = gameTime;
            while (timeRemaining > 0 && gameActive)
            {
                timeRemaining -= Time.deltaTime;
                timeText.text = "Time: " + Mathf.Max(timeRemaining, 0).ToString("F2");
                yield return null;
            }
            gameActive = false;
            DetermineWinner();
        }

        IEnumerator StartCountdown()
        {
            float countdownTime = 3.0f;
            while (countdownTime > 0)
            {
                StartMoveCharacters();
                countdownText.text = "Starting in: " + Mathf.Ceil(countdownTime).ToString();
                yield return new WaitForSeconds(1.0f);
                countdownTime -= 1.0f;
            }

            countdownText.text = "";
            gameActive = true;

            PlayerAnimationManager.SetState(Enums.CharacterState.Idle);
            EnemyAnimationManager.SetState(MonsterState.Idle);

            StartCoroutine(BotTapRoutine());
            StartCoroutine(GameTimer());
        }

        void StartMoveCharacters()
        {
            float moveDuration = 1.5f;
            playerTransform.DOMoveX(playerTransform.position.x + 0.2f, moveDuration).SetEase(Ease.Linear);
            enemyTransform.DOMoveX(enemyTransform.position.x - 0.2f, moveDuration).SetEase(Ease.Linear);
            PlayerAnimationManager.SetState(Enums.CharacterState.Walk);
            EnemyAnimationManager.SetState(MonsterState.Walk);
        }

        void UpdateProgressBar(float increment)
        {
            float targetValue = Mathf.Clamp(progressBar.value + increment, 0.0f, 1.0f);
            float duration = 0.3f;

            DOTween.To(() => progressBar.value, x => progressBar.value = x, targetValue, duration)
                .SetEase(Ease.Linear);

            if (targetValue >= 1.0f)
            {
                gameActive = false;
                DetermineWinner();
            }
            else if (targetValue <= 0.0f)
            {
                gameActive = false;
                DetermineWinner();
            }
        }

        void EndGameWinTimeOut()
        {
            gameActive = false;

            PlayerAnimationManager.SetState(CharacterState.Dance);
            EnemyAnimationManager.SetState(MonsterState.Death);
            imgResultPanel.sprite = resultSpritePanel[0];

            isLose = false;

            ResultImageLose.SetActive(false);
            ResultTextNameItem.gameObject.SetActive(true);
            imgResultItemRecive.gameObject.SetActive(true);
            resultText.text = "Win";

            SoundManager.Instance.PlayVFX(deadSound);

            StartCoroutine(EndGame());
        }

        void DetermineWinner()
        {
            if (!scoreSent)
            {
                if (progressBar.value >= 0.9f)
                {
                    scoreSent = true;

                    DataCenter.Instance.SendScoreDataToApi(lastPlayedRegion,
                        lastPlayedStage,
                        3, 0,
                        EndGameWinTimeOut, textResultReward, panelResultImgRewardType);

                    AdvanceToNextStage();
                }
                else if (progressBar.value < 0.9f)  
                {
                    PlayerAnimationManager.SetState(Enums.CharacterState.Death);
                    EnemyAnimationManager.SetState(MonsterState.Idle);
                    imgResultPanel.sprite = resultSpritePanel[1]; 

                    ResultImageLose.SetActive(true);  
                    ResultTextNameItem.gameObject.SetActive(false);
                    imgResultItemRecive.gameObject.SetActive(false);

                    isLose = true;

                    SoundManager.Instance.PlayVFX(deadSound); 
                    resultText.text = "Lose"; 

                    tryAgainBtn.SetActive(true); 
                    nextBtn.SetActive(false);  

                    StartCoroutine(EndGame());  
                }
            }
        }

        IEnumerator EndGame()
        {
            SoundManager.Instance.StopBGM();
            yield return new WaitForSeconds(1.5f); 

            switch (resultText.text)
            {
                case "Win":
                    SoundManager.Instance.VictoryVFX();
                    break;
                case "Lose":
                    SoundManager.Instance.DefeatVFX();
                    break;
            }

            ResultPanel.GetComponent<OpenObjAnimation>().OpenGameObject();
        }


        private void AdvanceToNextStage()
        {
            if (!DataCenter.Instance.IsTryAgain)
            {
                DataCenter.Instance.GetPlayerData().stage++;

                if (DataCenter.Instance.GetPlayerData().stage > 35)
                {
                    DataCenter.Instance.GetPlayerData().region++;
                    DataCenter.Instance.GetPlayerData().stage = 1;
                }

            }

            DataCenter.Instance.SendCurrentProgress(DataCenter.Instance.GetPlayerData().region,
               DataCenter.Instance.GetPlayerData().stage);
        }

        public void LoadSceneAgain()
        {
            DataCenter.Instance.IsTryAgain = false;

            DataCenter.Instance.InitialRegion = DataCenter.Instance.GetPlayerData().region;
            DataCenter.Instance.InitialStage = DataCenter.Instance.GetPlayerData().stage;

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
            if (!isLose)
            {
                DataCenter.Instance.IsTryAgain = true;
            }

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