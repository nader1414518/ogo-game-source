using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float smoothTime;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (player)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, player.transform.position + offset, ref velocity, smoothTime);
        }
    }
}
