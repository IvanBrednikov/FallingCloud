using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCollider : MonoBehaviour
{
    public Cloud player;
    [SerializeField] CloudSize playerSize;
    [SerializeField] new BoxCollider2D collider;
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;
    [SerializeField] bool attackIsReady = true;
    Coroutine attackRechargeC;
    [SerializeField] List<Collider2D> damagingColliders;
    [SerializeField] float[] xSizes;

    private void Start()
    {
        damagingColliders = new List<Collider2D>();
        playerSize.OnSizeChanged += PlayerSize_OnSizeChanged;
        SetColliderSize(playerSize.GetSize);
    }

    private void PlayerSize_OnSizeChanged(CloudSize.ESize size)
    {
        SetColliderSize(size);
    }

    public void SetColliderSize(CloudSize.ESize newSize)
    {
        Vector2 size = new Vector2(collider.bounds.size.x, collider.size.y);
        
        switch (newSize)
        {
            case CloudSize.ESize.Small:
                size.x = xSizes[0];
                break;
            case CloudSize.ESize.Medium:
                size.x = xSizes[1];
                break;
            case CloudSize.ESize.Big:
                size.x = xSizes[2];
                break;
        }

        collider.size = size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "CloudShield" || collision.transform.tag == "Boss")
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
        for (int i = 0; i < damagingColliders.Count; i++)
        {
            Collider2D buffer = damagingColliders[i];
            CloudShieldDurable cloud;
            if (buffer.TryGetComponent<CloudShieldDurable>(out cloud))
            {
                cloud.GainDamage(attackDamage);
            }
            BossAI boss;
            if (buffer.TryGetComponent<BossAI>(out boss) && CheckBossEnableToDamage())
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

    bool CheckBossEnableToDamage()
    {
        BossAI boss;
        bool result = false;
        for (int i = 0; i < damagingColliders.Count; i++)
            if (damagingColliders[i] != null && damagingColliders[i].TryGetComponent<BossAI>(out boss))
            {
                result = true;
                RaycastHit2D[] hits = Physics2D.RaycastAll(player.transform.position, Vector2.down, 50);
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].collider.tag == "CloudShield")
                        result = false;
                }
            }

        return result;
    }

    private void Update()
    {
        if (attackRechargeC == null)
        {
            attackRechargeC = StartCoroutine(MakeDamage());
        }
    }
}
