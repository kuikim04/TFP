using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using TMPro;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class SpecialEnemy : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        public string numberEnemy;

        [SerializeField] private TextMeshProUGUI textNumber;
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Sprite[] imagheSpecialItem; // 0 = +, 1 = -, 2 = *, 3 = /

        bool isTaped = false;

        [SerializeField] private AudioClip deadSound;
        private void Start()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }

            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
                _canvas.worldCamera = Camera.main;
            }
            else
            {
                Debug.LogError("No Canvas component found. Please attach this script to a GameObject with a Canvas component.");
            }

            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            ApplySpecialNumberEffect(numberEnemy);
        }
        private void OnMouseDown()
        {
            if (!isTaped && !playerController.isMoving)
            {
                if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup1))
                {
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup2))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 2 yet. Group 1 is not cleared.");
                        return;
                    }
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup3))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 3 yet. Group 1 and Group 2 are not cleared.");
                        return;
                    }
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup4))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 4 yet. Group 1, Group 2, and Group 3 are not cleared.");
                        return;
                    }
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup5))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 5 yet. Group 1, Group 2, Group 3, and Group 4 are not cleared.");
                        return;
                    }
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup6))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0
                        || GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0
                        || GameManager.Instance.EnemyGroup5.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 6 yet. Group 1, Group 2, Group 3, Group 4, and Group 5 are not cleared.");
                        return;
                    }
                    EnemyAction();
                }
                else if (GameManager.Instance.IsInGroup(this.transform, GameManager.Instance.EnemyGroup7))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0 ||
                        GameManager.Instance.EnemyGroup5.Count > 0 || GameManager.Instance.EnemyGroup6.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 7 yet. Group 1, Group 2, Group 3, Group 4, Group 5, and Group 6 are not cleared.");
                        return;
                    }
                    EnemyAction();
                }
            }
        }
        private void ApplySpecialNumberEffect(string specialNumber)
        {
            SpriteRenderer spriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (specialNumber.Contains("*"))
            {
                spriteRenderer.sprite = imagheSpecialItem[2]; 
            }
            else if (specialNumber.Contains("/"))
            {
                spriteRenderer.sprite = imagheSpecialItem[3];
            }
            else if (specialNumber.Contains("-"))
            {
                spriteRenderer.sprite = imagheSpecialItem[1];
            }
            else if (specialNumber.Contains("+"))
            {
                spriteRenderer.sprite = imagheSpecialItem[0];
            }

            string displayText = specialNumber.Replace("*", "x").Replace("/", "÷");
            textNumber.text = displayText;
        }

        private void EnemyAction()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.SetSpecialTarget(transform, numberEnemy);
                    isTaped = true;
                }
            }
        }
        public void Death()
        {
            textNumber.gameObject.SetActive(false);
            StartCoroutine(DestroyAfterDelay(0.2f));
        }
        IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RemoveEnemyFromList(gameObject);
            }

            SoundManager.Instance.PlayVFX(deadSound);
            Destroy(gameObject);
        }


    }
}
