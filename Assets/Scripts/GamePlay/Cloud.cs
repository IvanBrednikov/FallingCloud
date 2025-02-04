using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PointsTransfer(int points);
public delegate void CollideEvent(string tagCollidedObject);
public delegate void SimpleEvent();

public class Cloud : MonoBehaviour
{
    CloudSize sizeComponent;
    [SerializeField] PlayerProgress playerProgress;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 startPosition;
    [SerializeField] bool freezOnStart = true;
    [SerializeField] GameObject dieAnimationObject;

    //size
    [SerializeField] SpriteRenderer cloudSprite;
    [SerializeField] float size = 1f;
    [SerializeField] float sizeRatio = 0.005f;
    [SerializeField] float maxSize = 3f;
    [SerializeField] float baseForceRatio = 2f;
    [SerializeField] float maxSpeed = 40f;
    [SerializeField] float forceRatio = 2f;
    [SerializeField] float baseColliderSize = 0.5f;
    [SerializeField] float baseSpriteSize = 1f;

    public event PointsTransfer OnGainFood;
    public event CollideEvent OnObstacleCollide;

    public event SimpleEvent OnRainActivate;
    public event SimpleEvent OnRainDeactivate;
    public event SimpleEvent OnLightningActivate;
    public event SimpleEvent OnLightingDeactivate;
    public event SimpleEvent OnTornadoActivate;
    public event SimpleEvent OnTornadoDeactivate;

    public event SimpleEvent OnRainRecharged;
    public event SimpleEvent OnLightingRecharged;
    public event SimpleEvent OnTornadoRecharged;
    
    public event SimpleEvent OnTeleportation;

    //perks
    [SerializeField] GameObject rainCollider;
    [SerializeField] float rainActiveTime = 20f;
    [SerializeField] float rainRechargeTime = 30;
    Coroutine rainRecharge;

    [SerializeField] GameObject LightingCollider;
    [SerializeField] float lightingActiveTime = 3f;
    [SerializeField] float lightingRechargeTime = 15;
    Coroutine lightingRecharge;

    [SerializeField] GameObject tornadoCollider;
    [SerializeField] GameObject tornadoJointsCenter;
    [SerializeField] float tornadoActiveTime = 7f;
    [SerializeField] float tornadoRechargeTime = 60;
    Coroutine tornadoRecharge;
    [SerializeField] float foodFlightByTornadoTime = 1f;
    [SerializeField] float jointDistance = 2f;
    [SerializeField] float jointDampingRatio = 1f;
    [SerializeField] float jointFreq = 2f;
    [SerializeField] int maxTornadoJoints = 80;

    public PlayerStatistic statistic;

