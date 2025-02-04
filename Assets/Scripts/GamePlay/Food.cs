using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] float foodCount;
    [SerializeField] float selfDestroyTime = 10f;

    [SerializeField] bool isJointed = false;

    public float FoodCount { get { return foodCount; } 
        set
        {
            if(value > 0)
                foodCount = value;
            else
                foodCount = 1;
        } 
    }

    public void FoodConsume(Cloud player)
    {
        player.DesttroyTornadoJoint(name);
        player.FoodConsume(foodCount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cloud player = collision.GetComponent<Cloud>();
        RainCollider rainCollider = collision.GetComponent<RainCollider>();

        if (player == null && rainCollider != null)
            player = rainCollider.player;

        if(player != null)
        {
            FoodConsume(player);
            if (collision.gameObject.name == "RainCollider")
                player.statistic.FoodEarnByRain++;
        }
    }

    public IEnumerator DestroyOnTime()
    {
        yield return new WaitForSeconds(selfDestroyTime);
        Destroy(gameObject);
    }

    public bool IsJointed
    {
        get { return isJointed; }
        set { isJointed = value; }
    }
}
