using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Healthbar : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image healthFill;
    [SerializeField] Image doomFill;

    private void Update()
    {
        healthFill.fillAmount = health.SoftHealth / health.MaxHealth;
        doomFill.fillAmount = health.CurrentDoom / health.MaxHealth;
    }
}
