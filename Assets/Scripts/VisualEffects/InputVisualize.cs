using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class InputVisualize : MonoBehaviour
{
    [SerializeField] GameObject tailPrefab;
    [SerializeField] GameObject arrowPrefab;

    [SerializeField] Camera cam;
    [SerializeField] Transform arrowGizmo;
    [SerializeField] Transform tail;
    [SerializeField] Transform arrow;

    PlayerInput playerInput;
    bool arrowIsActive;
    Vector2 startPos;

    [SerializeField] bool visualizeActive = true;

    public bool VisualizeActive { get => visualizeActive; set => visualizeActive = value; }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.OnStartPosSelected += OnStartPosSelected;
        playerInput.OnEndPosSelected += OnEndPosSelected;
    }

    private void OnStartPosSelected(Vector2 point)
    {
        if(visualizeActive)
        {
            arrowIsActive = true;
            startPos = point;

            arrowGizmo = new GameObject().transform;
            arrowGizmo.parent = cam.transform;
            Vector3 newGizmoPosition = point;
            newGizmoPosition.z = 1;
            arrowGizmo.localPosition = newGizmoPosition;
            arrowGizmo.name = "ImpulseArrowGizmo";

            tail = Instantiate(tailPrefab).transform;
            arrow = Instantiate(arrowPrefab).transform;

            tail.parent = arrowGizmo;
            tail.localPosition = Vector3.zero;
            arrow.parent = arrowGizmo;
            arrow.localPosition = Vector3.zero;

            SetArrow();
        }
    }

    private void OnEndPosSelected(Vector2 point)
    {
        if (visualizeActive)
        {
            arrowIsActive = false;
            if (arrowGizmo != null && arrowGizmo.gameObject != null)
                Destroy(arrowGizmo.gameObject);
        }
    }

    private void Update()
    {
        if (playerInput.enabled == false && arrowGizmo != null)
        {
            Destroy(arrowGizmo.gameObject);
            arrowIsActive = false;
        }
            

        if (arrowIsActive)
        {
            SetArrow();
        }        
    }

    void SetArrow()
    {
        Vector2 currentPoint = playerInput.ImpulseEndPos;
        Vector2 direction = currentPoint - startPos;
        float vectorLength = direction.magnitude;
        Quaternion arrowRotation = Quaternion.FromToRotation(Vector3.right, direction);

        arrowGizmo.rotation = arrowRotation;
        tail.localScale = new Vector3(vectorLength, tail.localScale.y, tail.localScale.z);
        arrow.localPosition = new Vector3(vectorLength, arrow.localPosition.y, arrow.localPosition.z);
    }

    public void DestroyArrow()
    {
        Destroy(arrowGizmo.gameObject);
    }
}
