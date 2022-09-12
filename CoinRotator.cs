using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y + rotationSpeed, this.transform.localEulerAngles.z);
    }
}
