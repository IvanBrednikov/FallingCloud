using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudShield : BossAttack
{
    [SerializeField] GameObject cloudShield;
    [SerializeField] CloudShieldDurable[] clouds;
    [SerializeField] float shieldLifeTime = 20;
    [SerializeField] float prepareTime = 3f;
    [SerializeField] float shieldUpdateTime;
    bool updateAvailableByTimer = true;

    Coroutine activate;
    Coroutine deactivate;

    public bool UpdateAvailable 
    {
        get
        {
            bool shieldDamaged = false;
            foreach(CloudShieldDurable cloudObj in clouds)
            {
                if(cloudObj.CurrentHealth < cloudObj.MaxHealth || !cloudShield.activeSelf)
                    shieldDamaged = true;
            }
            return updateAvailableByTimer && shieldDamaged;
        }
    }

    private void Awake()
    {
        clouds = cloudShield.GetComponentsInChildren<CloudShieldDurable>();
    }

    public override void Activate()
    {
        base.Activate();
        pointToMove = player.transform.position;
        if (activate != null)
            StopCoroutine(activate);
        activate = StartCoroutine(ActivateShield());
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    IEnumerator ActivateShield()
    {
        yield return new WaitForSeconds(prepareTime);
        cloudShield.SetActive(true);
        for (int i = 0; i < clouds.Length; i++)
            clouds[i].gameObject.SetActive(true);

        if(deactivate != null)
            StopCoroutine(deactivate);
        deactivate = StartCoroutine(DeactivateShield());
        activate = null;

        updateAvailableByTimer = false;
        StartCoroutine(UpdateShieldDelay());

        InvokeSoundEffect();
    }

    IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(shieldLifeTime);
        cloudShield.SetActive(false);
        deactivate = null;
    }

    public void DeactivateShieldInstantly()
    {
        cloudShield.SetActive(false);
        deactivate = null;
    }

    IEnumerator UpdateShieldDelay()
    {
        yield return new WaitForSeconds(shieldUpdateTime);
        updateAvailableByTimer = true;
    }
}
