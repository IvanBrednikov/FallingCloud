using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainVisualize : MonoBehaviour
{
    [SerializeField] Transform gizmo;
    [SerializeField] ParticleSystem particles;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] CloudSize cloudSize;

    //rain angle offset by velocity
    [SerializeField] float maxAngle = 15;
    [SerializeField] float maxSpeedReact = 30;

    //sizeParams
    [SerializeField] ParticleSystemSize[] sizes;

    public void SetSize(CloudSize.ESize newSize)
    {
        foreach(ParticleSystemSize size in sizes)
        {
            if(size.Size == newSize)
            {
                ParticleSystem.EmissionModule module = particles.emission;
                module.rateOverTime = size.Emmision;
                ParticleSystem.ShapeModule shape = particles.shape;
                shape.scale = new Vector3(size.EmmiterWidth, 1, 1);
                break;
            }
        }
    }

    private void Start()
    {
        if(cloudSize != null)
        {
            cloudSize.OnSizeChanged += CloudSize_OnSizeChanged;
            SetSize(cloudSize.GetSize);
        }
    }

    private void CloudSize_OnSizeChanged(CloudSize.ESize size)
    {
        SetSize(size);
    }

    private void Update()
    {
        Vector2 velocity = playerRb.velocity;
        float offsetAngle = (maxAngle / maxSpeedReact) * velocity.x;
        if (offsetAngle > maxAngle)
            offsetAngle = maxAngle * Mathf.Sign(velocity.x);
        Quaternion rotation = Quaternion.AngleAxis(offsetAngle, Vector3.back);
        gizmo.rotation = rotation;
    }
    
    [System.Serializable]
    public class ParticleSystemSize
    {
        [SerializeField] string name;
        [SerializeField] CloudSize.ESize size;
        [SerializeField] float emmitterWidth = 2;
        [SerializeField] float emission = 50;

        public CloudSize.ESize Size { get { return size; } }

        public float EmmiterWidth { get { return emmitterWidth; } }

        public float Emmision { get {return emission; } }
    }
}