    //debug
    [SerializeField] bool ignorePerkProgress;
    [SerializeField] bool ignoreDamage;
    [SerializeField] bool ignoreSize;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sizeComponent = GetComponent<CloudSize>();
        if (freezOnStart)
            FreezePlayer();
        transform.position = startPosition;
    }

    public void AddImpulse(Vector3 force)
    {
        forceRatio = baseForceRatio * sizeComponent.CurrentMass;
        rb.AddForce(force * forceRatio, ForceMode2D.Impulse);
    }

    public void SetNextSize()
    {
        if (!ignoreSize)
            sizeComponent.SetNextSize();
    }

    public void SetPreviousSize()
    {
        if (!ignoreSize)
            sizeComponent.SetPreviousSize();
    }

    public void SetStartSize()
    {
        sizeComponent.ChangeSize(CloudSize.ESize.Small);
    }

    public CloudSize.ESize GetSize { get { return sizeComponent.GetSize; } }

    public void FoodConsume(float count)
    {
        if (OnGainFood != null)
            OnGainFood((int)count);
    }

    public void ObstacleCollide(string collisionTag)
    {
        if (!ignoreDamage)
        {
            if (OnObstacleCollide != null)
                OnObstacleCollide(collisionTag);
            if (collisionTag == "Boss")
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    //RainPerk
    public bool RainOnReload { get { return rainRecharge != null; } }

    public bool RainAvailableByProgress { get { return playerProgress.RainIsAvailable || ignorePerkProgress; } }

    public bool RainIsActive { get { return rainCollider.activeSelf; } }

    public void RainActivate()
    {
        if (!RainOnReload && RainAvailableByProgress)
        {
            rainCollider.SetActive(true);
            StartCoroutine(RainDeactivate());
            if (OnRainActivate != null)
                OnRainActivate();
        }
    }

    IEnumerator RainDeactivate()
    {
        yield return new WaitForSeconds(rainActiveTime);
        rainCollider.SetActive(false);
        rainRecharge = StartCoroutine(RainRecharge());
        if (OnRainDeactivate != null)
            OnRainDeactivate();
    }

    IEnumerator RainRecharge()
    {
        yield return new WaitForSeconds(rainRechargeTime);
        rainRecharge = null;
        if (OnRainRecharged != null)
            OnRainRecharged();
    }

    //Lightning Perk
    public bool LightningAvailableByProgress { get { return playerProgress.LightninhIsAvailable || ignorePerkProgress; } }

    public bool LightningOnReload { get { return lightingRecharge != null; } }

    public bool LightninhIsActive { get { return LightingCollider.activeSelf; } }

    public void LightningActivate()
    {
        if (LightningAvailableByProgress && !LightningOnReload)
        {
            LightingCollider.SetActive(true);
            StartCoroutine(LightingDeactivate());
            if (OnLightningActivate != null)
                OnLightningActivate();
        }
    }

    IEnumerator LightingDeactivate()
    {
        yield return new WaitForSeconds(lightingActiveTime);
        LightingCollider.SetActive(false);
        lightingRecharge = StartCoroutine(LightningRecharge());
        if (OnLightingDeactivate != null)
            OnLightingDeactivate();
    }

    IEnumerator LightningRecharge()
    {
        yield return new WaitForSeconds(lightingRechargeTime);
        lightingRecharge = null;
        if (OnLightingRecharged != null)
            OnLightingRecharged();
    }

    //tornado perk
    public bool TornadoAvailableByProgress { get { return playerProgress.TornadoIsAvailable || ignorePerkProgress; } }

    public bool TornadoOnReload { get { return tornadoRecharge != null; } }

    public bool TornadoIsActive { get { return tornadoCollider.activeSelf; } }

    public void TornadoActivate()
    {
        if (TornadoAvailableByProgress && !TornadoOnReload)
        {
            tornadoCollider.SetActive(true);
            StartCoroutine(TornadoDeactivate());
            if (OnTornadoActivate != null)
                OnTornadoActivate();
        }
    }

    IEnumerator TornadoDeactivate()
    {
        yield return new WaitForSeconds(tornadoActiveTime);
        tornadoCollider.SetActive(false);
        tornadoRecharge = StartCoroutine(TornadoRecharge());
        if (OnTornadoDeactivate != null)
            OnTornadoDeactivate();
    }

    IEnumerator TornadoRecharge()
    {
        yield return new WaitForSeconds(tornadoRechargeTime);
        tornadoRecharge = null;
        if (OnTornadoRecharged != null)
            OnTornadoRecharged();
    }

    public void CreateTornadoJoint(Rigidbody2D body)
    {
        int jointsCount = tornadoJointsCenter.GetComponents<SpringJoint2D>().Length;
        bool isJoined = false;

        if(jointsCount <= maxTornadoJoints)
        {
            SpringJoint2D joint = tornadoJointsCenter.AddComponent<SpringJoint2D>();
            joint.connectedBody = body;
            joint.distance = jointDistance;
            joint.autoConfigureDistance = false;
            joint.dampingRatio = jointDampingRatio;
            joint.frequency = jointFreq;
            isJoined = true;
        }

        Collider2D collider = body.GetComponent<Collider2D>();
        collider.enabled = false;
        body.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(TornadoFoodColliderEnable(collider));

        Food food;
        if (body.TryGetComponent(out food))
        {
            food.IsJointed = isJoined;
        }
    }

    public void DesttroyTornadoJoint(string name)
    {
        Joint2D[] joints = tornadoJointsCenter.GetComponents<Joint2D>();
        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i].connectedBody != null && joints[i].connectedBody.name == name)
            {
                Destroy(joints[i]);
                break;
            }
        }
    }

    public void DestroyAllJoints()
    {
        Joint2D[] joints = tornadoJointsCenter.GetComponents<Joint2D>();
        if (joints != null)
            for (int i = 0; i < joints.Length; i++)
            {
                Food food;
                if (joints[i].connectedBody != null)
                {
                    if (joints[i].connectedBody.TryGetComponent(out food))
                    {
                        food.StartCoroutine(food.DestroyOnTime());
                    }
                }
                Destroy(joints[i]);
            }
    }

    IEnumerator TornadoFoodColliderEnable(Collider2D collider)
    {
        yield return new WaitForSeconds(foodFlightByTornadoTime);
        if (collider != null)
            collider.enabled = true;
    }

    public void FullPerkReset()
    {
        rainCollider.SetActive(false);
        LightingCollider.SetActive(false);
        tornadoCollider.SetActive(false);
        lightingRecharge = null;
        rainRecharge = null;
        tornadoRecharge = null;
    }

    private void FixedUpdate()
    {
        Rigidbody2D body = tornadoJointsCenter.GetComponent<Rigidbody2D>();
        body.MovePosition(gameObject.transform.position);

        if (rb.velocity.magnitude >= maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    public void SetUpStartProperties()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        FreezePlayer();
        FullPerkReset();
        SetStartSize();
    }

    public void FreezePlayer()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void UnFreezePlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public bool PlayerFreezed { get { return rb.bodyType == RigidbodyType2D.Static; } }

    public float RainRechargeTime { get { return rainRechargeTime; } }

    public float LightningRechargeTime { get { return lightingRechargeTime; } }

    public float TornadoRechargeTime { get { return tornadoRechargeTime; } }

    public Vector2 StartPosition { get { return startPosition; } }

    public float RainActiveTime { get => rainActiveTime; }
    public float LightingActiveTime { get => lightingActiveTime; }
    public float TornadoActiveTime { get => tornadoActiveTime; }

    public float HorizontalSpeed { get => rb.velocity.x; }

    public float BaseImpulseRatio { 
        get => baseForceRatio; 
        set
        {
            if (value < 1)
                value = 1;
            if (value > 5)
                value = 5;
            baseForceRatio = value;
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Portal" && OnTeleportation != null)
            OnTeleportation();
    }

    public void SetArrowVisualizeState(bool state)
    {
        VelocityVisualize visualComponent = GetComponent<VelocityVisualize>();
        Transform arrowVisual = transform.Find("ArrowRotationGizmo");
        if (visualComponent != null)
            visualComponent.enabled = state;
        if (arrowVisual != null)
            arrowVisual.gameObject.SetActive(state);
    }
}
