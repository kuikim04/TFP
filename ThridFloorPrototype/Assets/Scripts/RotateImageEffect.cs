using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImageEffect : MonoBehaviour
{
    [SerializeField] private RectTransform itemImage; 
    [SerializeField] private float rotateDuration = 2f; 
    [SerializeField] private int loops = -1; 

    private void Start()
    {
        if (itemImage == null)
        {
            itemImage = GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        PlayRotateEffect();
    }
    private void OnDisable()
    {
        StopRotateEffect();
    }

    public void PlayRotateEffect()
    {
        itemImage.DORotate(new Vector3(0, 0, 360), rotateDuration, RotateMode.FastBeyond360)
                 .SetLoops(loops, LoopType.Restart)
                 .SetEase(Ease.Linear); 
    }

    public void StopRotateEffect()
    {
        itemImage.DOKill(); 
    }
}
