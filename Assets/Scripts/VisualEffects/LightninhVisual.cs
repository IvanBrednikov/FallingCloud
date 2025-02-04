using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightninhVisual : MonoBehaviour
{
    [SerializeField] CloudSize playerSize;
    [SerializeField] float[] sizes;

    private void Start()
    {
        playerSize.OnSizeChanged += PlayerSize_OnSizeChanged;
        SetSize(playerSize.GetSize);
    }

    private void PlayerSize_OnSizeChanged(CloudSize.ESize size)
    {
        SetSize(size);
    }

    void SetSize(CloudSize.ESize newSize)
    {
        float size;
        switch (newSize)
        {
            case CloudSize.ESize.Small:
                size = sizes[0];
                break;
            case CloudSize.ESize.Medium:
                size = sizes[1];
                break;
            case CloudSize.ESize.Big:
                size = sizes[2];
                break;
            default:
                size = sizes[0];
                break;
        }

        transform.transform.localScale = new Vector3(size, size, size);
    }
}
