using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public Character4D Character;
        public AnimationManager AnimationManager;

        private int targetNumberEnemy;
        private string targetSpecialNumberEnemy;
        public float distanceInFrontOfTarget = 1f;
        public float speed = 5f;
        private Transform target;
        private Transform targetToDestroy;

        [SerializeField] private TextMeshProUGUI textNumber;
        [SerializeField] private GameObject teleportEffect; 
        [SerializeField] private GameObject arrivalEffect;

        [SerializeField] private GameObject[] killEffectMalee;
        [SerializeField] private GameObject[] killEffectBow;

        public bool isMoving = false;

        [SerializeField] private AudioClip deadSound;
        [SerializeField] private AudioClip slashSound;
        [SerializeField] private AudioClip bowSound;

        private void Start()
        {
            textNumber.text = GameManager.Instance.scoreNumber.ToString();
        }

        public void SetTarget(Transform newTarget, int numberEnemy)
        {
            if (!isMoving)
            {
                if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup1))
                {
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup2))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 2 yet. Group 1 is not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup3))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 3 yet. Group 1 and Group 2 are not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup4))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 4 yet. Group 1, Group 2, and Group 3 are not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup5))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 5 yet. Group 1, Group 2, Group 3, and Group 4 are not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup6))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0
                        || GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0
                        || GameManager.Instance.EnemyGroup5.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 6 yet. Group 1, Group 2, Group 3, Group 4, and Group 5 are not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup7))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0 ||
                        GameManager.Instance.EnemyGroup5.Count > 0 || GameManager.Instance.EnemyGroup6.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 7 yet. Group 1, Group 2, Group 3, Group 4, Group 5, and Group 6 are not cleared.");
                        return;
                    }
                    SetTargetAndTeleport(newTarget, numberEnemy);
                }
            }
        }
        public void SetSpecialTarget(Transform newTarget, string numberEnemy)
        {
            if (!isMoving)
            {
                if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup1))
                {
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup2))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 2 yet. Group 1 is not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup3))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 3 yet. Group 1 and Group 2 are not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup4))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 4 yet. Group 1, Group 2, and Group 3 are not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup5))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 5 yet. Group 1, Group 2, Group 3, and Group 4 are not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup6))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0
                        || GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0
                        || GameManager.Instance.EnemyGroup5.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 6 yet. Group 1, Group 2, Group 3, Group 4, and Group 5 are not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
                else if (GameManager.Instance.IsInGroup(newTarget, GameManager.Instance.EnemyGroup7))
                {
                    if (GameManager.Instance.EnemyGroup1.Count > 0 || GameManager.Instance.EnemyGroup2.Count > 0 ||
                        GameManager.Instance.EnemyGroup3.Count > 0 || GameManager.Instance.EnemyGroup4.Count > 0 ||
                        GameManager.Instance.EnemyGroup5.Count > 0 || GameManager.Instance.EnemyGroup6.Count > 0)
                    {
                        Debug.Log("Cannot move to Group 7 yet. Group 1, Group 2, Group 3, Group 4, Group 5, and Group 6 are not cleared.");
                        return;
                    }
                    SetSpecialTargetAndTeleport(newTarget, numberEnemy);
                }
            }
        }

        private void SetTargetAndTeleport(Transform newTarget, int numberEnemy)
        {
            target = newTarget;
            targetNumberEnemy = numberEnemy;
            targetToDestroy = target;
            isMoving = true;

            TeleportToTarget();
        }
        private void SetSpecialTargetAndTeleport(Transform newTarget, string numberEnemy)
        {
            target = newTarget;
            targetSpecialNumberEnemy = numberEnemy;
            targetToDestroy = target;
            isMoving = true;

            TeleportToTarget();
        }

        void PlayTeleportEffect()
        {
            GameObject startEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
            SetScaleRecursively(startEffect.transform, 0.5f); 
            Destroy(startEffect, 0.5f);
        }

        void TeleportToTarget()
        {
            float targetX = target.position.x;
            float teleportX = targetX - distanceInFrontOfTarget;

            Vector3 teleportPosition = new Vector3(teleportX, target.position.y, target.position.z);

            transform.position = teleportPosition;

            GameObject endEffect = Instantiate(arrivalEffect, transform.position, Quaternion.identity);
            SetScaleRecursively(endEffect.transform, 5f);

            Destroy(endEffect, 0.1f);
            StartCoroutine(DestroyTargetAfterDelay());
        }

        IEnumerator DestroyTargetAfterDelay()
        {
            yield return new WaitForSeconds(0.1f);

            if (targetSpecialNumberEnemy != null)
            {
                ApplySpecialNumberEffect(targetSpecialNumberEnemy);
                ShowEffect();

                targetToDestroy.GetComponent<SpecialEnemy>().Death();
                targetSpecialNumberEnemy = null;

                AnimationManager.SetState(Enums.CharacterState.Idle);
            }

            else if (GameManager.Instance.scoreNumber > targetNumberEnemy)
            {
                GameManager.Instance.scoreNumber += targetNumberEnemy;
                textNumber.text = GameManager.Instance.scoreNumber.ToString();

                ShowEffect();

                targetToDestroy.GetComponent<Enemy>().Death();
                targetToDestroy = null;

                AnimationManager.SetState(Enums.CharacterState.Idle);
                GameManager.Instance.KillCount++;
            }

            else
            {
                SoundManager.Instance.PlayVFX(deadSound);

                AnimationManager.Hit();
                AnimationManager.SetState(Enums.CharacterState.Death);
                GameManager.Instance.isLose = true;
            }

            isMoving = false;
            GameManager.Instance.CheckScore();
        }

        private void ApplySpecialNumberEffect(string specialNumber)
        {
            if (specialNumber.Contains("*"))
            {
                ApplyOperation(specialNumber, '*', (score, multiplier) => score * multiplier);
            }
            else if (specialNumber.Contains("/"))
            {
                ApplyOperation(specialNumber, '/', (score, divisor) => divisor != 0 ? score / divisor : score);
            }
            else if (specialNumber.Contains("-"))
            {
                ApplyOperation(specialNumber, '-', (score, subtractor) => score - subtractor);
            }
            else if (specialNumber.Contains("+"))
            {
                ApplyOperation(specialNumber, '+', (score, adder) => score + adder);
            }

            textNumber.text = GameManager.Instance.scoreNumber.ToString();

            if (GameManager.Instance.scoreNumber <= 0)
            {
                HandleCharacterDeath();
            }
        }

        private void ApplyOperation(string specialNumber, char operation, Func<int, int, int> operationFunc)
        {
            string[] parts = specialNumber.Split(operation);
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
            {
                GameManager.Instance.scoreNumber = operationFunc(GameManager.Instance.scoreNumber, value);
            }
        }

        private void HandleCharacterDeath()
        {
            SoundManager.Instance.PlayVFX(deadSound);
            AnimationManager.Hit();
            AnimationManager.SetState(Enums.CharacterState.Death);
            GameManager.Instance.isLose = true;
        }


        void SetScaleRecursively(Transform obj, float scale)
        {
            obj.localScale = new Vector3(scale, scale, scale);
            foreach (Transform child in obj)
            {
                SetScaleRecursively(child, scale);
            }
        }

        public void ShowEffect()
        {
            switch (Character.WeaponType)
            {
                case WeaponType.Melee1H:
                case WeaponType.Melee2H:
                    SoundManager.Instance.PlayVFX(slashSound);
                    RandomAnimation();
                    EffectMalee();
                    break;
                case WeaponType.Bow:
                    SoundManager.Instance.PlayVFX(bowSound);
                    AnimationManager.Attack();
                    EffectBow();
                    break;
                default:
                    SoundManager.Instance.PlayVFX(slashSound);
                    RandomAnimation();
                    EffectMalee();
                    break;
            }
        }

        private void RandomAnimation()
        {
            int randomAnomation = UnityEngine.Random.Range(0,6);

            switch (randomAnomation)
            {
                case 0:
                    AnimationManager.Attack();
                    break;
                case 1:
                    AnimationManager.Slash2H();
                    break;
                case 2:
                    AnimationManager.Slash(true);
                    break;
                case 3:
                    AnimationManager.Jab();
                    break;
                case 4:
                    AnimationManager.HeavySlash1H();
                    break;
                case 5:
                    AnimationManager.FastStab();
                    break;
            }
        }
        private void EffectMalee()
        {
            int randomIndex = UnityEngine.Random.Range(0, killEffectMalee.Length);

            GameObject killEff = Instantiate(killEffectMalee[randomIndex], transform.position, Quaternion.identity);
            SetScaleRecursively(killEff.transform, 2f);
            Destroy(killEff, 0.5f);
        }
        private void EffectBow()
        {
            int randomIndex = UnityEngine.Random.Range(0, killEffectBow.Length);

            Vector3 newPosition = transform.position;
            newPosition.y += .5f;
            GameObject killEff = Instantiate(killEffectBow[randomIndex], newPosition, Quaternion.identity);
            SetScaleRecursively(killEff.transform, 5f);
            Destroy(killEff, 0.5f);
        }
    }
}
