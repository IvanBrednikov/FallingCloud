using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCloudSpawn : BossAttack
{
    [SerializeField] GameObject cloudPrefab;
    [SerializeField] float cloudStartSpeed = 0.5f;
    [SerializeField] float startDistance = 1f;
    [SerializeField] float timeBetweenSpawn = 2f;
    [SerializeField] int cloudsPerSpawn = 6;
    [SerializeField] float offsetAngle = 10f;
    [SerializeField] float currentOffsetAngle = 0;

    [SerializeField] float timeToScale = 1f;
    float currentScale;
    [SerializeField] float minScale = 0.5f;
    [SerializeField]float maxScale = 1.4f;

    GameObject[] scalingClouds;
    bool scalingIson;

    [SerializeField] float cloudLifeTime = 10f;

    Coroutine spawnWave;

    new void Start()
    {
        base.Start();
        currentScale = minScale;
    }

    public override void Activate()
    {
        base.Activate();
        pointToMove = player.transform.position;
        spawnWave = StartCoroutine(SpawnWave());
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(timeBetweenSpawn);

        scalingClouds = new GameObject[cloudsPerSpawn];
        float dAngle = 360 / cloudsPerSpawn;
        Vector2 spawnDirection;

        for (int i = 0; i < cloudsPerSpawn; i++)
        {
            Quaternion rotateDirection = Quaternion.AngleAxis((dAngle * i) + currentOffsetAngle, Vector3.back);
            spawnDirection = rotateDirection * Vector2.right;

            Vector2 position = (spawnDirection.normalized * startDistance) + (Vector2)transform.position;
            Vector2 scale = new Vector2(minScale, minScale);
            Vector2 velocity = spawnDirection.normalized * cloudStartSpeed;

            GameObject cloud = Instantiate(cloudPrefab);
            scalingClouds[i] = cloud;
            cloud.transform.localPosition = position;
            cloud.transform.localScale = scale;
            cloud.GetComponent<Rigidbody2D>().velocity = velocity;
            
            StartCoroutine(DestroyCloud(cloud));
        }

        currentOffsetAngle += offsetAngle;
        scalingIson = true;
        currentScale = minScale;
        spawnWave = null;
        InvokeSoundEffect();
    }

    IEnumerator DestroyCloud(GameObject cloud)
    {
        yield return new WaitForSeconds(cloudLifeTime);
        if (cloud != null)
            Destroy(cloud);
    }

    void Update()
    {
        if(spawnWave == null && attackIsActive)
        {
            spawnWave = StartCoroutine(SpawnWave());
        }

        if (scalingIson)
        {
            currentScale += ((maxScale - minScale) / timeToScale) * Time.deltaTime;

            for(int i = 0; i < scalingClouds.Length; i++)
            {
                if (scalingClouds[i] != null)
                    scalingClouds[i].transform.localScale = new Vector2(currentScale, currentScale);
            }

            if (currentScale > maxScale)
                scalingIson = false;
        }
    }
}
