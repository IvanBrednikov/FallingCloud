using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAttack : BossAttack
{
    public override void Activate()
    {
        base.Activate();
        pointToMove = player.transform.position;
    }
}
