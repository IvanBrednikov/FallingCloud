using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] Transform windGizmo;
    [SerializeField] Animator animator;
    [SerializeField] PlayerInput input;
    [SerializeField] bool changePosition;

    private void Start()
    {
        input.OnEndPosSelected += Input_OnEndPosSelected;
    }

    private void Input_OnEndPosSelected(Vector2 vector)
    {
        if(changePosition)
            transform.localPosition = input.ImpuleStartPos;

        Vector2 forceDirectionLocal = input.ImpulseEndPos - input.ImpuleStartPos;
        float angle = Vector2.SignedAngle(forceDirectionLocal, Vector2.right);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);

        transform.localRotation = rotation;
        animator.SetTrigger("MakeWind");
    }
}
