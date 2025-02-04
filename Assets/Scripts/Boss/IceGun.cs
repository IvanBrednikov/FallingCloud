using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGun : BossAttack
{
    [SerializeField] GameObject icePrefab;
    [SerializeField] float prepareTime;
    [SerializeField] float attackStageDuration;
    [SerializeField] bool randomSelectStages = true;
    [SerializeField] int maxStages = 5;

    [SerializeField] bool randomSelectAttack = true;
    enum AttackTypes {None, Wave, ShotGun, MachineGun}
    [SerializeField] AttackTypes currentAttack;

    [SerializeField] float waveSpawnRate;
    [SerializeField] int waveProjectileCount;
    [SerializeField] float waveProjectileSpeed;
    [SerializeField] float waveProjectileLifeTime;
    [SerializeField] float waveSpawnDistance;
    Coroutine waveCoroutine;

    [SerializeField] float shotGunAngleAttack = 60;
    [SerializeField] float separationDelay;
    [SerializeField] int separationTimes = 4;
    [SerializeField] float shotGunProjectileSpeed;
    [SerializeField] float shotGunFireRate;
    [SerializeField] int shotGunProjectileCount;
    [SerializeField] float shotGunProjectileLifeTime;
    [SerializeField] float shotGunSpawnDistance;
    Coroutine shotGunCoroutine;

    [SerializeField] float machineGunAngleAttack = 120;
    [SerializeField] float machineGunFireRate;
    [SerializeField] float machineGunProjectileSpeed;
    [SerializeField] float machineGunProjectileLifeTime;
    [SerializeField] float machineGunDirectionChangeFrequence;
    [SerializeField] float machineGunSpawnDistance;
    Vector2 mgStartPlayerPosition;
    bool angleIncrease;
    float currentAngle;
    Coroutine machineGunCoroutine;

    Coroutine prepareCoroutine;

    new void Start()
    {
        base.Start();
    }

    public override void PreActivate()
    {
        base.PreActivate();
        
        if (randomSelectStages)
        {
            int stagesCount;
            stagesCount = Random.Range(0, maxStages + 1);
            attakActiveTime = (stagesCount * (attackStageDuration + prepareTime));
        }
    }

    public override void Activate()
    {
        base.Activate();
        prepareCoroutine = StartCoroutine(PrepareNewAttack(prepareTime));
    }

    IEnumerator PrepareNewAttack(float waitTime)
    {
        AttackTypes oldAttack = currentAttack;
        currentAttack = AttackTypes.None;
        yield return new WaitForSeconds(waitTime);

        AttackTypes newAttack = oldAttack;
        if(randomSelectAttack)
        {
            while (newAttack == oldAttack)
            {
                newAttack = (AttackTypes)Random.Range(1, 4);

            }
            if (newAttack == AttackTypes.MachineGun)
            {
                mgStartPlayerPosition = player.transform.position;
                currentAngle = 0;
                angleIncrease = Random.Range(0, 1) == 1;
            }
        }
        
        currentAttack = newAttack;

        yield return new WaitForSeconds(attackStageDuration);
        if (attackIsActive)
            prepareCoroutine = StartCoroutine(PrepareNewAttack(prepareTime));
        else
            prepareCoroutine = null;
    }    

    IEnumerator WaveSpawn()
    {
        Vector2 spawnDirection;
        float dAngle = 360 / waveProjectileCount;

        for(int i = 0; i < waveProjectileCount; i++)
        {
            Quaternion directionRotate = Quaternion.AngleAxis(dAngle * i, Vector3.back);
            spawnDirection = directionRotate * Vector2.right;
            Vector2 position = spawnDirection * waveSpawnDistance + (Vector2)transform.position;

            GameObject projectile = Instantiate(icePrefab);
            projectile.transform.position = position;
            projectile.transform.rotation = directionRotate;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = spawnDirection.normalized * waveProjectileSpeed;

            StartCoroutine(DestroyProjectile(projectile, waveProjectileLifeTime));
        }

        InvokeSoundEffect();

        yield return new WaitForSeconds(1 / waveSpawnRate);
        waveCoroutine = null;
    }

    IEnumerator ShotGunMakeFirstShoot(int separationTimes)
    {
        ShotGunShoot(transform.position, player.transform.position - transform.position, shotGunAngleAttack, shotGunProjectileCount, separationTimes);
        
        yield return new WaitForSeconds(shotGunFireRate);   
        shotGunCoroutine = null;
    }

    void ShotGunShoot(Vector2 startPos, Vector2 shootDirection, float splashAngle, int newProjectileCount, int separationTimes)
    {
        Vector2 spawnDirection;
        float dAngle = splashAngle / newProjectileCount; //угол между снарядами
        float offsetAngle = ((newProjectileCount - 1) * dAngle / 2); //смещение, чтобы вектор делящий угол сплэша пополам был направлен в игрока
        Quaternion playerPosRotation = Quaternion.FromToRotation(Vector2.right, shootDirection); //вращение от вектора right до вектора shootDirection

        for (int i = 0; i < newProjectileCount; i++)
        {
            Quaternion directionRotate = Quaternion.AngleAxis((dAngle * i) - offsetAngle, Vector3.back);
            spawnDirection = directionRotate * shootDirection;
            Vector2 position = spawnDirection.normalized * shotGunSpawnDistance + (Vector2)startPos;

            GameObject projectile = Instantiate(icePrefab);
            projectile.transform.position = position;
            projectile.transform.rotation = directionRotate * playerPosRotation;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = spawnDirection.normalized * shotGunProjectileSpeed;

            if (separationTimes > 0)
                StartCoroutine(SeparateShotGunShoot(projectile, shootDirection, splashAngle, newProjectileCount, separationTimes));
            else
                StartCoroutine(DestroyProjectile(projectile, shotGunProjectileLifeTime));
        }

        InvokeSoundEffect();
    }

    IEnumerator SeparateShotGunShoot(GameObject projectile, Vector2 shootDirection, float splashAngle, int newProjectileCount, int separationTimes)
    {
        yield return new WaitForSeconds(separationDelay);

        bool projectileDamageable = projectile.GetComponent<Obstacle>() != null;

        if(projectile != null && projectileDamageable)
        {
            separationTimes--;
            ShotGunShoot(projectile.transform.position, shootDirection, splashAngle, newProjectileCount, separationTimes);
            Destroy(projectile);
        }
    }

    IEnumerator MachineGunShootOnce()
    {
        Vector2 shootDirection = (mgStartPlayerPosition - (Vector2)transform.position);
        Quaternion directionRotate = Quaternion.AngleAxis(currentAngle, Vector3.back);
        Vector2 spawnDirection = directionRotate * shootDirection;
        Vector2 position = (spawnDirection.normalized * machineGunSpawnDistance) + (Vector2)transform.position;

        //определение вращения
        Quaternion playerPosRotation = Quaternion.FromToRotation(Vector2.right, shootDirection); //вращение от вектора right до вектора shootDirection

        GameObject projectile = Instantiate(icePrefab);
        projectile.transform.position = position;
        projectile.transform.rotation = directionRotate * playerPosRotation;
        projectile.GetComponent<Rigidbody2D>().velocity = spawnDirection.normalized * machineGunProjectileSpeed;
        StartCoroutine(DestroyProjectile(projectile, machineGunProjectileLifeTime));

        InvokeSoundEffect();

        yield return new WaitForSeconds(1 / machineGunFireRate);
        machineGunCoroutine = null;
    }

    IEnumerator DestroyProjectile(GameObject projectile, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        if (projectile != null)
            Destroy(projectile);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    private void Update()
    {
        if(attackIsActive)
        {
            if (prepareCoroutine == null)
                prepareCoroutine = StartCoroutine(PrepareNewAttack(attackStageDuration));

            if (currentAttack == AttackTypes.Wave && waveCoroutine == null)
            {
                waveCoroutine = StartCoroutine(WaveSpawn());
            }

            if (currentAttack == AttackTypes.ShotGun && shotGunCoroutine == null)
            {
                shotGunCoroutine = StartCoroutine(ShotGunMakeFirstShoot(separationTimes));
            }
        }   
    }

    private void FixedUpdate()
    {
        if (currentAttack == AttackTypes.MachineGun && attackIsActive)
        {
            if (angleIncrease)
                currentAngle += machineGunAngleAttack * Time.fixedDeltaTime * (1 / machineGunDirectionChangeFrequence);
            else
                currentAngle -= machineGunAngleAttack * Time.fixedDeltaTime * (1 / machineGunDirectionChangeFrequence);

            float angleLimit = machineGunAngleAttack / 2;
            if (currentAngle > angleLimit || currentAngle < -angleLimit)
                angleIncrease = !angleIncrease;

            if (machineGunCoroutine == null)
                machineGunCoroutine = StartCoroutine(MachineGunShootOnce());
        }
    }
}
