using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OpenObjAnimation : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;

    public Transform targetTransform;
    public Vector3 targetScale = new Vector3(1, 1, 1);

    [SerializeReference] bool isOnMain;
    public void OpenGameObject()
    {
        if (isOnMain)
        {
            playerObj.SetActive(false);
            SoundManager.Instance.ClickSound();
        }

        gameObject.SetActive(true);
        targetTransform.localScale = Vector3.zero;
        targetTransform.DOScale(targetScale, 0.1f);

    }


    public void CloseGameObject()
    {
        targetTransform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
        {
            if (isOnMain) 
            {
                playerObj.SetActive(true);
                SoundManager.Instance.ClickSound();
            }

            gameObject.SetActive(false);
        });

    }
}
