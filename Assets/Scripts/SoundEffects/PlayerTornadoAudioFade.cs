using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTornadoAudioFade : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] AudioFadeDown fade;

    private void OnEnable()
    {
        fade.TotalPlayDuration = player.TornadoActiveTime;
        fade.StartSequenceFadeDown();
    }

    private void OnDisable()
    {
        fade.VolumeReturn();
    }
}
