using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class WeaponModifier : MonoBehaviour
{
    public WeaponTrigger Trigger { get; private set; }
    public WeaponAmmo Ammo { get; private set; }
    public WeaponEffect Effect { get; private set; }

    protected virtual void Awake ()
    {
        Trigger = GetComponent<WeaponTrigger>();
        Ammo = GetComponent<WeaponAmmo>();
        Effect = GetComponent<WeaponEffect>();

        Effect.PreExecuteEvent += OnEffectPreExecute;
        Effect.PostExecuteEvent += OnEffectPostExecute;
    }

    protected virtual void OnEffectPreExecute() { }
    protected virtual void OnEffectPostExecute() { }
}
