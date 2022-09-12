using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> items;

    

    private void InitializeIndices()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetIndex(i);
        }
    }

    private bool IsFull()
    {
        bool isFull = true;

        foreach (var item in items)
        {
            if (item.IsFree())
            {
                isFull = false;
            }
        }

        return isFull;
    }

    public void AddItem(Collectable collectable)
    {
        if (IsFull())
            return;

        int count = 0;
        foreach (var item in items)
        {
            if (item.IsFree())
            {
                Debug.Log("Added item to inventory ... ");
                Debug.Log("Slot number: " + count.ToString());
                item.InjectCollectable(collectable);
                item.SetIsFree(false);
                break;
            }
            count++;
        }

        StoreItems();
    }

    public void StoreItems()
    {
        List<string> itemsValues = new List<string>();
        foreach (var item in items)
        {
            if (!item.IsFree())
            {
                var itemCollectable = item.GetCollectable();
                if (itemCollectable != null)
                {
                    if (itemCollectable.type == CollectableType.Coin)
                    {
                        itemsValues.Add("collectable_coin");
                    }
                    else if (itemCollectable.type == CollectableType.Health)
                    {
                        itemsValues.Add("collectable_health");
                    }
                    else if (itemCollectable.type == CollectableType.PowerUp)
                    {
                        itemsValues.Add("collectable_powerup");
                    }
                }
            }
        }

        PlayerPrefsX.SetStringArray("inventory_items", itemsValues.ToArray());
    }

    public void LoadItems()
    {
        foreach (var item in items)
        {
            item.Clear();
        }

        var loadedItems = PlayerPrefsX.GetStringArray("inventory_items", null, 0);
        if (loadedItems != null)
        {
            foreach (var key in loadedItems)
            {
                if (key == "collectable_coin")
                {
                    foreach (var item in items)
                    {
                        if (item.IsFree())
                        {
                            CoinCollectable coinCollectable = new CoinCollectable();
                            coinCollectable.type = CollectableType.Coin;
                            coinCollectable.value = 1;
                            coinCollectable.collectableIcon = Resources.Load<Sprite>("coin_collectable_icon");
                            item.InjectCollectable(coinCollectable);
                            break;
                        }
                    }
                }
                else if (key == "collectable_health")
                {
                    foreach (var item in items)
                    {
                        if (item.IsFree())
                        {
                            HealthCollectable healthCollectable = new HealthCollectable();
                            healthCollectable.type = CollectableType.Health;
                            healthCollectable.value = 0.2f;
                            healthCollectable.collectableIcon = Resources.Load<Sprite>("health_collectable_icon");
                            item.InjectCollectable(healthCollectable);
                            break;
                        }
                    }
                }
                else if (key == "collectable_powerup")
                {
                    foreach (var item in items)
                    {
                        if (item.IsFree())
                        {
                            PowerUpCollectable powerupCollectable = new PowerUpCollectable();
                            powerupCollectable.type = CollectableType.PowerUp;
                            powerupCollectable.value = 1.0f;
                            powerupCollectable.collectableIcon = Resources.Load<Sprite>("powerup_collectable_icon");
                            item.InjectCollectable(powerupCollectable);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            ClearItems();
        }
    }

    private void ClearItems()
    {
        foreach (var item in items)
        {
            item.Clear();
        }
    }

    private void OnEnable()
    {
        InitializeIndices();

        StartCoroutine(loadItems());
    }

    IEnumerator loadItems()
    {
        yield return new WaitForSeconds(3.0f);

        LoadItems();
    }

    //private void OnDisable()
    //{
    //    StoreItems();
    //}

    //private void OnApplicationQuit()
    //{
    //    StoreItems();
    //}

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (!focus)
    //    {
    //        StoreItems();
    //    }
    //    else
    //    {
    //        LoadItems();
    //    }
    //}
}
