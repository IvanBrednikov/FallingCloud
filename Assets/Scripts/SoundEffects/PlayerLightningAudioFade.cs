using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightningAudioFade : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] AudioFadeDown fade;

    private void OnEnable()
    {
        fade.TotalPlayDuration = player.LightingActiveTime;
        fade.StartSequenceFadeDown();
    }

    private void OnDisable()
    {
        fade.VolumeReturn();
    }
}
