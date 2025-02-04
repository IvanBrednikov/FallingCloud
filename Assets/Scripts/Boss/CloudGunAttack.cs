using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGunAttack : BossAttack
{
    [SerializeField] float cloudGunAttackTime = 5f;
    [SerializeField] GameObject cloudProjectilePrefab;
    [SerializeField] float shootsPerSec = 4;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float sprayRatio = 1f;
    [SerializeField] Coroutine shootRoutine;
    [SerializeField] float projectileLifeTime = 5;

    Rigidbody2D bossRigidBody;

    new private void Start()
    {
        base.Start();
        bossRigidBody = GetComponent<Rigidbody2D>();
    }

    public override void Activate()
    {
        attackIsActive = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    IEnumerator CloudShoot()
    {
        yield return new WaitForSeconds(1 / shootsPerSec);
        GameObject projectile = Instantiate(cloudProjectilePrefab);
        projectile.transform.position = transform.position;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        float spray = Random.Range(0, sprayRatio);
        Vector2 velocity = (player.transform.position + new Vector3(spray, spray, 0)) - transform.position;
        rb.velocity = (velocity.normalized * projectileSpeed) + bossRigidBody.velocity;
        shootRoutine = null;
        StartCoroutine(ProjectileDestroy(projectile, projectileLifeTime));

        InvokeSoundEffect();
    }

    IEnumerator ProjectileDestroy(GameObject projectile, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (projectile != null)
            Destroy(projectile);
    }

    private void Update()
    {
        if (shootRoutine == null && attackIsActive)
        {
            shootRoutine = StartCoroutine(CloudShoot());
        }
    }
}
