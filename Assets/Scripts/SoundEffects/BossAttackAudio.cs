using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackAudio : MonoBehaviour
{
    [SerializeField] AudioSource track;
    [SerializeField] bool needFade;
    [SerializeField] bool needToStopTrack = true;
    [SerializeField] AudioFadeDown fade;
    [SerializeField] BossAI boss;
    [SerializeField] BossAI.Attacks selectedAttack;
    BossAttack attack;

    private void Start()
    {
        attack = boss.GetAttack(selectedAttack);
        attack.OnSoundEffect += Attack_OnSoundEffect;
        attack.OnAttackEnd += Attack_OnAttackEnd;
        
        if(needFade)
            fade.TotalPlayDuration = attack.AttackTime;
    }

    private void Attack_OnAttackEnd()
    {
        if (needFade)
        {
            fade.VolumeReturn();
        }
        
        if(needToStopTrack)
            track.Stop();
    }

    private void Attack_OnSoundEffect()
    {
        if (needFade)
        {
            fade.VolumeReturn();
            fade.StartSequenceFadeDown();
        }
        
        track.Play();
    }
}
