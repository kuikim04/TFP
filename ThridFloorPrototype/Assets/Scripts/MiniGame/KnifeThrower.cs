using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrower : MonoBehaviour
{
    public UiManagerMinigame1 uiManager;

    public int ScoreMiniGame = 0;
    public int TotalKnife = 15;

    public GameObject KnifePrefab; 
    public Transform SpawnPoint;   
    public float ThrowForce = 10f;

    [SerializeField] AudioClip shootSound;
    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UiManagerMinigame1>();
        }
    }

    public void ThrowKnife()
    {
        if (TotalKnife <= 0)
        {
            return; 
        }

        SoundManager.Instance.PlayVFX(shootSound);

        GameObject knife = Instantiate(KnifePrefab, SpawnPoint.position, SpawnPoint.rotation);
        Rigidbody2D rb = knife.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * ThrowForce, ForceMode2D.Impulse);
        knife.GetComponent<Knife>().uiManager = uiManager;

        uiManager.DecreaseKnifeCount();
    }
}
