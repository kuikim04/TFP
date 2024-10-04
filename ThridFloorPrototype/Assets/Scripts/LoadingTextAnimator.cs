using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTextAnimator : MonoBehaviour
{
    public TextMeshProUGUI loadingText; 
    private string baseText = "Loading";
    private int dotCount = 0;
    private Tween dotTween;

    void OnEnable()
    {
        AnimateLoadingText();
    }

    void OnDisable()
    {
        if (dotTween != null)
        {
            dotTween.Kill();
        }
    }

    void AnimateLoadingText()
    {
        dotCount = 0;

        dotTween = DOTween.To(() => dotCount, x => dotCount = x, 3, 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .OnUpdate(() =>
            {
                loadingText.text = baseText + new string('.', dotCount);
            });
    }
}
