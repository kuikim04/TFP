using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public AnimationManager AnimationManager;

        [SerializeField] private PlayerController playerController;

        public int numberEnemy;

        [SerializeField] private TextMeshProUGUI textNumber;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject killEffect;

        bool isTaped = false;

        [SerializeField] private AudioClip deadSound;
        [SerializeField] private AudioClip slashSound;
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

            AnimationManager.SetState(Enums.CharacterState.Idle);
            TurnLeft();

            textNumber.text = numberEnemy.ToString();
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
        private void EnemyAction()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.SetTarget(transform, numberEnemy);

                    if (GameManager.Instance.scoreNumber <= numberEnemy)
                    {
                        SoundManager.Instance.PlayVFX(slashSound);

                        AnimationManager.Attack();
                        isTaped = true;

                        /* GameObject endEffect = Instantiate(killEffect, transform.position, Quaternion.identity);
                         SetScaleRecursively(endEffect.transform, 3f);*/
                    }
                    else
                    {
                        AnimationManager.Hit();
                        isTaped = true;

                    }
                }
            }
        }

        public void TurnLeft()
        {
            GetComponent<Character4D>().SetDirection(Vector2.left);
        }
        public void TurnRight()
        {
            GetComponent<Character4D>().SetDirection(Vector2.right);
        }
        public void TurnUp()
        {
            GetComponent<Character4D>().SetDirection(Vector2.up);
        }
        public void TurnDown()
        {
            GetComponent<Character4D>().SetDirection(Vector2.down);
        }
        public void Show4Direction()
        {
            GetComponent<Character4D>().SetDirection(Vector2.zero);

        }
        public void Death()
        {
            textNumber.gameObject.SetActive(false);
            AnimationManager.SetState(Enums.CharacterState.Death);
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

        void SetScaleRecursively(Transform obj, float scale)
        {
            obj.localScale = new Vector3(scale, scale, scale);
            foreach (Transform child in obj)
            {
                SetScaleRecursively(child, scale);
            }
        }
    }
}
