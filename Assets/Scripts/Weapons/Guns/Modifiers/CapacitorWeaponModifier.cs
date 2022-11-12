using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class CapacitorWeaponModifier : WeaponModifier
{
    [SerializeField] float maxCapacitance;
    [SerializeField] float capacitanceRegen;
    [SerializeField] float shotCost;
    [Range(0.0f, 1.0f)][SerializeField] float currentCapacitance;

    [Space]
    [SerializeField] AnimationCurve speedCurve;

    private void Update()
    {
        currentCapacitance += capacitanceRegen / maxCapacitance * Time.deltaTime;
        currentCapacitance = Mathf.Clamp01(currentCapacitance);
    }

    protected override void OnEffectPreExecute()
    {
        float efficiency = Mathf.Min(1.0f, currentCapacitance * maxCapacitance / shotCost);
        ((IWeaponEffectSpeedOverride)Effect).OverrideSpeed = speedCurve.Evaluate(efficiency);

        currentCapacitance -= shotCost / maxCapacitance;
    }
}
