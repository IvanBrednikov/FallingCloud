using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource size1, size2, size3, bossFight;
    AudioSource currentTrack;
    [SerializeField] float crossFadeTime = 0.5f;
    AudioSource coroutineTrack;
    Coroutine crossFadeCoroutine;
    int _currentrack = 0;

    [SerializeField] CloudSize playerSize;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] GameProgress gameProgress;
    Dictionary<AudioSource, float> volumes;

    public int CurrentTrack 
    { 
        get => _currentrack; 
        set
        {
            if (value > 3)
                _currentrack = 3;
            else
                if (value < 0)
                _currentrack = 0;
            else
                _currentrack = value;
        }
    }

    private void Start()
    {
        playerSize.OnSizeChanged += PlayerSize_OnSizeChanged;
        mapGenerator.OnPortalSpawn += MapGenerator_OnPortalSpawn;
        gameProgress.OnGameProgressReset += GameProgress_OnGameProgressReset;
        volumes = new Dictionary<AudioSource, float>();
        volumes.Add(size1, size1.volume);
        volumes.Add(size2, size2.volume);
        volumes.Add(size3, size3.volume);
        volumes.Add(bossFight, bossFight.volume);        
        PlayNewTrack(size1);
    }

    private void GameProgress_OnGameProgressReset()
    {
        RestartPlayer();
    }

    private void MapGenerator_OnPortalSpawn()
    {
        PlayNextTrack();
    }

    private void PlayerSize_OnSizeChanged(CloudSize.ESize size)
    {
        int sizeNumber = (int)size;
        if(sizeNumber > CurrentTrack)
        {
            PlayNextTrack();
        }
    }

    AudioSource GetTrack(int index)
    {
        AudioSource result = null;
        switch (CurrentTrack)
        {
            case 0:
                result = size1;
                break;
            case 1:
                result = size2;
                break;
            case 2:
                result = size3;
                break;
            case 3:
                result = bossFight;
                break;
            default:
                result = size1;
                break;
        }

        return result;
    }

    void PlayNextTrack()
    {
        CurrentTrack++;
        PlayNewTrack(GetTrack(CurrentTrack));
    }

    void PlayNewTrack(AudioSource newTrack)
    {
        if (currentTrack != null)
        {
            if (crossFadeCoroutine == null)
                crossFadeCoroutine = StartCoroutine(CrossFadeTracks(newTrack));
            else
            {
                StopCoroutine(crossFadeCoroutine);
                coroutineTrack.Stop();
                SetUpDefaultVolumes();
                SimpleSetUpTrack();
            }
        }
        else
            SimpleSetUpTrack();

        void SimpleSetUpTrack()
        {
            if(currentTrack != null)
            {
                currentTrack.Stop();
            }
                
            currentTrack = newTrack;
            currentTrack.Play();
        }
    }
    
    void SetUpDefaultVolumes()
    {
        size1.volume = volumes[size1];
        size2.volume = volumes[size2];
        size3.volume = volumes[size3];
        bossFight.volume = volumes[bossFight];
    }

    IEnumerator CrossFadeTracks(AudioSource newTrack)
    {
        float timeElapsed = 0;

        float currentTrackDefaultVolume = currentTrack.volume;
        float newTrackDefaultVolume = newTrack.volume;

        newTrack.Play();
        coroutineTrack = newTrack;

        while (timeElapsed < crossFadeTime)
        {
            currentTrack.volume = currentTrackDefaultVolume * (1 - timeElapsed / crossFadeTime);
            newTrack.volume = newTrackDefaultVolume * (timeElapsed / crossFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentTrack.Stop();
        currentTrack.volume = currentTrackDefaultVolume;
        newTrack.volume = newTrackDefaultVolume;
        currentTrack = newTrack;
        crossFadeCoroutine = null;
    }

    void RestartPlayer()
    {
        if(CurrentTrack != 0)
        {
            CurrentTrack = 0;
            PlayNewTrack(size1);
        }
    }
}
