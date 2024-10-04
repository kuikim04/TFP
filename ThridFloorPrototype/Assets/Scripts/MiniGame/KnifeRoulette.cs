using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeRoulette : MonoBehaviour
{
    public bool hasCollided = false;
    private RouletteSlot lastCollidedSlot = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;

        if (collision.gameObject.CompareTag("Roulette"))
        {
            hasCollided = true;

            collision.transform.GetComponent<RouletteGame>().StopWheel();
            Debug.Log("STOP");

            transform.SetParent(collision.transform);
            GetComponent<Rigidbody2D>().isKinematic = true;

            if (lastCollidedSlot != null)
            {
                int rewardValue = lastCollidedSlot.valueSlot;
                collision.transform.GetComponent<RouletteGame>().textValueReward.text = $"x{rewardValue}";
                collision.transform.GetComponent<RouletteGame>().isEndGame = true;
                collision.transform.GetComponent<RouletteGame>().EndGame();
                Debug.Log("Final Reward Value: " + rewardValue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<RouletteSlot>(out var rouletteSlot))
        {
            lastCollidedSlot = rouletteSlot;
            Debug.Log("Detected Slot: " + rouletteSlot.valueSlot);
        }
        else
        {
            Debug.Log("RouletteSlot component not found on collided object.");
        }
    }
}
