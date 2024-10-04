using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnEnable : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup is null. Skipping fade.");
            return;
        }

        canvasGroup.alpha = 1f;

        canvasGroup.DOFade(0f, 3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                if (canvasGroup != null) // ��Ǩ�ͺ����ա�÷�����������
                {
                    DestroySelf();
                }
            });
    }

    private void DestroySelf()
    {
        if (gameObject != null) // ��Ǩ�ͺ����ѵ���ѧ��������
        {
            Destroy(gameObject);
        }
    }


}
