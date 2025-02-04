using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UIElements;

[RequireComponent(typeof(VideoPlayer))]
public class GameplayVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer vPlayer;
    [SerializeField] float inActivityThreshold = 60;
    [SerializeField] float inActivityTime = 0f;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] PanelManager uiManager;
    [SerializeField] SpriteRenderer cloudSprite;
    int sortId = 0;

    private void Start()
    {
        vPlayer = GetComponent<VideoPlayer>();
        uiManager = FindObjectOfType<PanelManager>();
        vPlayer.playOnAwake = false;
        vPlayer.targetCameraAlpha = 0f;
        vPlayer.isLooping = true;
        if (cloudSprite != null)
            sortId = cloudSprite.sortingLayerID;
    }

    IEnumerator VideoFade()
    {
        cloudSprite.sortingLayerID = 0;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            vPlayer.targetCameraAlpha = elapsedTime / fadeDuration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vPlayer.targetCameraAlpha = 1f;

        if (cloudSprite != null)
            cloudSprite.sortingLayerID = sortId;

        fadeCoroutine = null;
    }

    Coroutine fadeCoroutine;
    private void Update()
    {
        if (Input.anyKey || Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            inActivityTime = 0; 
        else
            inActivityTime += Time.deltaTime;


        if (inActivityTime > inActivityThreshold && !vPlayer.isPlaying)
        {
            vPlayer.Play();
            if (fadeCoroutine == null)
            {
                fadeCoroutine = StartCoroutine(VideoFade());
                uiManager.HideCurrentPanel();
            }
        }
            

        if (inActivityTime < inActivityThreshold && vPlayer.isPlaying)
        {
            vPlayer.Stop();
            StopAllCoroutines();
            vPlayer.targetCameraAlpha = 0f;
            uiManager.UnhideCurrentPanel();
        }
    }
}
