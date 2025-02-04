using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform boss;
    [SerializeField] float interpolateRate = 1;
    [SerializeField] bool isBossCamera;

    [SerializeField] float sizeRatio;
    [SerializeField] float baseSize = 5;
    [SerializeField] float maxCameraSize = 15;
    [SerializeField] float minCameraSize = 5;
    [SerializeField] Vector2 offset;

    //bossWindow
    [SerializeField] float bossRightWindowBoard;
    [SerializeField] float bossLeftWindowBoard;
    [SerializeField] float bossUpperWindowBoard;
    [SerializeField] float bossBottomWindowBoard;

    //tresholds for camera
    [SerializeField] float bottomGlobalBoard;
    [SerializeField] float upperGlobalBoard;

    //window for move camera
    [SerializeField] float rightWindowBoard;
    [SerializeField] float leftWindowBoard;
    [SerializeField] float upperWindowBoard;
    [SerializeField] float bottomWindowBoard;

    [SerializeField] Vector2 startPosition;

    private void FixedUpdate()
    {
        Vector2 targetPos = transform.position;

        if(isBossCamera)
        {
            Vector2 distance = boss.position - player.position;
            targetPos = (distance / 2) + (Vector2)player.position;

            float newSize = baseSize * sizeRatio * distance.magnitude;
            if (newSize > maxCameraSize)
                newSize = maxCameraSize;
            if (newSize < minCameraSize)
                newSize = minCameraSize;

            Camera.main.orthographicSize = newSize;

            if (newSize == maxCameraSize)
            {
                float rightBoardInWorld = bossRightWindowBoard + targetPos.x;
                if (player.position.x > rightBoardInWorld)
                {
                    offset.x = player.position.x - rightBoardInWorld;

                }
                float leftBoardInWorld = bossLeftWindowBoard + targetPos.x;
                if (player.position.x < leftBoardInWorld)
                {
                    offset.x = player.position.x - leftBoardInWorld;
                }
                float uppertBoardInWorld = bossUpperWindowBoard + targetPos.y;
                if (player.position.y > uppertBoardInWorld)
                {
                    offset.y = player.position.y - uppertBoardInWorld;
                }
                float bottomBoardInWorld = bossBottomWindowBoard + targetPos.y;
                if (player.position.y < bottomBoardInWorld)
                {
                    offset.y = player.position.y - bottomBoardInWorld;
                }

                targetPos += offset;
            }
            else
                offset = Vector2.zero;
        }
        else
        {
            float rightBoardInWorld = rightWindowBoard + transform.position.x;
            float leftBoardInWorld = leftWindowBoard + transform.position.x;
            float upperBoardInWorld = upperWindowBoard + transform.position.y;
            float bottomBoardInWorld = bottomWindowBoard + transform.position.y;

            if (rightBoardInWorld < player.position.x)
                targetPos.x = player.position.x - rightBoardInWorld + transform.position.x;

            if (leftBoardInWorld > player.position.x)
                targetPos.x = player.position.x - leftBoardInWorld + transform.position.x;

            if (upperBoardInWorld < player.position.y)
                targetPos.y = player.position.y - upperBoardInWorld + transform.position.y;

            if (bottomBoardInWorld > player.position.y)
                targetPos.y = player.position.y - bottomBoardInWorld + transform.position.y;

            if (targetPos.y >= upperGlobalBoard)
                targetPos.y = upperGlobalBoard;

            if (targetPos.y <= bottomGlobalBoard)
                targetPos.y = bottomGlobalBoard;
        }

        transform.position = Vector3.Slerp(transform.position, targetPos, interpolateRate*Time.fixedDeltaTime);
    }

    public void SetBossCamera()
    {
        isBossCamera = true;
    }

    public void RestartCamera()
    {
        transform.position = startPosition;
        isBossCamera = false;
        Camera.main.orthographicSize = baseSize;
    }

    public void InputVisualizeState(bool state)
    {
        InputVisualize iVisual = GetComponentInChildren<InputVisualize>(true);
        iVisual.VisualizeActive = state;
    }
}
