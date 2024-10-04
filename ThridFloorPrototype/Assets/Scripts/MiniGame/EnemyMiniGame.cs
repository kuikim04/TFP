using Assets.FantasyMonsters.Scripts;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class EnemyMiniGame : MonoBehaviour
    {
        public AnimationManager AnimationManager;
        public Monster AnimationMonster;
        [SerializeField] private bool isTap;
        void Start()
        {
            if (!isTap)
            {
                AnimationManager.SetState(Enums.CharacterState.Idle);
                TurnLeft();
            }
            else
                AnimationMonster.SetState(MonsterState.Walk);

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
    }
}
