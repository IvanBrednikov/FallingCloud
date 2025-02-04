using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossCloudRotationControler : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.AngleAxis(0, Vector3.back);
    }
}
