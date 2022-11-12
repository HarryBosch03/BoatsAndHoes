using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class MagazineAmmo : WeaponAmmo
{
    [SerializeField] int magazineSize;
    [SerializeField] int currentMagazine;
    [SerializeField] float reloadTime;

    public override event Action ReloadEvent;

    public int CurrentMagazine => currentMagazine;

    private void Update()
    {
        IController controller = GetComponentInParent < IController>();
        if (controller.Reload)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        if (Reloading) yield break;
        if (currentMagazine >= magazineSize) yield break;

        Reloading = true;
        currentMagazine = 0;

        ReloadEvent?.Invoke();

        yield return new WaitForSeconds(reloadTime);

        Reloading = false;
        currentMagazine = magazineSize;
    }

    public override bool TryFire()
    {
        bool state = CanFire();
        if (state) currentMagazine--;
        return state;
    }

    public override bool CanFire()
    {
        if (Reloading) return false;
        if (currentMagazine <= 0) return false;

        return true;
    }
}
