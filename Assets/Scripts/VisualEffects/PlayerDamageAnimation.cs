using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageAnimation : MonoBehaviour
{
    [SerializeField] Animator animator1;
    [SerializeField] Animator animator2;
    [SerializeField] Animator animator3;
    [SerializeField] SpriteRenderer sprite1;
    [SerializeField] SpriteRenderer sprite2;
    [SerializeField] SpriteRenderer sprite3;

    public Cloud player;
    public CloudSize playerSize;

    [SerializeField] Color size1Color;
    [SerializeField] Color size2Color;
    [SerializeField] Color size3Color;

    [SerializeField] float ownSize1;
    [SerializeField] float ownSize2;
    [SerializeField] float ownSize3;

    //spawned animation props
    [SerializeField] bool destroyOnTime;
    [SerializeField] float timeToDestroy = 1;

    private void Start()
    {
        if(!destroyOnTime)
        {
            player.OnObstacleCollide += Player_OnObstacleCollide;
            playerSize.OnSizeChanged += PlayerSize_OnSizeChanged;
        }
    }

    private void PlayerSize_OnSizeChanged(CloudSize.ESize size)
    {
        PlayAnimation();
    }

    private void Player_OnObstacleCollide(string tagCollidedObject)
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        Vector3 size = new Vector3(1, 1);

        if (playerSize.GetSize == CloudSize.ESize.Small)
        {
            size = new Vector2(ownSize1, ownSize1);
            sprite1.color = size1Color;
            sprite2.color = size1Color;
            sprite3.color = size1Color;
        }
        else
            if (playerSize.GetSize == CloudSize.ESize.Medium)
        {
            size = new Vector2(ownSize2, ownSize2);
            sprite1.color = size2Color;
            sprite2.color = size2Color;
            sprite3.color = size2Color;
        }
        else
            if (playerSize.GetSize == CloudSize.ESize.Big)
        {
            size = new Vector2(ownSize3, ownSize3);
            sprite1.color = size3Color;
            sprite2.color = size3Color;
            sprite3.color = size3Color;
        }

        transform.localScale = size;
        animator1.SetTrigger("TakenDamage");
        animator2.SetTrigger("TakenDamage");
        animator3.SetTrigger("TakenDamage");

        if (destroyOnTime)
            StartCoroutine(DestroyAnimation());
    }

    IEnumerator DestroyAnimation()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }
}
