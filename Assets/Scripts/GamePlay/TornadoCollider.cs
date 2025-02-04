using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoCollider : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] PlayerStatistic statistic;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle obstacle;
        if(collision.TryGetComponent(out obstacle))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if(obstacle.ConvertToFood())
                player.CreateTornadoJoint(rb);
        }

        Food food;
        if(collision.TryGetComponent(out food))
        {
            if(!food.IsJointed)
            {
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                player.CreateTornadoJoint(rb);
            }
        }

        statistic.ObjectsMovedByTornado++;
    }

    private void OnDisable()
    {
        player.DestroyAllJoints();
    }
}
