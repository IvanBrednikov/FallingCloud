using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerProgress : MonoBehaviour
{
    [SerializeField] Cloud player;

    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameUI gameUI;
    [SerializeField] DeathnessTornado deathnessTornado;
    [SerializeField] CameraControl cameraControl;
    [SerializeField] GameProgress gameProgress;

    [SerializeField] int _currentPoints = 0;
    [SerializeField] int playerHealth = 0;
    [SerializeField] int maxHealth = 10;
    [SerializeField] int nextHealthPoints;
    [SerializeField] PlayerAbility[] playerAbilities;
    [SerializeField] Stage[] stages;
    int currentStageIndex = 0;
    [SerializeField] StageDifficultyModifiers[] stageDifficultyModifiers;

    public event SimpleEvent OnPlayerDie;
    public event SimpleEvent OnPlayerAbilityGain;
    public event SimpleEvent OnPlayerAbilityLoss;

    [SerializeField] PlayerStatistic statistics;
    [SerializeField] int altutudeThresholdBonus= 15;
    [SerializeField] int ySpeedThresholdBonus = 20;

    bool gameEndDamageIgnore = false;

    private void Start()
    {
        player.OnGainFood += Player_OnFoodIncrease;
        player.OnObstacleCollide += Player_OnObstacleCollide;
        Points = _currentPoints;
    }

    public int Points
    {
        get { return _currentPoints; }
        private set
        {
            int difference = value - _currentPoints;
            if(difference > 0)
            {
                nextHealthPoints += difference;
            }

            if (value > 0)
                _currentPoints = value;
            else
                _currentPoints = 0;
            HealthUpdate();
        }
    }

    private void Player_OnObstacleCollide(string tag)
    {
        if(!gameEndDamageIgnore)
        {
            if (tag == "floor")
                CloudGainDamage(100);
            else
                CloudGainDamage();

            statistics.SufferedDamage++;
            if (tag == "Boss" || tag == "Projectile")
                statistics.SufferedDamageByBoss++;
            StageUpdate();
        }
    }

    private void Player_OnFoodIncrease(int points)
    {
        Points += points * GetBonusModifier;
        statistics.TotalFoodEaten++;
    }

    void CheckAbilitiesAvailable()
    {
        bool newAbilityGain = false;
        bool abilityLoss = false;

        foreach(PlayerAbility ability in playerAbilities)
        {
            if (playerHealth >= ability.GainTreshold)
            {
                if (ability.AbilityAvailable == false)
                    newAbilityGain = true;
                ability.AbilityAvailable = true;
            }
                

            if (playerHealth <= ability.LossTreshold)
            {
                if (ability.AbilityAvailable == true)
                    abilityLoss = true;
                ability.AbilityAvailable = false;
            }
                
        }

        if (newAbilityGain && OnPlayerAbilityGain != null)
            OnPlayerAbilityGain();
        if (abilityLoss && OnPlayerAbilityLoss != null)
            OnPlayerAbilityLoss();
    }

    void ResetAbilities()
    {
        foreach (PlayerAbility ability in playerAbilities)
            ability.AbilityAvailable = false;
    }

    Stage CurrentStage { get { return stages[currentStageIndex];} }
    public bool RainIsAvailable 
    {
        get 
        {
            bool result = false;
            bool found = false;
            foreach(PlayerAbility ability in playerAbilities)
                if(ability.name == "Rain")
                {
                    found = true;
                    result = ability.AbilityAvailable;
                }

            if (!found)
                throw new System.Exception("PlayerAbility \"Rain\" not found");
            return result;
        } 
    }

    public bool LightninhIsAvailable 
    {
        get
        {
            bool result = false;
            bool found = false;
            foreach (PlayerAbility ability in playerAbilities)
                if (ability.name == "Lightning")
                {
                    found = true;
                    result = ability.AbilityAvailable;
                }

            if (!found)
                throw new System.Exception("PlayerAbility \"Lightning\" not found");
            return result;
        }
    }

    public bool TornadoIsAvailable 
    {
        get
        {
            bool result = false;
            bool found = false;
            foreach (PlayerAbility ability in playerAbilities)
                if (ability.name == "Tornado")
                {
                    found = true;
                    result = ability.AbilityAvailable;
                }

            if (!found)
                throw new System.Exception("PlayerAbility \"Tornado\" not found");
            return result;
        }
    }

    void HealthUpdate()
    {
        if (nextHealthPoints >= PointsPerHealth)
        {
            if (playerHealth < maxHealth)
            {
                playerHealth++;
                nextHealthPoints = 0;
                statistics.HealthEarn++;
            }
            CheckAbilitiesAvailable();
            StageUpdate();
        }
    }

    void StageUpdate()
    {
        //stage up check
        int nextStageIndex = currentStageIndex + 1;
        if (nextStageIndex < stages.Length)
        {
            if (playerHealth >= stages[nextStageIndex].minHealth)
            {
                currentStageIndex = nextStageIndex;
            }
        }

        //stage down check
        int prevoiousStageIndex = currentStageIndex - 1;
        bool isLastStage = currentStageIndex == stages.Length - 1;
        if (prevoiousStageIndex >= 0 && !isLastStage)
        {
            if (playerHealth < stages[currentStageIndex].minHealth)
                currentStageIndex = prevoiousStageIndex;
        }
    }

    public int Health
    {
        get { return playerHealth; }
    }

    public int NextHealthPoints { get => nextHealthPoints; }

    public int PointsPerHealth { get { return ((int)(CurrentStage.pointsPerHealth * GetCurrentDifficultyModifier)); } }

    public bool GameEndDamageIgnore { get => gameEndDamageIgnore; set => gameEndDamageIgnore = value; }

    public int GetBonusModifier 
    {
        get
        {
            int result = 1;

            if (CheckSpeedBonus)
                result *= 2;

            if (CheckAltitudeBonus)
                result *= 2;
            return result;
        } 
    }

    public bool CheckSpeedBonus { get => !gameProgress.IsBossFight && player.HorizontalSpeed >= ySpeedThresholdBonus; }

    public bool CheckAltitudeBonus { get => !gameProgress.IsBossFight && player.transform.position.y >= altutudeThresholdBonus; }

    void CloudGainDamage()
    {
        playerHealth--;
        if (playerHealth < 0)
            PlayerDie();
        CheckAbilitiesAvailable();
    }

    void CloudGainDamage(int healthDamage)
    {
        playerHealth -= healthDamage;
        if (playerHealth < 0)
            PlayerDie();
        CheckAbilitiesAvailable();
    }

    void PlayerDie()
    {
        player.gameObject.SetActive(false);
        StopAllCoroutines();
        if (OnPlayerDie != null)
            OnPlayerDie();
        playerHealth = 0;
    }

    public void ProgressRestart()
    {
        _currentPoints = 0;
        playerHealth = 0;
        nextHealthPoints = 0;
        currentStageIndex = 0;
        ResetAbilities();
    }

    float GetCurrentDifficultyModifier 
    {
        get
        {
            GameProgress.Difficulty currentDifficulty = gameProgress.GetDifficulty;
            bool found = false;
            float result = 1;
            foreach(StageDifficultyModifiers modifier in stageDifficultyModifiers)
                if(currentDifficulty == modifier.difficulty)
                {
                    result = modifier.modifier;
                    found = true;
                    break;
                }

            if (found)
                return result;
            else
                throw new System.Exception("Player stage difficulty modifier for \"" + 
                    currentDifficulty.ToString() + "\" difficulty not found");
        }
    }
}

[System.Serializable]
public class PlayerAbility
{
    public string name;
    [SerializeField] int gainTreshold; //equal to gain
    [SerializeField] int lossTreshold; //equal to loss
    bool abilityAvailable;
    public UnityEvent OnAbilityGain;
    public UnityEvent OnAbilityLoss;

    public int GainTreshold { get { return gainTreshold; } }

    public int LossTreshold { get { return lossTreshold; } }

    public bool AbilityAvailable { 
        get { return abilityAvailable; }
        set 
        {
            if(abilityAvailable != value)
            {
                if (value)
                    OnAbilityGain.Invoke();
                else
                    OnAbilityLoss.Invoke();
            }
            abilityAvailable = value; 
        } 
    }

    public PlayerAbility(string name, int gainTreshold, int lossTreshold, int pointsToGainHealth)
    {
        this.name = name;
        this.gainTreshold = gainTreshold;
        this.lossTreshold = lossTreshold;
    }
}
[System.Serializable]
public class Stage
{
    public int minHealth; //minimum health to start stage
    public int pointsPerHealth; //point to gain 1 health
}
[System.Serializable]
public class StageDifficultyModifiers
{
    public string itemName;
    public GameProgress.Difficulty difficulty;
    public float modifier;
}