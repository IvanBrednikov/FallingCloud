using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffects : MonoBehaviour
{
    [SerializeField] Transform sprite;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool rotate = true;

    private void Update()
    {
        if(rotate)
        {
            float angle = Time.deltaTime * rotationSpeed;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);
            sprite.rotation *= rotation;
        }
    }
}
