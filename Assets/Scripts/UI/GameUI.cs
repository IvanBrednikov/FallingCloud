using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameUI : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] PlayerProgress progress;
    [SerializeField] GameProgress gameProgress;
    [SerializeField] bool fastRestart;
    [SerializeField] PanelManager panels;
    [SerializeField] ResultsPanel resultsPanel;
    [SerializeField] PlayerStatistic statistic;

    float rainTimer;
    float lightningTimer;
    float tornadoTimer;

    private void Start()
    {
        progress.OnPlayerDie += Progress_OnPlayerDie;
        player.OnRainActivate += Player_OnRainActivate;
        player.OnLightningActivate += Player_OnLightningActivate;
        player.OnTornadoActivate += Player_OnTornadoActivate;
    }

    private void Player_OnTornadoActivate()
    {
        tornadoTimer = player.TornadoRechargeTime;
    }

    private void Player_OnLightningActivate()
    {
        lightningTimer = player.LightningRechargeTime;
    }

    private void Player_OnRainActivate()
    {
        rainTimer = player.RainRechargeTime;
    }

    private void Progress_OnPlayerDie()
    {
        ShowRestartMenu();
    }

    public void ShowRestartMenu()
    {
        if (fastRestart)
        {
            gameProgress.RestartGame();
        }
        else
        {
            panels.SetLevelScreenState(false);
            panels.SetResultPanelState(true);
            resultsPanel.LoadStatistic();
        }       
    }

    public void ShowMainMenu()
    {
        panels.SetMainMenuState(true);
    }

    public void PressStartGame()
    {
        gameProgress.StartGame();
        statistic.UpdateStatistic();
    }
    public void PressRestartGame()
    {
        gameProgress.RestartGame();
        statistic.UpdateStatistic();
    }

    public void PressReturnToMenu()
    {
        gameProgress.ExitToMainMenu();
    }

    public void PauseMenuActivate()
    {
        panels.SetPausePanelState(true);
    }

    public void PauseMenuClose()
    {
        panels.SetPausePanelState(false);
    }

    public void OptionsClose()
    {
        panels.SetOptionsState(false);
    }

    public void UnPause()
    {
        gameProgress.UnPause();
    }

    public void SetLevelSceenActive(bool state)
    {
        panels.SetLevelScreenState(state);
    }

    public bool GameInProgress { get { return !player.PlayerFreezed; } }

    private void Update()
    {
        if (rainTimer > 0 && !player.RainIsActive)
            rainTimer -= Time.deltaTime;
        if (lightningTimer > 0 && !player.LightninhIsActive)
            lightningTimer -= Time.deltaTime;
        if (tornadoTimer > 0 && !player.TornadoIsActive)
            tornadoTimer -= Time.deltaTime;
    }

    public float RainChargeLevel { get { return 1 - (rainTimer / player.RainRechargeTime); } }

    public float LightninhChargeLevel { get { return 1 - (lightningTimer / player.LightningRechargeTime); } }

    public float TornadoChargeLevel { get { return 1 - ( tornadoTimer / player.TornadoRechargeTime); } }

    public bool FastRestart { get => fastRestart; set => fastRestart = value; }

    public float NextHealthProgress { get => (progress.NextHealthPoints / (float)progress.PointsPerHealth); }
}
