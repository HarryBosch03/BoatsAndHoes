
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using static UnityEngine.Rendering.DebugUI;

public class ChargeTrigger : WeaponTrigger
{
    [SerializeField] float chargeTime;
    [SerializeField] AnimationCurve speedCurve;

    [Space]
    [SerializeField] Transform root;
    [SerializeField] float shakeAmplitude;
    [SerializeField] float shakeFrequency;

    [Space]
    [SerializeField] Animator animator;

    [Space]
    [SerializeField] UnityEvent<float> chargeEvent;

    bool charging;
    float startChargeTime;

    bool lastShootState;

    public override event Action UseEvent;

    private float GetChargePercent()
    {
        return Mathf.Clamp01((Time.time - startChargeTime) / chargeTime);
    }

    private void Update()
    {
        IController controller = GetComponentInParent<IController>();

        if (!weaponEffect.CanExecute()) return;

        if (controller.Shoot && !lastShootState)
        {
            startChargeTime = Time.time;
            charging = true;
        }
        if (!controller.Shoot && lastShootState)
        {
            float percent = GetChargePercent();
            if (weaponEffect is IWeaponEffectSpeedOverride)
            {
                ((IWeaponEffectSpeedOverride)weaponEffect).OverrideSpeed = speedCurve.Evaluate(percent);
            }

            weaponEffect.Execute();
            chargeEvent.Invoke(0.0f);
            charging = false;
        }

        lastShootState = controller.Shoot;
    }

    private void LateUpdate()
    {
        if (charging)
        {
            float percent = GetChargePercent();
            root.localPosition += new Vector3
            {
                x = Mathf.PerlinNoise(Time.time * shakeFrequency, 0.5f) * 2.0f - 1.0f,
                y = Mathf.PerlinNoise(Time.time * shakeFrequency, 1.5f) * 2.0f - 1.0f,
                z = Mathf.PerlinNoise(Time.time * shakeFrequency, 2.5f) * 2.0f - 1.0f,
            } * shakeAmplitude * percent;

            if (animator) animator.SetFloat("charge", percent);
            chargeEvent.Invoke(percent);
        }
    }
}
