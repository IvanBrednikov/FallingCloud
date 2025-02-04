using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedWind : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animatorSize1;
    [SerializeField] Animator animatorSize2;
    [SerializeField] Animator animatorSize3;
    [SerializeField] float velocityTreshold = 15;
    [SerializeField] CloudSize size;
    [SerializeField] float interpolationRatio = 0.0015f;
    void Update()
    {
        bool effectState = rb.velocity.magnitude >= velocityTreshold;
        animatorSize1.SetBool("Speed wind is active", effectState);
        animatorSize2.SetBool("Speed wind is active", effectState);
        animatorSize3.SetBool("Speed wind is active", effectState);

        if (size.GetSize == CloudSize.ESize.Small)
        {
            animatorSize2.gameObject.SetActive(false);
            animatorSize3.gameObject.SetActive(false);
        }
        else
        if (size.GetSize == CloudSize.ESize.Medium)
        {
            animatorSize2.gameObject.SetActive(true);
            animatorSize3.gameObject.SetActive(false);
        }
        else
        if (size.GetSize == CloudSize.ESize.Big)
        {
            animatorSize2.gameObject.SetActive(true);
            animatorSize3.gameObject.SetActive(true);
        }

        Vector2 glRight = transform.TransformVector(Vector2.right);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, Vector2.Lerp(glRight, rb.velocity, interpolationRatio));
    }
}
