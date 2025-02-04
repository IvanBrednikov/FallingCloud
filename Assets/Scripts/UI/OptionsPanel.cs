using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PanelManager panelManager;
    [SerializeField] GameUI gameUI;
    [SerializeField] AudioSettings audioSettings;

    Button doneButton;
    Button returnSens;
    DropdownField languageDropDown;
    DropdownField resolutionDropDown;
    Slider masterVolume;
    Slider effectVolume;
    Slider musicVolume;
    Slider impulseSens;
    Toggle fullScreen;
    Toggle playWindSound;
    Toggle showArrows;
    Toggle showResultsPanel;


    void OnEnable() => GetComponent<UIDocumentLocalization>().onCompleted += Bind;

    void Bind ()
    {
        doneButton = (Button)doc.rootVisualElement.Query("DoneButton");
        returnSens = (Button)doc.rootVisualElement.Query("ReturnSens");
        languageDropDown = (DropdownField)doc.rootVisualElement.Query("Language");
        resolutionDropDown = (DropdownField)doc.rootVisualElement.Query("ScreenResolution");
        masterVolume = (Slider)doc.rootVisualElement.Query("Master");
        effectVolume = (Slider)doc.rootVisualElement.Query("Effects");
        musicVolume = (Slider)doc.rootVisualElement.Query("Music");
        fullScreen = (Toggle)doc.rootVisualElement.Query("FullScreen");
        playWindSound = (Toggle)doc.rootVisualElement.Query("WindSoundOn");
        showArrows = (Toggle)doc.rootVisualElement.Query("ShowArrows");
        showResultsPanel = (Toggle)doc.rootVisualElement.Query("ShowResultsPanel");
        impulseSens = (Slider)doc.rootVisualElement.Query("ImpulsSensevity");

        resolutionDropDown.choices = GameSettings.GetResolutionsChoices;
        resolutionDropDown.RegisterValueChangedCallback(ResolutionChanged);
        fullScreen.RegisterValueChangedCallback(FullScreenChanged);

        doneButton.clicked += DoneButton_clicked;
        returnSens.clicked += ReturnSens_clicked;
        playWindSound.RegisterValueChangedCallback(PlayWindSoundValueChanged);
        showArrows.RegisterValueChangedCallback(ShowArrowsValueChanged);
        showResultsPanel.RegisterValueChangedCallback(ShowResultsPanelValueChanged);
        masterVolume.RegisterValueChangedCallback(MasterVolumeValueChanged);
        effectVolume.RegisterValueChangedCallback(EffectsVolumeValueChanged);
        musicVolume.RegisterValueChangedCallback(MusicVolumeValueChanged);
        impulseSens.RegisterValueChangedCallback(ImpulseSensevityChanged);

        fullScreen.value = Screen.fullScreen;
        playWindSound.value = GameSettings.PlayWindSound;
        showArrows.value = GameSettings.ShowArrows;
        resolutionDropDown.value = GameSettings.GetCurrentResolutionChoice();
        impulseSens.value = GameSettings.ImpulseRatio;

        if(GameSettings.LanguageIndex >= 0)
        {
            languageDropDown.value = LocalizationSettings.AvailableLocales.Locales[GameSettings.LanguageIndex].LocaleName;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[GameSettings.LanguageIndex];
        }
        languageDropDown.RegisterValueChangedCallback(LanguageValueChanged);

        masterVolume.value = audioSettings.MasterVolume;
        effectVolume.value = audioSettings.EffectsVolume;
        musicVolume.value = audioSettings.MusicVolume;

        if (Application.platform == RuntimePlatform.Android)
        {
            resolutionDropDown.visible = false;
        }
    }

    private void ReturnSens_clicked()
    {
        impulseSens.value = 2;
    }

    private void DoneButton_clicked()
    {
        if (gameUI.GameInProgress)
            panelManager.SetPausePanelState(true);
        else
            panelManager.SetMainMenuState(true);
        
        panelManager.SetOptionsState(false);
        
        GameSettings.SaveSettings();
        audioSettings.SaveSettings();
    }

    private void ResolutionChanged(ChangeEvent<string> e)
    {
        GameSettings.SetScreenResolution(resolutionDropDown.index, fullScreen.value);
    }

    private void FullScreenChanged(ChangeEvent<bool> e)
    {
        GameSettings.SetScreenResolution(resolutionDropDown.index, e.newValue);
    }

    private void PlayWindSoundValueChanged(ChangeEvent<bool> e)
    {
        GameSettings.PlayWindSound = e.newValue;
    }

    private void ShowArrowsValueChanged(ChangeEvent<bool> e)
    {
        GameSettings.ShowArrows = e.newValue;
    }

    private void ShowResultsPanelValueChanged(ChangeEvent<bool> e)
    {
        GameSettings.ShowResultsPanel = e.newValue;
    }

    private void MasterVolumeValueChanged(ChangeEvent<float> e)
    {
        audioSettings.MasterVolume = e.newValue;
    }

    private void EffectsVolumeValueChanged(ChangeEvent<float> e)
    {
        audioSettings.EffectsVolume = e.newValue;
    }

    private void MusicVolumeValueChanged(ChangeEvent<float> e)
    {
        audioSettings.MusicVolume = e.newValue;
    }

    public void UpdateSettings()
    {
        if(showResultsPanel != null)
            showResultsPanel.value = GameSettings.ShowResultsPanel;
    }

    private void LanguageValueChanged(ChangeEvent<string> e)
    {
        if(languageDropDown.index >= 0)
        {
            GameSettings.LanguageIndex = languageDropDown.index;
            GameSettings.SaveSettings();
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageDropDown.index];
        }
    }

    private void ImpulseSensevityChanged(ChangeEvent<float> e)
    {
        GameSettings.ImpulseRatio = e.newValue;
    }
}
