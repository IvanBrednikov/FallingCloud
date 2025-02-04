using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainAttack : BossAttack
{
    [SerializeField] GameObject rain;
    [SerializeField] float colliderGrowTime;
    [SerializeField] new BoxCollider2D collider;
    float defaultColliderHeight;
    float defaultColliderLocalY;

    override public void Activate()
    {
        base.Activate();
        attackIsActive = true;
        rain.SetActive(true);
        StartCoroutine(DelayColliderActivate());
        InvokeSoundEffect();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        attackIsActive = false;
        rain.SetActive(false);
    }

    IEnumerator DelayColliderActivate()
    {
        float timeElapsed = 0;
        Vector2 size = collider.size;
        float y = 0;

        while(timeElapsed < colliderGrowTime)
        {
            float ratio = (timeElapsed / colliderGrowTime);
            size.y = defaultColliderHeight * ratio;
            y = defaultColliderLocalY * ratio;
            collider.size = size;
            collider.transform.localPosition = new Vector3(collider.transform.localPosition.x, y, collider.transform.localPosition.z);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        size.y = defaultColliderHeight;
        collider.size = size;
        collider.transform.localPosition = new Vector3(collider.transform.localPosition.x, defaultColliderLocalY, collider.transform.localPosition.z);
    }

    private new void Start()
    {
        base.Start();
        defaultColliderHeight = collider.size.y;
        defaultColliderLocalY = collider.transform.localPosition.y;
    }

    private void Update()
    {
        
    }
}
