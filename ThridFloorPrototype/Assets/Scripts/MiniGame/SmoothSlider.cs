using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SmoothSlider : MonoBehaviour
{
    public Slider slider;
    public float targetValue;
    public float duration = 0.5f;

    void Start()
    {
        UpdateSliderSmoothly(targetValue, duration);
    }

    void UpdateSliderSmoothly(float target, float duration)
    {
        slider.DOValue(target, duration).SetEase(Ease.Linear);
    }
}
