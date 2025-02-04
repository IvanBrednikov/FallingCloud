using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistic : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] BossAI boss;
    [SerializeField] PlayerProgress progress;
    [SerializeField] CloudSize cloudSize;
    [SerializeField] GameProgress gameProgress;

    [SerializeField] Rigidbody2D playerRb;

    int attempts;
    int obstacleDestroyedByLightning;
    int projectileDestroyedByLightning;
    int totalFoodEaten;
    int foodEarnByRain;
    int objectsMovedByTornado;
    int sufferedDamage;
    int healthEarn;
    float maxSpeed;
    int sufferedDamageByBoss;
    bool portalRised;
    CloudSize.ESize maxCloudSize;
    float maxDistance;
    float levelElapsedTime;

    public bool BossComplete { get { return PortalRised && !boss.gameObject.activeSelf; } }
    public bool ShowGoldArmsOwner { get { return BossComplete && Difficulty == "Hard"; } }
    public string Difficulty
    {
        get
        {
            string result = "Easy";
            if (gameProgress.GetDifficulty == GameProgress.Difficulty.Easy)
                result = "Easy";
            if (gameProgress.GetDifficulty == GameProgress.Difficulty.Normal)
                result = "Normal";
            if (gameProgress.GetDifficulty == GameProgress.Difficulty.Hard)
                result = "Hard";

            return result;
        }
    }
    public int TotalPoints { get { return progress.Points; } }
    public int MaxSpeed { get { return Mathf.RoundToInt(maxSpeed); } }
    public int HealthEarn { get => healthEarn; set => healthEarn = value; }
    public int SufferedDamage { get => sufferedDamage; set => sufferedDamage = value; }
    public int SufferedDamageByBoss { get => sufferedDamageByBoss; set => sufferedDamageByBoss = value; }
    public int TotalFoodEaten { get => totalFoodEaten; set => totalFoodEaten = value; }
    public int ObjectsMovedByTornado { get => objectsMovedByTornado; set => objectsMovedByTornado = value; }
    public int FoodEarnByRain { get => foodEarnByRain; set => foodEarnByRain = value; }
    public int ProjectileDestroyedByLightning { get => projectileDestroyedByLightning; set => projectileDestroyedByLightning = value; }
    public int ObstacleDestroyedByLightning { get => obstacleDestroyedByLightning; set => obstacleDestroyedByLightning = value; }
    public int DisntaceComplete { get { return Mathf.RoundToInt(maxDistance); } }
    public bool PortalRised { get => portalRised; set => portalRised = value; }
    public string GetCloudSize
    {
        get
        {
            string result = "size1";
            if (maxCloudSize == CloudSize.ESize.Medium)
                result = "size2";
            if (maxCloudSize == CloudSize.ESize.Big)
                result = "size3";
            return result;
        }
    }
    public int Attempts { get => attempts; set => attempts = value; }
    public string LevelElapsedTimeFormatted
    {
        get
        {
            int minutes = Mathf.FloorToInt(levelElapsedTime / 60);
            int seconds = Mathf.FloorToInt(levelElapsedTime % 60);
            string result;
            if (minutes > 0)
                result = minutes + ":" + seconds;
            else
                result = levelElapsedTime.ToString("0.000");
            return result;
        }
    }

    

    public void ClearStatistic()
    {
        maxSpeed = 0;
        healthEarn = 0;
        totalFoodEaten = 0;
        sufferedDamage = 0;
        sufferedDamageByBoss = 0;
        objectsMovedByTornado = 0;
        foodEarnByRain = 0;
        obstacleDestroyedByLightning = 0;
        projectileDestroyedByLightning = 0;
        portalRised = false;
        maxDistance = 0;
        levelElapsedTime = 0;
    }

    private void OnEnable()
    {
        LoadStatistic();
    }

    public void UpdateStatistic()
    {
        attempts++;
        SaveStatistic();
    }

    private void Update()
    {
        if (playerRb.velocity.magnitude > maxSpeed)
            maxSpeed = playerRb.velocity.magnitude;
        if (cloudSize.GetSize > maxCloudSize)
            maxCloudSize = cloudSize.GetSize;
        if (!portalRised)
            maxDistance = player.transform.position.x - player.StartPosition.x;
        
        if(!player.PlayerFreezed)
            levelElapsedTime += Time.deltaTime;
    }

    public void SaveStatistic()
    {
        PlayerPrefs.SetInt("attempts", attempts);
    }

    public void LoadStatistic()
    {
        attempts = PlayerPrefs.GetInt("attempts", 0);
    }
}
