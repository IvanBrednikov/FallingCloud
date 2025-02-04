using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAudioEffect : MonoBehaviour
{
    [SerializeField] AudioSource idle;
    [SerializeField] AudioFadeDown fade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            idle.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && fade != null)
        {
            float fadeTime = 1f;
            fade.MakeFadeDown(fadeTime);
            StartCoroutine(ResetTrack(fadeTime));
        }
    }

    IEnumerator ResetTrack(float time)
    {
        yield return new WaitForSeconds(time);
        fade.VolumeReturn();
        idle.Stop();
    }
}
