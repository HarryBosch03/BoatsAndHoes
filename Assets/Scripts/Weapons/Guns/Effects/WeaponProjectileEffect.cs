using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
[DisallowMultipleComponent]
public class WeaponProjectileEffect : WeaponEffect, IWeaponEffectSpeedOverride
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject shooter;
    [SerializeField] int projectileCount;
    [SerializeField] float spray;

    [Space]
    [SerializeField] bool overrideDamage;
    [SerializeField] protected float damage;
    [SerializeField] bool overrideSpeed;
    [SerializeField] protected float muzzleSpeed;

    [Space]
    [SerializeField] UnityEvent executeEvent;

    public float? OverrideSpeed 
    {
        get => overrideSpeed ? muzzleSpeed : null;
        set
        {
            overrideSpeed = value.HasValue;
            if (value.HasValue) muzzleSpeed = value.Value;
        }
    }

    public override event System.Action PreExecuteEvent;
    public override event System.Action PostExecuteEvent;

    public override void Execute()
    {
        if (weaponAmmo.TryFire())
        {
            PreExecuteEvent?.Invoke();

            for (int i = 0; i < projectileCount; i++)
            {
                Quaternion rotation = Quaternion.Euler(Random.insideUnitSphere * spray);
                Projectile instance = Instantiate(projectile, muzzle.position, muzzle.rotation * rotation);
                instance.Shooter = shooter;

                if (overrideDamage)
                {
                    instance.Damage = damage;
                }

                if (overrideSpeed)
                {
                    instance.Rigidbody.velocity = instance.transform.forward * muzzleSpeed;
                }

                Rigidbody rigidbody = GetComponentInParent<Rigidbody>();
                instance.Rigidbody.velocity += rigidbody.velocity;
            }

            PostExecuteEvent?.Invoke();
            executeEvent?.Invoke();
        }
    }

    public override bool CanExecute()
    {
        if (!weaponAmmo.CanFire()) return false;

        return true;
    }
}
