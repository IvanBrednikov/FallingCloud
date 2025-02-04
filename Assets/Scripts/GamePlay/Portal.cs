using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] GameProgress progress;
    [SerializeField] Vector2 positionToTeleport;

    private void Start()
    {
        progress = FindObjectOfType<GameProgress>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.position = positionToTeleport;
            collision.rigidbody.velocity = Vector2.zero;
            progress.BossFightStart();

            collision.gameObject.GetComponent<Cloud>().statistic.PortalRised = true;

            Destroy(gameObject);
        }
    }
}
