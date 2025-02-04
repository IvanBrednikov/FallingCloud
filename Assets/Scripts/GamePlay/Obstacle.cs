using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D obstacleCollider;
    [SerializeField] bool destroyableByLighting = true;
    [SerializeField] bool convertableToFood = true;
    [SerializeField] bool destroyOnCollide = false;
    [SerializeField] float foodCount;
    [SerializeField] bool autoSetCollider = true;

    private void Start()
    {
        if(autoSetCollider)
            obstacleCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Cloud player = collision.gameObject.GetComponent<Cloud>();
        if(player != null)
        {
            player.ObstacleCollide(gameObject.tag);
            if (destroyOnCollide)
                Destroy(gameObject);
        }
    }

    public void DestroyByLighting()
    {
        if(destroyableByLighting)
            Destroy(gameObject);
    }

    public bool ConvertToFood()
    {
        if(convertableToFood)
        {
            Food foodProp = gameObject.AddComponent<Food>();
            foodProp.FoodCount = foodCount;
            rb.bodyType = RigidbodyType2D.Dynamic;
            obstacleCollider.isTrigger = true;
            Destroy(this);
        }

        return convertableToFood;
    }
}
