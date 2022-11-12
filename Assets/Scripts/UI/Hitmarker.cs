using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Hitmarker : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string hitAnimName;

    private void OnEnable()
    {
        Health.RecieveDamageEvent += OnRecievedDamage;
    }

    private void OnDisable()
    {
        Health.RecieveDamageEvent -= OnRecievedDamage;
    }

    private void OnRecievedDamage(object sender, DamageArgs e)
    {
        if (transform.ChildOf(e.damager.transform))
        {
            animator.Play(hitAnimName, 0, 0.0f);
        }
    }
}
