using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityVisualize : MonoBehaviour
{
    [SerializeField]
    Transform rotationGizmo;

    [SerializeField]
    float scaleRatio = 1;
    [SerializeField]
    float maxSpeed;

    [SerializeField]
    Transform tail;
    [SerializeField]
    float maxTailLength;
    

    [SerializeField]
    Transform arrow;
    [SerializeField]
    float maxArrowDistance;
    [SerializeField]
    float maxArrowSize;
    [SerializeField]
    float minArrowSize;
    [SerializeField]
    float sizeRatio;

    [SerializeField]
    Rigidbody2D rb;

    private void Update()
    {
        Vector2 velocity = rb.velocity;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, velocity);
        rotationGizmo.rotation = rotation;

        float tailLength = (rb.velocity.magnitude * (maxTailLength / maxSpeed)) * scaleRatio;
        tail.localScale = new Vector3(tailLength, tail.localScale.y, tail.localScale.z);

        float arrowSize = (((rb.velocity.magnitude / maxSpeed) * (maxArrowSize - minArrowSize)) + minArrowSize) * sizeRatio;
        float arrowDistance = (rb.velocity.magnitude * (maxArrowDistance / maxSpeed)) * scaleRatio;

        arrow.localPosition = new Vector3(arrowDistance, arrow.localPosition.y, arrow.localPosition.z);
        arrow.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
    }
}
