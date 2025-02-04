using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityRotation : MonoBehaviour
{
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        transform.rotation.SetLookRotation(Vector3.back, rb.velocity);
    }
}
