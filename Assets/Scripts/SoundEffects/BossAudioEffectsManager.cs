using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudioEffectsManager : MonoBehaviour
{
    [SerializeField] AudioSource stageUp, die, damaged;
    [SerializeField] BossAI boss;

    private void Start()
    {
        boss.OnDamageTaken += Boss_OnDamageTaken;
        boss.OnDie += Boss_OnDie;
        boss.OnBossStageUp += Boss_OnBossStageUp;
    }

    private void Boss_OnBossStageUp()
    {
        stageUp.Play();
    }

    private void Boss_OnDie()
    {
        die.Play();
    }

    private void Boss_OnDamageTaken()
    {
        damaged.Play();
    }
}
