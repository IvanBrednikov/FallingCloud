using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRainAudioFade : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] AudioFadeDown fade;

    private void OnEnable()
    {
        fade.TotalPlayDuration = player.RainActiveTime;
        fade.StartSequenceFadeDown();
    }

    private void OnDisable()
    {
        fade.VolumeReturn();
    }
}
