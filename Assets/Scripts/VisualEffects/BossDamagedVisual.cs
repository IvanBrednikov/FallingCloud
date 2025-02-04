using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamagedVisual : MonoBehaviour
{
    [SerializeField] BossAI boss;
    [SerializeField] Animator animator1;
    [SerializeField] Animator animator2;
    [SerializeField] Animator animator3;
    [SerializeField] bool setEvent = false;

    private void Start()
    {
        if(setEvent)
            boss.OnDamageTaken += Boss_OnDamageTaken;
    }

    private void Boss_OnDamageTaken()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        animator1.SetTrigger("TakenDamage");
        animator2.SetTrigger("TakenDamage");
        animator3.SetTrigger("TakenDamage");
    }
}
