using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite collectableIcon;
}


public enum CollectableType
{
    Coin,
    Health,
    PowerUp
}