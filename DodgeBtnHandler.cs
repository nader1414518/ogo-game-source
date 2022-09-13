using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DodgeBtnHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private PlayerController playerController;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (playerController)
        {
            playerController.Dodge();
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (playerController)
        {
            playerController.StopDodging();
        }
    }
}
