using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarEditorCharacter : MonoBehaviour
{
    public ScrollRect scrollRect;  
    public GameObject content;    
    private RectTransform contentRectTransform;
    private Scrollbar verticalScrollbar;

    private void Start()
    {
        verticalScrollbar = scrollRect.verticalScrollbar;
        contentRectTransform = content.GetComponent<RectTransform>();

        UpdateScrollbarVisibility();
    }

    public void UpdateScrollbarVisibility()
    {
        float contentHeight = contentRectTransform.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        if (contentHeight > viewportHeight)
        {
            verticalScrollbar.gameObject.SetActive(true); 
        }
        else
        {
            verticalScrollbar.gameObject.SetActive(false);
        }
    }
}
