using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    [SerializeField] PlayerProgress playerProgress;
    [SerializeField] Cloud player;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameUI gameUI;
    [SerializeField] DeathnessTornado deathnessTornado;
    [SerializeField] CameraControl cameraControl;
    [SerializeField] PlayerStatistic statistic;
    [SerializeField] float reslutPanelDelay;

    //boss fight prepares
    [SerializeField] BossAI boss;
    [SerializeField] GameObject bossArena;

    public enum Difficulty {Easy, Normal, Hard}
    [SerializeField] Difficulty difficulty;

    bool pauseOn;

    public event SimpleEvent OnGameProgressReset;

    private void OnEnable()
    {
        GameSettings.LoadSettings();
    }

    public void RestartGame()
    {
        deathnessTornado.RestartGame();
        deathnessTornado.AutoControl = true;
        playerInput.enabled = true;
        playerProgress.ProgressRestart();
        player.SetUpStartProperties();
        player.UnFreezePlayer();
        mapGenerator.GenerateNewMap();
        cameraControl.RestartCamera();
        statistic.ClearStatistic();
        BossReset();

        OnGameProgressReset?.Invoke();
    }

    public void ExitToMainMenu()
    {
        deathnessTornado.RestartGame();
        playerProgress.ProgressRestart();
        player.SetUpStartProperties();
        mapGenerator.GenerateNewMap();
        playerInput.enabled = false;
        cameraControl.RestartCamera();
        cameraControl.enabled = false;
        statistic.ClearStatistic();
        BossReset();

        OnGameProgressReset?.Invoke();
    }

    public void StartGame()
    {
        player.UnFreezePlayer();
        playerInput.enabled = true;
        deathnessTornado.AutoControl = true;
        cameraControl.enabled = true;
    }

    public void PauseGame()
    {
        if(!player.PlayerFreezed && player.gameObject.activeSelf)
        {
            if (!pauseOn)
            {
                gameUI.PauseMenuActivate();
                Time.timeScale = 0;
                playerInput.enabled = false;
                pauseOn = true;
            }
            else
            {
                UnPause();
            }
                
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        gameUI.PauseMenuClose();
        gameUI.OptionsClose();
        pauseOn = false;
        playerInput.enabled = true;
    }

    public void BossFightStart()
    {
        mapGenerator.DestroyAllChunks();
        bossArena.SetActive(true);
        boss.gameObject.SetActive(true);
        deathnessTornado.gameObject.SetActive(false);
        cameraControl.SetBossCamera();
        boss.BossConfigure();
    }

    void BossReset()
    {
        bossArena.SetActive(false);
        boss.gameObject.SetActive(false);
        deathnessTornado.gameObject.SetActive(true);
        boss.DestroyProjectiles();
    }

    public void LevelComplete()
    {
        StartCoroutine(ShowResultsMenuDelay());
        playerProgress.GameEndDamageIgnore = true;
    }

    IEnumerator ShowResultsMenuDelay()
    {
        yield return new WaitForSeconds(reslutPanelDelay);
        player.FreezePlayer();
        playerInput.enabled = false;
        gameUI.ShowRestartMenu();
        playerProgress.GameEndDamageIgnore = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public Difficulty GetDifficulty { get { return difficulty; } set { difficulty = value; } }

    public bool IsBossFight { get => boss.gameObject.activeSelf; }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
    }
}
