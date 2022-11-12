using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class WeaponTrigger : MonoBehaviour
{
    [SerializeField] protected WeaponEffect weaponEffect;

    public abstract event System.Action UseEvent;
}
