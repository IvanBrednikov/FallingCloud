using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class GameSettings
{
    private static List<Resolution> resolutions = GetResolutionsOnStart;
    static bool playWindSound;
    static bool showArrows;
    static bool showResultsPanel;
    static GameProgress.Difficulty lastSelectedDifficulty;
    static int languageIndex = 0;
    static float impulseRatio = 2;

    public static void SetScreenResolution(int index, bool fullScreen)
    {
        if(index >=0)
        {
            Resolution newResolution = resolutions[index];
            Screen.SetResolution(newResolution.width, newResolution.height, fullScreen, newResolution.refreshRate);
        }
    }

    public static string GetCurrentResolutionChoice()
    {
        Resolution res = Screen.currentResolution;
        string result = res.width + "x" + res.height + " " + res.refreshRate + "Hz";
        return result;
    }

    static List<Resolution> GetResolutionsOnStart
    {
        get
        {
            List<Resolution> result = new List<Resolution>();
            foreach(Resolution res in Screen.resolutions)
            {
                if (res.width >= 800 && res.height >= 600)
                    result.Add(res);
            }
            
            return result;
        }
    }

    public static List<string> GetResolutionsChoices
    { 
        get 
        {
            List<string> list = new List<string>();
            foreach(Resolution res in resolutions)
            {
                string choice = res.width + "x" + res.height + " " + res.refreshRate + "Hz";
                list.Add(choice);
            }
            
            return list; 
        }
    }

    public static bool PlayWindSound 
    { 
        get { return playWindSound; } 
        set 
        { 
            playWindSound = value;
            PlayerAudioEffects playerAudio = Object.FindObjectOfType<PlayerAudioEffects>();
            if (playerAudio != null)
                playerAudio.PlayAccelerateSound = value;
        } 
    }

    public static bool ShowArrows 
    { 
        get { return showArrows; } 
        set 
        {
            Cloud player = Object.FindObjectOfType<Cloud>();
            if(player != null)
                player.SetArrowVisualizeState(value);
            CameraControl cameraControl = Object.FindObjectOfType<CameraControl>();
            if (cameraControl != null)
                cameraControl.InputVisualizeState(value);
            showArrows = value; 
        } 
    }

    public static bool ShowResultsPanel 
    { 
        get => showResultsPanel; 
        set
        {
            GameUI gameUI = Object.FindObjectOfType<GameUI>();
            if (gameUI != null)
                gameUI.FastRestart = !value;
            showResultsPanel = value;
        }
    }

    public static GameProgress.Difficulty LastSelectedDifficulty 
    { 
        get => lastSelectedDifficulty;
        set 
        {
            lastSelectedDifficulty = value;
            GameProgress progress = Object.FindObjectOfType<GameProgress>();
            if(progress != null)
            {
                progress.GetDifficulty = value;
            }
            SaveSettings();
        }
    }

    public static int LanguageIndex { get => languageIndex; set => languageIndex = value; }

    public static void SaveSettings()
    {
        int playWindSoundInt = 0;
        if (playWindSound)
            playWindSoundInt = 1;
        int showArrowsInt = 0;
        if(showArrows)
            showArrowsInt = 1;
        int showResultsPanelInt = 0;
        if (showResultsPanel)
            showResultsPanelInt = 1;

        PlayerPrefs.SetInt("playWindSound", playWindSoundInt);
        PlayerPrefs.SetInt("showArrows", showArrowsInt);
        PlayerPrefs.SetInt("showResultsPanel", showResultsPanelInt);
        PlayerPrefs.SetInt("lastSelectedDifficulty", (int)lastSelectedDifficulty);
        PlayerPrefs.SetInt("languageIndex", languageIndex);
        PlayerPrefs.SetFloat("impulseSens", impulseRatio);
    }

    public static float ImpulseRatio 
    { 
        get
        {
            Cloud player = Object.FindObjectOfType<Cloud>();
            return player.BaseImpulseRatio;
        }
        set
        {
            Cloud player = Object.FindObjectOfType<Cloud>();
            player.BaseImpulseRatio = value;
            impulseRatio = player.BaseImpulseRatio;
        }
    }

    public static void LoadSettings()
    {
        languageIndex = PlayerPrefs.GetInt("languageIndex", 0);
        int playWindSoundInt = PlayerPrefs.GetInt("playWindSound", 1);
        int showArrowsInt = PlayerPrefs.GetInt("showArrows", 1);
        int showResultsPanelInt = PlayerPrefs.GetInt("showResultsPanel", 1);
        ImpulseRatio = PlayerPrefs.GetFloat("impulseSens", 2f);
        lastSelectedDifficulty = (GameProgress.Difficulty)PlayerPrefs.GetInt("lastSelectedDifficulty", 0);
        PlayWindSound = playWindSoundInt == 1;
        ShowArrows = showArrowsInt == 1;
        ShowResultsPanel = showResultsPanelInt == 1;
    }
}
