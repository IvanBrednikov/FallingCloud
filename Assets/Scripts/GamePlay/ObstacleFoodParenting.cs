using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFoodParenting : MonoBehaviour
{
    private void Start()
    {
        Food[] food = transform.GetComponentsInChildren<Food>();
        Transform chunk = transform.parent;
        foreach(Food transform in food)
        {
            transform.transform.parent = chunk;
        }
    }
}
