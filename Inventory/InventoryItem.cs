using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private Collectable collectable;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Button discardBtn;

    private bool isFree = true;

    private int index;

    private void InitializeUseDiscardEvents()
    {
        // TODO: get discard button and use buttons and assign their functions
        if (discardBtn != null)
        {
            discardBtn.onClick.RemoveAllListeners();
            discardBtn.onClick.AddListener(delegate
            {
                if (this.isFree)
                    return;

                // TODO: instantiate collectable here
                var gameMan = FindObjectOfType<GameMan>();
                if (gameMan)
                {
                    gameMan.InstantiateDroppedItem(this.collectable);
                }

                Clear();

                var invCtrl = FindObjectOfType<InventoryController>();
                if (invCtrl)
                {
                    invCtrl.StoreItems();
                    //invCtrl.LoadItems();
                }
            });
        }

        var useBtn = this.GetComponent<Button>();
        if (useBtn != null)
        {
            useBtn.onClick.RemoveAllListeners();
            useBtn.onClick.AddListener(delegate
            {
                if (this.isFree)
                    return;

                if (this.collectable.type == CollectableType.PowerUp)
                {
                    var player = FindObjectOfType<PlayerController>();
                    if (player)
                    {
                        player.IncrementPower(((PowerUpCollectable)this.collectable).value);
                    }
                }

                Clear();

                var invCtrl = FindObjectOfType<InventoryController>();
                if (invCtrl)
                {
                    invCtrl.StoreItems();
                    //invCtrl.LoadItems();
                }
            });
        }
    }

    public void InjectCollectable(Collectable collectable)
    {
        this.isFree = false;
        this.collectable = collectable;
        this.icon.sprite = collectable.collectableIcon;
        this.discardBtn.GetComponent<Image>().enabled = true;
    }

    public Collectable GetCollectable()
    {
        return this.collectable;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return this.index;
    }

    public bool IsFree()
    {
        return this.isFree;
    }

    public void SetIsFree(bool value)
    {
        this.isFree = value;
    }

    public void Clear()
    {
        this.isFree = true;
        this.collectable = null;
        this.icon.sprite = null;
        this.discardBtn.GetComponent<Image>().enabled = false;
    }

    private void OnEnable()
    {
        Clear();

        InitializeUseDiscardEvents();
    }
}
