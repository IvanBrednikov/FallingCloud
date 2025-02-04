using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudShieldDurable : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    public float CurrentHealth { get => currentHealth; }
    public float MaxHealth { get => maxHealth; }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void GainDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            gameObject.SetActive(false);
    }
}
