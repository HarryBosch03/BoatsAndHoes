using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class WeaponAmmo : MonoBehaviour
{
    public abstract event System.Action ReloadEvent;

    public bool Reloading { get; protected set; }

    public abstract bool TryFire();
    public abstract bool CanFire();
}
