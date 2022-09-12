using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarObject : MonoBehaviour
{
    [SerializeField]
    private GameObject rightHandNode;

    [SerializeField]
    private GameObject currentWeaponObject;

    public void SetRightHandNode(GameObject value)
    {
        this.rightHandNode = value;
    }

    public GameObject GetRightHandNode()
    {
        return this.rightHandNode;
    }

    public void PlaceWeapon(WeaponBase weapon)
    {
        StartCoroutine(placeWeapon(weapon));
    }

    IEnumerator placeWeapon(WeaponBase weapon)
    {
        try
        {
            Destroy(currentWeaponObject.gameObject);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }

        weapon.gameObject.layer = GetRightHandNode().layer;

        yield return new WaitForSeconds(0.1f);

        currentWeaponObject = Instantiate(weapon.gameObject, GetRightHandNode().transform);
        //currentWeaponObject.layer = GetRightHandNode().layer;

        // TODO: apply weapon placement parameters
        currentWeaponObject.transform.localPosition = Vector3.zero + weapon.GetPositionOffset();
        currentWeaponObject.transform.localEulerAngles = weapon.GetRotationOffset();
    }
}
