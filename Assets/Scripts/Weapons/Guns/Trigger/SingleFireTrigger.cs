using System;
using UnityEngine;
[SelectionBase]
[DisallowMultipleComponent]
public sealed class SingleFireTrigger : WeaponTrigger
{
    [Space]
    [SerializeField] float firerate;

    float lastFireTime;

    public override event Action UseEvent;

    private void Update()
    {
        IController controller = GetComponentInParent<IController>();

        if (controller.Shoot && Time.time > lastFireTime + 60.0f / firerate)
        {
            UseEvent?.Invoke();
            weaponEffect.Execute();
            lastFireTime = Time.time;
        }
    }
}
