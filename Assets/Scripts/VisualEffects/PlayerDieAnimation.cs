using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieAnimation : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] CloudSize cloudSize;
    [SerializeField] GameObject animationPrefab;
    [SerializeField] PlayerProgress progress;

    private void Start()
    {
        progress.OnPlayerDie += Player_OnPlayerDie;
    }

    private void Player_OnPlayerDie()
    {
        GameObject animationObject = Instantiate(animationPrefab);
        animationObject.transform.position = player.transform.position;
        PlayerDamageAnimation dAnimation = animationObject.GetComponent<PlayerDamageAnimation>();
        dAnimation.player = player;
        dAnimation.playerSize = cloudSize;
        dAnimation.PlayAnimation();
    }
}
