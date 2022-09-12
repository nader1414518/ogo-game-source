using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarController : MonoBehaviour
{
    private void RotateTowards()
    {
        Vector3 relativePos = Camera.main.transform.position - this.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, rotation.eulerAngles.y, this.transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        RotateTowards();
    }
}
