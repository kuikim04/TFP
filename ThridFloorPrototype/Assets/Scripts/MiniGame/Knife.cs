using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public UiManagerMinigame1 uiManager;
    bool isStick = false;

    [SerializeField] AudioClip coliderCircleSound;
    [SerializeField] AudioClip coliderArrowSound;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Knife"))
        {
            if (!isStick)
            {
                SoundManager.Instance.PlayVFX(coliderArrowSound);
                uiManager.DecreaseScore(1);
                Destroy(gameObject);
            }
        }

        else if (collision.gameObject.CompareTag("Circle"))
        {
            uiManager.IncreaseScore(1);
            transform.SetParent(collision.transform);
            GetComponent<Rigidbody2D>().isKinematic = true;

            SoundManager.Instance.PlayVFX(coliderCircleSound);

            isStick = true;
        }
    }
}
