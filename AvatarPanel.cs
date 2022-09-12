using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanel : MonoBehaviour
{
    [SerializeField]
    private Image weaponIcon;

    public void LoadWeapon(WeaponBase weapon)
    {
        if (weaponIcon)
        {
            weaponIcon.sprite = weapon.GetIcon();

            weaponIcon.GetComponent<Button>().onClick.RemoveAllListeners();
            weaponIcon.GetComponent<Button>().onClick.AddListener(delegate
            {
                // TODO: show weapon information here
                Debug.Log($"Showing information for {weapon.GetName()}... Oops, it will be implemented verrrrrrry soon!!");
            });
        }
    }
}
