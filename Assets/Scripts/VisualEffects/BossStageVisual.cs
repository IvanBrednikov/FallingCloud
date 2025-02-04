using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageVisual : MonoBehaviour
{
    [SerializeField] SpriteRenderer currentSprite;
    [SerializeField] BossAI boss;
    [SerializeField] Sprite stage1Sprite;
    [SerializeField] Sprite stage2Sprite;
    [SerializeField] Sprite stage3Sprite;

    [SerializeField] float maxLightningAngle = 60;
    [SerializeField] float minlightningAngle = -60;
    [SerializeField] Transform animationTransform;
    [SerializeField] Animator animator;

    private void Start()
    {
        boss.OnBossStageUp += Boss_OnBossStageUp;
        boss.OnBossConfigure += Boss_OnBossConfigure;
    }

    private void Boss_OnBossConfigure()
    {
        SetSpriteStage(boss.GetCurrentStage);
    }

    private void Boss_OnBossStageUp()
    {
        SetSpriteStage(boss.GetCurrentStage);
        PlayAnimation();
    }

    void SetSpriteStage(BossAI.BossStage stage)
    {
        switch (stage)
        {
            case BossAI.BossStage.Simple:
                currentSprite.sprite = stage1Sprite;
                break;
            case BossAI.BossStage.Normal:
                currentSprite.sprite = stage2Sprite;
                break;
            case BossAI.BossStage.Hard:
                currentSprite.sprite = stage3Sprite;
                break;
            default:
                currentSprite.sprite = stage1Sprite;
                break;
        }
    }

    void PlayAnimation()
    {
        float lightningAngle = Random.Range(minlightningAngle, maxLightningAngle);
        Quaternion rotation = Quaternion.AngleAxis(lightningAngle, Vector3.back);
        animationTransform.rotation = rotation;
        animator.SetTrigger("StageUp");
    }
}
