using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossCloudAttack : BossAttack
{
    [SerializeField] GameObject cloudPrefab;
    [SerializeField] int cloudCountByRadius = 10;
    [SerializeField] float cloudInterval = 4f;

    [SerializeField] float timeToScale = 1f;
    float currentScale;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1.4f;

    [SerializeField] float timeToStartRotation = 1.5f;
    [SerializeField] float rotationSpeed = 45f; //degrees per sec
    GameObject crossGizmo;
    GameObject[] allClouds;
    bool scalingIson;
    bool rotationIsOn;

    new void Start()
    {
        base.Start();
        currentScale = minScale;
    }

    public override void Activate()
    {
        base.Activate();
        pointToMove = player.transform.position;
        crossGizmo = new GameObject();
        crossGizmo.transform.position = transform.position;
        crossGizmo.transform.parent = transform;

        allClouds = new GameObject[4*cloudCountByRadius];

        int index = 0;

        for(int i = 0; i < 4; i++)
        {
            Vector2 spawnDirection = Vector2.right;
            if(i == 1)
                spawnDirection = Vector2.up;
            if (i == 2)
                spawnDirection = Vector2.left;
            if (i == 3)
                spawnDirection = Vector2.down;

            for(int j = 0; j < cloudCountByRadius; j++)
            {
                Vector2 position = spawnDirection.normalized * cloudInterval * (j + 1);
                Vector2 scale = new Vector2(minScale, minScale);

                GameObject cloud = Instantiate(cloudPrefab);
                allClouds[index] = cloud;
                cloud.transform.parent = crossGizmo.transform;
                cloud.transform.localPosition = position;
                cloud.transform.localScale = scale;

                index++;
            }
        }

        scalingIson = true;
        StartCoroutine(StartRotation());
        InvokeSoundEffect();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Destroy(crossGizmo);
        allClouds = null;
        currentScale = minScale;
        rotationIsOn = false;
        scalingIson = false;
    }

    IEnumerator StartRotation()
    {
        yield return new WaitForSeconds(timeToStartRotation);
        rotationIsOn = true;
    }

    void Update()
    {
        if(scalingIson)
        {
            currentScale += ((maxScale - minScale) / timeToScale) * Time.deltaTime;
            for(int i = 0; i < allClouds.Length; i++)
                if (allClouds[i] != null)
                {
                    allClouds[i].transform.localScale = new Vector2(currentScale, currentScale);
                }

            if(currentScale > maxScale)
                scalingIson=false;
        }

        if(rotationIsOn)
        {
            Quaternion newRotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.back) * crossGizmo.transform.rotation;
            crossGizmo.transform.rotation = newRotation;
        }
    }
}
