using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VerticalScrollMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    [SerializeField] private RectTransform imageRect; 
    [SerializeField] private float scrollSpeed = 1.0f; 

    private Vector2 pointerStartLocalPosition;
    private Vector2 imageStartLocalPosition;

    private float imageHeight;
    private float containerHeight;

    private void Start()
    {
        imageHeight = imageRect.rect.height;
        containerHeight = GetComponent<RectTransform>().rect.height;
        ResetPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out pointerStartLocalPosition);
        imageStartLocalPosition = imageRect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var localPointerPosition))
        {
            Vector2 pointerDelta = localPointerPosition - pointerStartLocalPosition;
            Vector2 newPosition = imageStartLocalPosition + new Vector2(0, pointerDelta.y * scrollSpeed);

            float minY = (containerHeight - imageHeight) / 2;
            float maxY = (imageHeight - containerHeight) / 2;

            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            imageRect.anchoredPosition = newPosition;
        }
    }
    public void ResetPosition()
    {
        float bottomPositionY = (containerHeight - imageHeight) / 2;
        imageRect.anchoredPosition = new Vector2(imageRect.anchoredPosition.x, bottomPositionY);
    }

}
