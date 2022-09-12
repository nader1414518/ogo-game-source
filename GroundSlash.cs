using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlash : MonoBehaviour
{
    [SerializeField]
    public float speed = 30;
    [SerializeField]
    private float slowdownRate = 0.01f;
    [SerializeField]
    private float detectingDistance = 0.1f;
    [SerializeField]
    private float destroyDelay = 5;

    private Rigidbody rb;
    private bool stopped;

    private void Start()
    {
        this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);

        if (this.GetComponent<Rigidbody>() != null)
        {
            rb = this.GetComponent<Rigidbody>();
            StartCoroutine(slowDown());
        }
        else
        {
            Debug.Log("No rigidbody ... ");
        }

        Destroy(this.gameObject, destroyDelay);
    }

    private void FixedUpdate()
    {
        if (!stopped)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);

            if (Physics.Raycast(distance, this.transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
            {
                this.transform.position = new Vector3(this.transform.position.x, hit.point.y, this.transform.position.z);
            }
            else
            {
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
            }

            Debug.DrawRay(distance, this.transform.TransformDirection(-Vector3.up * detectingDistance), Color.red);
        }
    }

    IEnumerator slowDown()
    {
        float t = 1;
        while (t > 0)
        {
            rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, t);
            t -= slowdownRate;
            yield return new WaitForSeconds(0.1f);
        }

        stopped = true;
    }
}
