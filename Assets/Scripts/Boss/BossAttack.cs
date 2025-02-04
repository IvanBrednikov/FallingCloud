using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossAI))]
public class BossAttack : MonoBehaviour
{
    [SerializeField] protected BossAI bossAI;
    [SerializeField] protected Transform player;
    [SerializeField] protected float attakActiveTime;
    [SerializeField] protected BossAI.MovementType movementType;
    [SerializeField] protected float movementSpeed = 1;
    [SerializeField] protected Vector2 pointToMove;
    [SerializeField] protected BossAI.Attacks attack;
    [SerializeField] protected bool attackIsActive;

    public event SimpleEvent OnSoundEffect;
    public event SimpleEvent OnAttackEnd;

    protected void Start()
    {
        bossAI = GetComponent<BossAI>();
        player = bossAI.Player.transform;
    }

    virtual public void PreActivate()
    {

    }

    virtual public void Activate()
    {
        attackIsActive = true;
    }

    virtual public void Deactivate()
    {
        attackIsActive = false;
        if (OnAttackEnd != null)
            OnAttackEnd();
    }

    void Update()
    {

    }

    protected void InvokeSoundEffect()
    {
        OnSoundEffect?.Invoke();
    }

    public BossAI.Attacks Attack { get { return attack; } }

    public BossAI.MovementType MovementType { get { return movementType; } }

    public float AttackTime { get { return attakActiveTime; } }

    public float MovementSpeed { get { return movementSpeed; } }

    public Vector2 PointToMove { get { return pointToMove; } }
}