using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSphere : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField]
    TrailRenderer trail;
    [SerializeField]
    float freqency;
    [SerializeField]
    float amplitude;
    [SerializeField]
    float graphicInterval = 0.1f;
    [SerializeField]
    float xSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail.enabled = false;
    }

    private void Start()
    {
        trail.enabled = true;
    }

    private void FixedUpdate()
    {
        float x = transform.localPosition.x + graphicInterval;
        float y = amplitude * Mathf.Sin(x / freqency);
        Vector2 targetPosition = new Vector2(x, y);
        Vector2 velocity = transform.TransformDirection(targetPosition - (Vector2)transform.localPosition);
        rb.velocity = velocity.normalized * xSpeed;
    }

    public float Speed
    {
        get { return xSpeed; }
        set { xSpeed = value; }
    }
}
