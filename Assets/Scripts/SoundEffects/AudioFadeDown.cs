using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeDown : MonoBehaviour
{
    [SerializeField] AudioSource track;
    [SerializeField] float fadeDuration;

    bool ableToFade;

    float defaultVolume;
    float totalPlayDuration;

    private void Awake()
    {
        defaultVolume = track.volume;
    }

    public void StartSequenceFadeDown()
    {
        StartCoroutine(WaitForFade());
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(totalPlayDuration - fadeDuration);
        StartCoroutine(FadeDown());
    }

    public void MakeFadeDown(float time)
    {
        fadeDuration = time;
        StartCoroutine(FadeDown());
    }

    IEnumerator FadeDown()
    {
        float elapsedTime = 0;
        ableToFade = true;

        while(elapsedTime <= fadeDuration && ableToFade)
        {
            track.volume = (1 - (elapsedTime / fadeDuration)) * defaultVolume;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void VolumeReturn()
    {
        ableToFade = false;
        track.volume = defaultVolume;
    }

    public float TotalPlayDuration { get => totalPlayDuration; set => totalPlayDuration = value; }
}
