using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PanelManager panelManager;
    [SerializeField] GameUI gameUI;
    [SerializeField] GameProgress gameProgress;
    [SerializeField] PlayerStatistic statistic;

    Button startButton;
    Button easyButton;
    Button normalButton;
    Button hardButton;

    Label attemptValue;
    Button tutorialButton;
    Button optionsButton;
    Button exitButton;

    public delegate void DifficultyButtonHandler(int difficulty);
    public event DifficultyButtonHandler OnDifficultySelected;
    void OnEnable() => GetComponent<UIDocumentLocalization>().onCompleted += Bind;
    void OnDisable() => GetComponent<UIDocumentLocalization>().onCompleted -= Bind;

    void Bind()
    {
        startButton = (Button)doc.rootVisualElement.Query("StartButton");
        attemptValue = (Label)doc.rootVisualElement.Query("AttemptsLabel");
        tutorialButton = (Button)doc.rootVisualElement.Query("TutorialButton");
        optionsButton = (Button)doc.rootVisualElement.Query("OptionsButton");
        exitButton = (Button)doc.rootVisualElement.Query("ExitButton");

        startButton.clicked += StartButton_clicked;
        optionsButton.clicked += OptionsButton_clicked;
        exitButton.clicked += ExitButton_clicked;

        easyButton = (Button)doc.rootVisualElement.Query("Easy");
        normalButton = (Button)doc.rootVisualElement.Query("Normal");
        hardButton = (Button)doc.rootVisualElement.Query("Hard");

        normalButton.clicked += NormalButton_clicked;
        easyButton.clicked += EasyButton_clicked;
        hardButton.clicked += HardButton_clicked;
        tutorialButton.clicked += TutorialButton_clicked;

        UpdateAttempts();
        SelectDifficulty(GameSettings.LastSelectedDifficulty);
    }

    private void TutorialButton_clicked()
    {
        panelManager.SetMainMenuState(false);
        panelManager.SetTutorialPanelState(true);
    }

    public void UpdateAttempts()
    {
        statistic.LoadStatistic();
        attemptValue.text = statistic.Attempts.ToString();
    }

    private void ExitButton_clicked()
    {
        Application.Quit();
    }

    private void OptionsButton_clicked()
    {
        panelManager.SetOptionsState(true);
        panelManager.SetMainMenuState(false);
    }

    private void StartButton_clicked()
    {
        gameUI.PressStartGame();
        panelManager.SetLevelScreenState(true);
        panelManager.SetMainMenuState(false);
    }

    private void HardButton_clicked()
    {
        SelectDifficulty(GameProgress.Difficulty.Hard);
    }

    private void NormalButton_clicked()
    {
        SelectDifficulty(GameProgress.Difficulty.Normal);
    }

    private void EasyButton_clicked()
    {
        SelectDifficulty(GameProgress.Difficulty.Easy);
    }

    void SelectDifficulty(GameProgress.Difficulty difficulty)
    {
        easyButton.RemoveFromClassList("button-difficulty-easy");
        normalButton.RemoveFromClassList("button-difficulty-normal");
        hardButton.RemoveFromClassList("button-difficulty-hard");

        switch (difficulty)
        {
            case GameProgress.Difficulty.Easy:
                easyButton.AddToClassList("button-difficulty-easy");
                break;
            case GameProgress.Difficulty.Normal:
                normalButton.AddToClassList("button-difficulty-normal");
                break;
            case GameProgress.Difficulty.Hard:
                hardButton.AddToClassList("button-difficulty-hard");
                break;
        }

        GameSettings.LastSelectedDifficulty = difficulty;
    }
}
