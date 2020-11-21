using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    public float speed;
    public float rotateSpeed = 10;
    float throttle;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        /*throttle += Input.GetAxis("Vertical") * Time.deltaTime;
        throttle = Mathf.Clamp(throttle, 0, 1);*/

        rb.velocity = transform.forward * speed * Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotateSpeed);
    }
}
