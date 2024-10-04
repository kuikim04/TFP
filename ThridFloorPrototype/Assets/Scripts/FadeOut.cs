using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 2f;

    void Start()
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, fadeDuration).OnComplete(OnFadeComplete);

    }

    void OnFadeComplete()
    {
        canvasGroup.alpha = 0;
    }
}
