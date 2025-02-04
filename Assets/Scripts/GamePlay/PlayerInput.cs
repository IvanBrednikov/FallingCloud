using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Vector2Event(Vector2 vector);

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Cloud player;

    Vector2 pointerStart;
    Vector2 currentPointerPos;

    public event Vector2Event OnStartPosSelected;
    public event Vector2Event OnEndPosSelected;

    int currentTouchIndex = -1;

    private void Start()
    {
        Input.multiTouchEnabled = true;
    }

    private void Update()
    {
        

        if (Application.platform != RuntimePlatform.Android)
        {
            currentPointerPos = cam.transform.InverseTransformPoint(cam.ScreenToWorldPoint(Input.mousePosition));
            
            if (Input.GetMouseButtonDown(0))
            {
                pointerStart = currentPointerPos;
                if (OnStartPosSelected != null)
                    OnStartPosSelected(currentPointerPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 pointerEnd = currentPointerPos;
                Vector2 forceDirection = pointerEnd - pointerStart;

                player.AddImpulse(forceDirection);

                OnEndPosSelected?.Invoke(currentPointerPos);
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                currentPointerPos = cam.transform.InverseTransformPoint(cam.ScreenToWorldPoint(t.position));
                if (currentTouchIndex == -1)
                {
                    pointerStart = currentPointerPos;
                    if (OnStartPosSelected != null)
                        OnStartPosSelected(currentPointerPos);
                    currentTouchIndex = 0;
                }

                if (t.phase == TouchPhase.Ended)
                {
                    Vector2 pointerEnd = currentPointerPos;
                    Vector2 forceDirection = pointerEnd - pointerStart;

                    player.AddImpulse(forceDirection);

                    OnEndPosSelected?.Invoke(currentPointerPos);
                    
                    currentTouchIndex = -1;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.RainActivate();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            player.LightningActivate();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.TornadoActivate();
        }
    }

    public Vector2 ImpuleStartPos
    {
        get { return pointerStart; }
    }

    public Vector2 ImpulseEndPos
    {
        get { return currentPointerPos; }
    }
}
