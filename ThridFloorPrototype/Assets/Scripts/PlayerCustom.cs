using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts
{
    public class PlayerCustom : MonoBehaviour
{
    public AnimationManager AnimationManager;

    void Start()
    {
        AnimationManager.SetState(Enums.CharacterState.Idle);
        TurnDown();
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
