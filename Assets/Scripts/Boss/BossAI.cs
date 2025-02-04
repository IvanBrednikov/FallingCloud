using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public enum MovementType {Stand, MoveToPoint, MoveToPlayer, MoveToPointAbovePlayer};
    public enum Attacks {Idle, Rain, CloudGun, CrossCloud, SimpleLightning, AdvancedCloudSpawn, AdvancedLightning, CloudShield, SpawnTornadoes, IceGun};
    public enum BossStage {Simple, Normal, Hard, Debug};

    Rigidbody2D rb;

    [SerializeField] GameProgress gameProgress;
    [SerializeField] Cloud player;
    [SerializeField] float speed;
    [SerializeField] AnimationCurve speedIncreaseByDistance;
    [SerializeField] MovementType movementType;
    [SerializeField] Vector2 pointToMove;
    [SerializeField] float distanceAbovePlayer = 5f;

    [SerializeField] Attacks currentAttack;
    [SerializeField] BossAttack currentAttackInfo;
    float attackChangeTimer;
    BossAttack[] attacks;

    [SerializeField] BossStage currentStage;
    [SerializeField] BossStage maxStage;
    [SerializeField] Attacks[] stage1Attacks;
    [SerializeField] Attacks[] stage2Attacks;
    [SerializeField] Attacks[] stage3Attacks;
    [SerializeField] Attacks[] debugAttacks;

    [SerializeField] float distance;

    [SerializeField] float melleAttackRate = 2f;
    bool meleeAttackReady = true;
    Coroutine rechargeMeleeAttackCoroutine;

    [SerializeField] int currentHealth;
    [SerializeField] int stage1Health;
    [SerializeField] int stage2Health;
    [SerializeField] int stage3Health;

    public event SimpleEvent OnBossConfigure;
    public event SimpleEvent OnBossStageUp;
    public event SimpleEvent OnDamageTaken;
    public event SimpleEvent OnDie;

    private void Awake()
    {
        attacks = GetComponents<BossAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //attack
        attackChangeTimer -= Time.fixedDeltaTime;
        if (attackChangeTimer <= 0)
        {
            StartNewAttack();
        }

        //move
        if (currentAttackInfo != null)
            movementType = currentAttackInfo.MovementType;

        Vector2 movementDirection = Vector2.zero;
        if (movementType == MovementType.MoveToPlayer)
        {
            movementDirection = player.transform.position - transform.position;
            distance = Vector2.Distance(player.transform.position, transform.position);
        }

        if (movementType == MovementType.MoveToPoint)
        {
            if(currentAttackInfo != null)
                pointToMove = currentAttackInfo.PointToMove;
            movementDirection = pointToMove - (Vector2)transform.position;
            distance = Vector2.Distance(pointToMove, transform.position);
        }

        if (movementType == MovementType.MoveToPointAbovePlayer)
        {
            Vector3 pointAbovePlayer = player.transform.position + new Vector3(0, distanceAbovePlayer, 0);
            movementDirection = pointAbovePlayer - transform.position;
            distance = Vector2.Distance(pointAbovePlayer, transform.position);
        }
        
        rb.velocity = movementDirection.normalized * speed * speedIncreaseByDistance.Evaluate(distance);
    }

    public void BossConfigure()
    {
        currentStage = BossStage.Simple;
        maxStage = (BossStage)(int)gameProgress.GetDifficulty;
        SetHealth();
        
        CloudShield shieldAttack = (CloudShield)GetAttack(Attacks.CloudShield);
        shieldAttack.DeactivateShieldInstantly();

        OnBossConfigure?.Invoke();
    }

    void StartNewAttack()
    {
        //deactivate
        if (currentAttackInfo != null)
            currentAttackInfo.Deactivate();

        //selection
        int maxIndex;
        if (currentStage == BossStage.Simple)
            maxIndex = stage1Attacks.Length;
        else
        if (currentStage == BossStage.Normal)
            maxIndex = stage2Attacks.Length;
        else
        if (currentStage == BossStage.Hard)
            maxIndex = stage3Attacks.Length;
        else
            maxIndex = debugAttacks.Length;

        Attacks oldAttack = currentAttack;

        while(oldAttack == currentAttack && maxIndex > 1)
        {
            int attackIndex = Random.Range(0, maxIndex);

            if (currentStage == BossStage.Simple)
                currentAttack = stage1Attacks[attackIndex];
            else
            if (currentStage == BossStage.Normal)
                currentAttack = stage2Attacks[attackIndex];
            else
            if (currentStage == BossStage.Hard)
            {
                //check shield update available
                CloudShield shieldAttack = (CloudShield)GetAttack(Attacks.CloudShield);
                if (shieldAttack.UpdateAvailable)
                    currentAttack = Attacks.CloudShield;
                else
                    //overwise random attack
                    currentAttack = stage3Attacks[attackIndex];
            }
            else
                currentAttack = debugAttacks[attackIndex];
        }

        //activation
        currentAttackInfo = GetAttack(currentAttack);
        currentAttackInfo.PreActivate();
        movementType = currentAttackInfo.MovementType;
        speed = currentAttackInfo.MovementSpeed;
        attackChangeTimer = currentAttackInfo.AttackTime;
        currentAttackInfo.Activate();
    }

    public BossAttack GetAttack(Attacks attack)
    {
        BossAttack result = null;

        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].Attack == attack)
            {
                result = attacks[i];
                break;
            }
        }

        return result;
    }

    public Cloud Player
    { get { return player; } }

    public IEnumerator MelleCoroutine()
    {
        meleeAttackReady = false;
        yield return new WaitForSeconds(1 / melleAttackRate);
        meleeAttackReady = true;
        rechargeMeleeAttackCoroutine = null;
    }

    public void RechargeMelleAttack()
    {
        if (rechargeMeleeAttackCoroutine == null)
            rechargeMeleeAttackCoroutine = StartCoroutine(MelleCoroutine());
    }

    public bool MeleeAttackReady { get { return meleeAttackReady; } }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && meleeAttackReady)
        {
            Cloud player = collision.transform.GetComponent<Cloud>();
            player.ObstacleCollide("Boss");
            RechargeMelleAttack();
        }
    }

    public void SetHealth()
    {
        switch(currentStage)
        {
            case BossStage.Simple:
                currentHealth = stage1Health;
                break;
            case BossStage.Normal:
                currentHealth = stage2Health;
                break;
            case BossStage.Hard:
                currentHealth = stage3Health;
                break;
        }
    }

    void SetNextStage()
    {
        if (currentStage == maxStage)
        {
            Death();
        }
        else
        {
            if (currentStage == BossStage.Simple)
                currentStage = BossStage.Normal;
            else
            if (currentStage == BossStage.Normal)
                currentStage = BossStage.Hard;

            SetHealth();
            StartNewAttack();

            if (OnBossStageUp != null)
                OnBossStageUp();
        }
    }

    public void DamageGain(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            SetNextStage();
        }
        if (OnDamageTaken != null)
            OnDamageTaken();
    }

    void Death()
    {
        currentHealth = 0;
        gameProgress.LevelComplete();
        DestroyProjectiles();
        if (OnDie != null)
            OnDie();
        gameObject.SetActive(false);
    }

    public void DestroyProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        for (int i = 0; i < projectiles.Length; i++)
            Destroy(projectiles[i]);
    }

    public float HealthPercent 
    { 
        get 
        {
            int maxHealth = stage1Health;
            if (currentStage == BossStage.Normal)
                maxHealth = stage2Health;
            if (currentStage == BossStage.Hard)
                maxHealth = stage3Health;

            return currentHealth / (float)maxHealth;
        } 
    }

    public BossStage GetCurrentStage { get { return currentStage; } }

    public BossAttack GetCurrentAttack { get { return currentAttackInfo; } }
}
