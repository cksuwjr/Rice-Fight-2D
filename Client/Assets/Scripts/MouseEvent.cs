using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject OnOFF;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnOFF.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnOFF.SetActive(false);
    }
}
