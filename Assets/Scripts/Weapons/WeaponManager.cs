using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class WeaponManager : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;

    int currentWeaponIndex;

    public GameObject CurrentWeapon
    {
        get
        {
            if (currentWeaponIndex < 0) return null;
            if (currentWeaponIndex >= weapons.Length) return null;

            return weapons[currentWeaponIndex];
        }
    }

    private void OnEnable()
    {
        SetWeapon(currentWeaponIndex);
    }

    private void SetWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }
        currentWeaponIndex = index;
    }
}
