using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

public class ResultsPanel : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PanelManager panelManager;
    [SerializeField] GameUI gameUI;
    [SerializeField] PlayerStatistic statistic;

    Button gameExitButton;
    Button menuButton;
    Button restartButton;
    Toggle showResultsAgain;

    //statsLables
    //always shown
    Label totalPoints;
    Label distanceComplete;
    Label totalFoodEaten;
    Label sufferedDamage;
    Label healthEarn;
    Label difficulty;
    Label portalRised;
    Label cloudSize;
    Label maxSpeed;
    Label attempt;
    Label timeElapsed;

    //dynamic shown
    VisualElement obstacleDestroyed;
    VisualElement projectilesDestroyed;
    VisualElement foodByRain;
    VisualElement objectsByTornado;
    VisualElement bossStageComplete;
    VisualElement goldArmsOwner;
    VisualElement damageByBoss;

    void OnEnable() => GetComponent<UIDocumentLocalization>().onCompleted += Bind;

    void Bind()
    {
        gameExitButton = (Button)doc.rootVisualElement.Query("GameExitButton");
        menuButton = (Button)doc.rootVisualElement.Query("MenuButton");
        restartButton = (Button)doc.rootVisualElement.Query("RestartButton");
        showResultsAgain = (Toggle)doc.rootVisualElement.Query("ShowResultsAgain");

        gameExitButton.clicked += GameExitButton_clicked;
        menuButton.clicked += MenuButton_clicked;
        restartButton.clicked += RestartButton_clicked;
        showResultsAgain.RegisterValueChangedCallback(ShowResultsAgainValueChanged);

        //stats
        totalPoints = doc.rootVisualElement.Query("TotalPoints").Children<Label>("Value");
        distanceComplete = doc.rootVisualElement.Query("Distance").Children<Label>("Value");
        totalFoodEaten = doc.rootVisualElement.Query("TotalFoodEaten").Children<Label>("Value");
        sufferedDamage = doc.rootVisualElement.Query("SufferedDamage").Children<Label>("Value");
        healthEarn = doc.rootVisualElement.Query("HealthEarn").Children<Label>("Value");
        difficulty = doc.rootVisualElement.Query("Difficulty").Children<Label>("Value");
        portalRised = doc.rootVisualElement.Query("PortalRised").Children<Label>("Value");
        cloudSize = doc.rootVisualElement.Query("CloudSize").Children<Label>("Value");
        maxSpeed = doc.rootVisualElement.Query("MaxSpeed").Children<Label>("Value");
        attempt = doc.rootVisualElement.Query("Attempts").Children<Label>("Value");
        timeElapsed = doc.rootVisualElement.Query("TimeElapsed").Children<Label>("Value");

        obstacleDestroyed = doc.rootVisualElement.Query("ObstaclesDestroyed");
        projectilesDestroyed = doc.rootVisualElement.Query("ProjectilesDestroyed");
        foodByRain = doc.rootVisualElement.Query("FoodEatenByRain");
        objectsByTornado = doc.rootVisualElement.Query("ObjectsMovedByTornado");
        bossStageComplete = doc.rootVisualElement.Query("BossStageComplete");
        goldArmsOwner = doc.rootVisualElement.Query("GoldArmsOwner");
        damageByBoss = doc.rootVisualElement.Query("SufferedDamageByBoss");
    }

    private void RestartButton_clicked()
    {
        gameUI.PressRestartGame();
        panelManager.SetResultPanelState(false);
        panelManager.SetLevelScreenState(true);
    }

    private void MenuButton_clicked()
    {
        gameUI.PressReturnToMenu();
        panelManager.SetResultPanelState(false);
        panelManager.SetMainMenuState(true);
    }

    public void LoadStatistic()
    {
        string localizedYes = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "yes");
        string localaziedNo = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "no");
        string localizedEasy = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "easy");
        string localizedNormal = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "normal");
        string localiazedHard = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "hard");
        string localizedSize1 = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "size1");
        string localizedSize2 = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "size2");
        string localizedSize3 = LocalizationSettings.StringDatabase.GetLocalizedString("AllStrings", "size3");

        string difficultyS;
        switch (statistic.Difficulty)
        {
            case "Easy":
                difficultyS = localizedEasy;
                break;
            case "Normal":
                difficultyS = localizedNormal;
                break;
            case "Hard":
                difficultyS = localiazedHard;
                break;
            default:
                difficultyS = localizedEasy;
                break;
        }
        difficulty.text = difficultyS;

        string localizedSize;
        switch (statistic.GetCloudSize)
        {
            case "size1":
                localizedSize = localizedSize1;
                break;
            case "size2":
                localizedSize = localizedSize2;
                break;
            case "size3":
                localizedSize = localizedSize3;
                break;
            default:
                localizedSize = localizedSize1;
                break;
        }
        cloudSize.text = localizedSize;

        totalPoints.text = statistic.TotalPoints.ToString();
        distanceComplete.text = statistic.DisntaceComplete.ToString();
        totalFoodEaten.text = statistic.TotalFoodEaten.ToString();
        sufferedDamage.text = statistic.SufferedDamage.ToString();
        healthEarn.text = statistic.HealthEarn.ToString();
        portalRised.text = statistic.PortalRised ? localizedYes : localaziedNo;
        maxSpeed.text = statistic.MaxSpeed.ToString();
        attempt.text = statistic.Attempts.ToString();
        timeElapsed.text = statistic.LevelElapsedTimeFormatted;

        if(statistic.ObstacleDestroyedByLightning > 0)
        {
            ((Label)obstacleDestroyed.Query("Value")).text = statistic.ObstacleDestroyedByLightning.ToString();
            obstacleDestroyed.style.display = DisplayStyle.Flex;
        }
        else
            obstacleDestroyed.style.display = DisplayStyle.None;

        if (statistic.ProjectileDestroyedByLightning > 0)
        {
            ((Label)projectilesDestroyed.Query("Value")).text = statistic.ProjectileDestroyedByLightning.ToString();
            projectilesDestroyed.style.display = DisplayStyle.Flex;
        }
        else
            projectilesDestroyed.style.display = DisplayStyle.None;
        
        if(statistic.FoodEarnByRain > 0)
        {
            ((Label)foodByRain.Query("Value")).text = statistic.FoodEarnByRain.ToString();
            foodByRain.style.display = DisplayStyle.Flex;
        }
        else
            foodByRain.style.display = DisplayStyle.None;

        if (statistic.ObjectsMovedByTornado > 0)
        {
            ((Label)objectsByTornado.Query("Value")).text = statistic.ObjectsMovedByTornado.ToString();
            objectsByTornado.style.display = DisplayStyle.Flex;
        }
        else
            objectsByTornado.style.display = DisplayStyle.None;

        if (statistic.PortalRised)
        {
            ((Label)bossStageComplete.Query("Value")).text = statistic.BossComplete ? localizedYes : localaziedNo;
            bossStageComplete.style.display = DisplayStyle.Flex;
        }
        else
            bossStageComplete.style.display = DisplayStyle.None;

        if(statistic.ShowGoldArmsOwner)
            goldArmsOwner.style.display = DisplayStyle.Flex;
        else
            goldArmsOwner.style.display = DisplayStyle.None;

        if(statistic.PortalRised)
        {
            ((Label)damageByBoss.Query("Value")).text = statistic.SufferedDamageByBoss.ToString();
            damageByBoss.style.display = DisplayStyle.Flex;
        }
        else
            damageByBoss.style.display = DisplayStyle.None;

        showResultsAgain.value = GameSettings.ShowResultsPanel;
    }

    private void GameExitButton_clicked()
    {
        Application.Quit();
    }

    private void ShowResultsAgainValueChanged(ChangeEvent<bool> e)
    {
        GameSettings.ShowResultsPanel = e.newValue;
        GameSettings.SaveSettings();
    }
}
