using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackBtnHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private PlayerController player;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player)
        {
            player.Attack();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (player)
        {
            player.StopAttacking();
        }
    }
}
