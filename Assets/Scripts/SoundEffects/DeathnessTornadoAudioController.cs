using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathnessTornadoAudioController : MonoBehaviour
{
    [SerializeField] DeathnessTornado tornado;
    [SerializeField] AudioSource track;
    [SerializeField] float fadeDuration = 0.4f;
    float defaultVolume;

    private void Start()
    {
        defaultVolume = track.volume;
        tornado.OnTornadoStateChange += Tornado_OnTornadoStateChange;
        tornado.OnTornadoRestart += Tornado_OnTornadoRestart;
    }

    private void Tornado_OnTornadoRestart()
    {
        if (track.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEffect(false, true));
        }
    }

    private void Tornado_OnTornadoStateChange(bool newState)
    {
        if(!track.isPlaying && newState)
        {
            StopAllCoroutines();
            track.Play();
            StartCoroutine(FadeEffect(true, false));
        }

        if(track.isPlaying && !newState)
        {
            Debug.Log("StopTrack");
            StopAllCoroutines();
            StartCoroutine(FadeEffect(false, true));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!track.isPlaying && collision.tag == "Player" && !tornado.IsAgreesiveState)
        {
            StopAllCoroutines();
            track.Play();
            StartCoroutine(FadeEffect(true, false));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(track.isPlaying && collision.tag == "Player" && !tornado.IsAgreesiveState)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEffect(false, true));
        }
    }

    IEnumerator FadeEffect(bool volumeUp, bool stopTrack)
    {
        float timeElapsed = 0;
        float volumeRatio;

        while(timeElapsed < fadeDuration)
        {
            if (volumeUp)
                volumeRatio = timeElapsed / fadeDuration;
            else
                volumeRatio = 1 - (timeElapsed / fadeDuration);
            track.volume = defaultVolume * volumeRatio;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (stopTrack)
            track.Stop();
        track.volume = defaultVolume;
    }
}
