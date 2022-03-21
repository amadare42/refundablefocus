using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RefundableFocus;

public class PointerEventListener : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public event Action<PointerEventData> PointerEnter;
    public event Action<PointerEventData> PointerExit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.PointerEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.PointerExit?.Invoke(eventData);
    }
}