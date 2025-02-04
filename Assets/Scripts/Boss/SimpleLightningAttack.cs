using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLightningAttack : BossAttack
{
    [SerializeField] GameObject lightningObject;
    [SerializeField] SpriteRenderer lightningSprite;
    [SerializeField] Collider2D lightningCollider;
    [SerializeField] float timeToPrepare = 1f;
    [SerializeField] float maxAlpha = 1;
    float prepareTimer;
    bool isPreparing;

    new void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        base.Activate();
        lightningObject.SetActive(true);
        lightningCollider.enabled = false;
        isPreparing = true;
        prepareTimer = timeToPrepare;
        InvokeSoundEffect();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        lightningObject.SetActive(false);
        isPreparing=false;
    }

    private void Update()
    {
        if(isPreparing)
        {
            prepareTimer -= Time.deltaTime;
            if (prepareTimer < 0)
            {
                isPreparing = false;
                lightningCollider.enabled = true;
            }
                
            Color col = lightningSprite.color;
            col.a = (1 - (prepareTimer / timeToPrepare))*maxAlpha;
            lightningSprite.color = col;
        }
    }
}
