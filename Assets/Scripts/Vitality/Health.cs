using System;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [Range(0.0f, 1.0f)][SerializeField] float currentHealth;
    
    [Space]
    [Range(0.0f, 1.0f)][SerializeField] float currentDoom;
    [Range(0.0f, 1.0f)][SerializeField] float doomPercent;

    [Space]
    [Range(0.0f, 1.0f)][SerializeField] float damageCapPercent;

    [Space]
    [SerializeField] float regenDelay;
    [SerializeField] float regenRate;

    [Space]
    [SerializeField] GameObject deathPrefab;

    public float SoftHealth => currentHealth * maxHealth;
    public float CurrentDoom => currentDoom * maxHealth;
    public float MaxHealth => maxHealth;
    public float LastDamageTime { get; private set; }

    public static event System.EventHandler<DamageArgs> RecieveDamageEvent;

    private void Update()
    {
        if (Time.time > LastDamageTime + regenDelay)
        {
            currentHealth += Mathf.Min(regenRate / maxHealth * Time.deltaTime, 1.0f - currentHealth);
        }
    }

    public void Damage(DamageArgs args)
    {
        args.damage = Mathf.Min(args.damage, maxHealth * damageCapPercent);

        currentHealth -= args.damage / maxHealth;
        currentDoom += args.damage / maxHealth * doomPercent;

        if (currentHealth < 0.0f) currentHealth = 0.0f;
        if (currentDoom > 1.0f) currentDoom = 1.0f;

        LastDamageTime = Time.time;

        RecieveDamageEvent?.Invoke(this, args);

        if (currentHealth <= currentDoom)
        {
            Die(args);
        }
    }

    private void Die(DamageArgs args)
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}

public struct DamageArgs
{
    public GameObject damager;
    public float damage;

    public DamageArgs(GameObject damager, float damage)
    {
        this.damager = damager;
        this.damage = damage;
    }
}

