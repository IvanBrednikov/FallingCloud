using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedLightning : BossAttack
{
    //spawnLightnings
    [SerializeField] GameObject lightningSpherePrefab;
    [SerializeField] int lightningsCount;
    [SerializeField] float lightningLifeTime;
    [SerializeField] float lightningSpeed;
    [SerializeField] float startDistance;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] float scalingTime; //from activate
    [SerializeField] float spawnRate = 1f;

    float currentScale;
    bool scalingIsOn;
    GameObject[] spheres;
    Coroutine spawnCoroutine;

    //simpleLightning
    [SerializeField] GameObject lightningObject;
    [SerializeField] SpriteRenderer lightningSprite;
    [SerializeField] Collider2D lightningCollider;
    [SerializeField] float timeToPrepare = 1f; //from activate
    [SerializeField] float maxAlpha = 1;
    float prepareTimer;
    bool isPreparingOwnLigthning;

    new void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        base.Activate();
        //ownLightning
        lightningObject.SetActive(true);
        lightningCollider.enabled = false;
        isPreparingOwnLigthning = true;
        prepareTimer = timeToPrepare;
        movementType = BossAI.MovementType.MoveToPlayer;

        spawnCoroutine = StartCoroutine(SpawnSpheres());
        InvokeSoundEffect();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        lightningObject.SetActive(false);
        isPreparingOwnLigthning = false;
    }

    IEnumerator ActivateOwnLighting()
    {
        yield return new WaitForSeconds(scalingTime);
        lightningObject.SetActive(true);
        lightningCollider.enabled = false;
        isPreparingOwnLigthning = true;
        prepareTimer = timeToPrepare;
        movementType = BossAI.MovementType.MoveToPlayer;
    }

    IEnumerator DestroySphere(GameObject lightingSphere)
    {
        yield return new WaitForSeconds(lightningLifeTime);
        if(lightingSphere != null)
            Destroy(lightingSphere);
    }   
    
    IEnumerator SpawnSpheres()
    {
        yield return new WaitForSeconds(1 / spawnRate);
        currentScale = minScale;
        spheres = new GameObject[lightningsCount];
        Vector2 spawnDirection;
        float dAngle = 360 / lightningsCount;

        for (int i = 0; i < lightningsCount; i++)
        {
            Quaternion rotateDirection = Quaternion.AngleAxis(dAngle * i, Vector3.back);
            spawnDirection = rotateDirection * Vector2.right;
            Vector2 position = (spawnDirection.normalized * startDistance) + (Vector2)transform.position;
            Vector2 scale = new Vector2(minScale, minScale);

            GameObject lSphere = Instantiate(lightningSpherePrefab);
            lSphere.transform.position = position;
            lSphere.transform.localScale = scale;
            lSphere.transform.rotation = rotateDirection;
            lSphere.transform.parent = transform;
            LightningSphere sphere = lSphere.GetComponentInChildren<LightningSphere>();
            sphere.Speed = lightningSpeed;
            sphere.enabled = false;

            spheres[i] = lSphere;
            StartCoroutine(DestroySphere(lSphere));
        }

        scalingIsOn = true;
        spawnCoroutine = null;
    }

    private void Update()
    {
        if (scalingIsOn)
        {
            currentScale += ((maxScale - minScale) / scalingTime) * Time.deltaTime;

            for (int i = 0; i < spheres.Length; i++)
            {
                if (spheres[i] != null)
                    spheres[i].transform.localScale = new Vector2(currentScale, currentScale);
            }

            if (currentScale > maxScale)
            {
                scalingIsOn = false;
                for (int i = 0; i < spheres.Length; i++)
                {
                    if (spheres[i] != null && spheres[i].transform.childCount > 0)
                    {
                        spheres[i].transform.parent = null;
                        spheres[i].GetComponentInChildren<LightningSphere>().enabled = true;
                    }
                }
                if(attackIsActive)
                    spawnCoroutine = StartCoroutine(SpawnSpheres());
            }
        }

        if (isPreparingOwnLigthning)
        {
            prepareTimer -= Time.deltaTime;
            if (prepareTimer < 0)
            {
                isPreparingOwnLigthning = false;
                lightningCollider.enabled = true;
            }

            Color col = lightningSprite.color;
            col.a = maxAlpha * (1 - (prepareTimer / timeToPrepare));
            lightningSprite.color = col;
        }
    }
}
