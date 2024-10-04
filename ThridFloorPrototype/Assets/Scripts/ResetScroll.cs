using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    private void OnEnable()
    {
        scroll.verticalNormalizedPosition = 1;
    }
}
