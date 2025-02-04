using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PausePanel : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PanelManager panelManager;
    [SerializeField] GameUI gameUI;
    [SerializeField] MapGenerator mapGen;
    [SerializeField] PlayerStatistic statistic;
    [SerializeField] LocalizationAsset asset;

    Button resumeButton;
    Button optionsButton;
    Button returnToMenuButton;
    Button exitGameButton;
    Label mapSeedLabel;
    Label distanceLabel;

    void OnEnable() => GetComponent<UIDocumentLocalization>().onCompleted += Bind;

    void Bind ()
    {
        resumeButton = (Button)doc.rootVisualElement.Query("Resume");
        optionsButton = (Button)doc.rootVisualElement.Query("Options");
        returnToMenuButton = (Button)doc.rootVisualElement.Query("MainMenu");
        exitGameButton = (Button)doc.rootVisualElement.Query("ExitGame");
        mapSeedLabel = (Label)doc.rootVisualElement.Query("MapSeedLabel");
        distanceLabel = (Label)doc.rootVisualElement.Query("DistanceLabel");

        resumeButton.clicked += ResumeButton_clicked;
        optionsButton.clicked += OptionsButton_clicked;
        returnToMenuButton.clicked += ReturnToMenuButton_clicked;
        exitGameButton.clicked += ExitGameButton_clicked;
    }

    private void ExitGameButton_clicked()
    {
        Application.Quit();
    }

    private void ReturnToMenuButton_clicked()
    {
        gameUI.UnPause();
        panelManager.SetMainMenuState(true);
        panelManager.SetLevelScreenState(false);
        gameUI.PressReturnToMenu();
    }

    private void OptionsButton_clicked()
    {
        panelManager.SetOptionsState(true);
        panelManager.SetPausePanelState(false);
    }

    private void ResumeButton_clicked()
    {
        gameUI.UnPause();
        panelManager.SetPausePanelState(false);
        panelManager.SetLevelScreenState(true);
    }

    public void UpdateInfo()
    {
        string localizedMapSeed = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "seed");
        string localizedDistance = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "distanceDebug");

        if (mapSeedLabel != null)
            mapSeedLabel.text = localizedMapSeed + mapGen.Seed.ToString();
        if (distanceLabel != null)
            distanceLabel.text = localizedDistance + statistic.DisntaceComplete.ToString();
    }
}
