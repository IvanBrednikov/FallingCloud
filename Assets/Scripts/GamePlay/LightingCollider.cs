using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingCollider : MonoBehaviour
{
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;
    [SerializeField] bool attackIsReady = true;
    Coroutine attackRechargeC;
    [SerializeField] List<Collider2D> damagingColliders;
    
    [SerializeField] new CircleCollider2D collider;
    [SerializeField] float[] sizes;
    [SerializeField] CloudSize playerSize;

    [SerializeField] PlayerStatistic statistic;

    private void Start()
    {
        damagingColliders = new List<Collider2D>();
        playerSize.OnSizeChanged += PlayerSize_OnSizeChanged;
        SetNewSize(playerSize.GetSize);
    }

    private void PlayerSize_OnSizeChanged(CloudSize.ESize size)
    {
        SetNewSize(size);
    }

    void SetNewSize(CloudSize.ESize newSize)
    {
        float newRaidus;
        switch (newSize)
        {
            case CloudSize.ESize.Small:
                newRaidus = sizes[0];
                break;
            case CloudSize.ESize.Medium:
                newRaidus = sizes[1];
                break;
            case CloudSize.ESize.Big:
                newRaidus = sizes[2];
                break;
            default:
                newRaidus = sizes[0];
                break;
        }

        collider.radius = newRaidus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle obstacle;
        if (collision.TryGetComponent<Obstacle>(out obstacle))
        {
            obstacle.DestroyByLighting();
            if (collision.tag == "Projectile")
                statistic.ProjectileDestroyedByLightning++;
            else
                statistic.ObstacleDestroyedByLightning++;
        }

        if(collision.transform.tag == "CloudShield" || collision.transform.tag == "Boss")
        {
            if (!damagingColliders.Contains(collision))
                damagingColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "CloudShield" || collision.transform.tag == "Boss")
        {
            if (damagingColliders.Contains(collision))
                damagingColliders.Remove(collision);
        }
    }

    IEnumerator MakeDamage()
    {
        for(int i = 0; i < damagingColliders.Count; i++)
        {
            Collider2D buffer = damagingColliders[i];
            CloudShieldDurable cloud;
            if(buffer.TryGetComponent<CloudShieldDurable>(out cloud))
            {
                cloud.GainDamage(attackDamage);
            }

            BossAI boss;
            if(buffer.TryGetComponent<BossAI>(out boss))
            {
                boss.DamageGain(attackDamage);
            }
        }
        yield return new WaitForSeconds(1 / attackRate);
        attackRechargeC = null;
    }

    private void OnEnable()
    {
        attackRechargeC = StartCoroutine(MakeDamage());
    }

    private void Update()
    {
        if(attackRechargeC == null)
        {
            attackRechargeC = StartCoroutine(MakeDamage());
        }
    }
}
