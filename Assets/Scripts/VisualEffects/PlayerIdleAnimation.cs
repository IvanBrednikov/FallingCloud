using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleAnimation : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] GameObject bossArena;
    [SerializeField] Animator animator;

    private void Update()
    {
        animator.SetBool("PlayerFreeze", player.PlayerFreezed && !bossArena.activeSelf);
        animator.enabled = player.PlayerFreezed && !bossArena.activeSelf;
    }
}
