using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PistolAnimator : WeaponAnimator
{
    [Space]
    public Transform slide;
    public Vector3 slideDirection;

    [Space]
    public float rackDistance;
    public AnimationCurve fireSlideCurve;

    protected override void LateUpdate()
    {
        base.LateUpdate();

        float slideOffset = rackDistance * fireSlideCurve.Evaluate(Time.time - effect.LastExecuteTime);

        MagazineAmmo ammo = base.ammo as MagazineAmmo;
        if (ammo)
        {
            if (ammo.CurrentMagazine <= 0 && !ammo.Reloading)
            {
                slideOffset = rackDistance;
            }
        }

        slide.localPosition += slideDirection * slideOffset;
    }
}
