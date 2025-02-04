using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StateEvent(bool newState);

public class DeathnessTornado : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Cloud player;
    [SerializeField] bool isAgreesiveState;
    [SerializeField] bool autoControlState;
    [SerializeField] float forceValue = 10;
    [SerializeField] float maxDistance = 30;
    [SerializeField] float minDistance = 10;
    [SerializeField] float maxTimeOfStayment = 5f;
    [SerializeField] float speedInAgressiveState = 5f;
    [SerializeField] Vector2 startPosition;
    [SerializeField] float z;

    float timer;

    public event StateEvent OnTornadoStateChange;
    public event SimpleEvent OnTornadoRestart;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Cloud player;
        if (collision.gameObject.TryGetComponent(out player))
        {
            player.ObstacleCollide("deathnessTornado");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Cloud player;
        if (collision.gameObject.TryGetComponent(out player))
        {
            player.ObstacleCollide("floor");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = maxTimeOfStayment;
    }

    private void FixedUpdate()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance > maxDistance)
        {
            if (autoControlState)
            {
                if(isAgreesiveState)
                    OnTornadoStateChange?.Invoke(false);
                isAgreesiveState = false;
                
            }
                
            timer = maxTimeOfStayment;

            Vector3 translate = new Vector3(player.transform.position.x - maxDistance, transform.position.y, z);
            transform.position = translate;
        }
        else
            timer -= Time.fixedDeltaTime;

        if ((distance < minDistance || timer <= 0) && autoControlState)
        {
            if(!isAgreesiveState)
                OnTornadoStateChange?.Invoke(true);
            isAgreesiveState = true;
        }
            

        if (isAgreesiveState)
            rb.velocity = new Vector2(speedInAgressiveState, 0);
        else
            rb.velocity = Vector2.zero;
    }

    public bool AutoControl
    {
        get { return autoControlState; }
        set 
        { 
            autoControlState = value; 
            timer = maxTimeOfStayment;
        }
    }

    public bool IsAgreesiveState { get => isAgreesiveState; }

    public void RestartGame()
    {
        transform.position = startPosition;
        timer = maxTimeOfStayment;
        autoControlState = false;
        isAgreesiveState = false;
        OnTornadoRestart?.Invoke();
    }
}
