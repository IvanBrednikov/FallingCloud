using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioEffects : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerProgress playerLogic;

    [SerializeField] AudioSource accelerate, foodGain, damagedGain, 
        die, abilityGain, abilityLoss, teleportation;

    [SerializeField] bool playAccelerateSound = true;

    public bool PlayAccelerateSound { get => playAccelerateSound; set => playAccelerateSound = value; }

    private void Start()
    {
        playerInput.OnEndPosSelected += PlayerInput_OnEndPosSelected;
        player.OnGainFood += Player_OnGainFood;
        player.OnObstacleCollide += Player_OnObstacleCollide;
        playerLogic.OnPlayerDie += PlayerLogic_OnPlayerDie;
        playerLogic.OnPlayerAbilityGain += PlayerLogic_OnPlayerAbilityGain;
        playerLogic.OnPlayerAbilityLoss += PlayerLogic_OnPlayerAbilityLoss;
        player.OnTeleportation += Player_OnTeleportation;
    }

    private void Player_OnTeleportation()
    {
        teleportation.Play();
    }

    private void PlayerLogic_OnPlayerAbilityLoss()
    {
        abilityLoss.Play();
    }

    private void PlayerLogic_OnPlayerAbilityGain()
    {
        abilityGain.Play();
    }

    private void PlayerLogic_OnPlayerDie()
    {
        die.Play();
    }

    private void Player_OnObstacleCollide(string tagCollidedObject)
    {
        damagedGain.Play();
    }

    private void Player_OnGainFood(int points)
    {
        foodGain.Play();
    }

    private void PlayerInput_OnEndPosSelected(Vector2 vector)
    {
        if(playAccelerateSound)
            accelerate.Play();
    }
}
