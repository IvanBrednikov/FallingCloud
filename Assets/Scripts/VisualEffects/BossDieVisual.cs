using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieVisual : MonoBehaviour
{
    [SerializeField] BossAI boss;
    [SerializeField] Animator animator;

    private void Start()
    {
        boss.OnDie += Boss_OnDie;
    }

    private void Boss_OnDie()
    {
        animator.SetTrigger("BossDie");
    }
}
