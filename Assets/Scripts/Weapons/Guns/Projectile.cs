using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float muzzleSpeed;

    [Space]
    [SerializeField] float collisionSize;
    [SerializeField] LayerMask collisionMask;

    [Space]
    [SerializeField] GameObject hitPrefab;

    [Space]
    [SerializeField] float minDamageSpeed;
    [SerializeField] float embedSpeed;

    [Space]
    [SerializeField] Collider slowCollider;

    public Rigidbody Rigidbody { get; private set; }

    public float Damage { get => damage; set => damage = value; }
    public GameObject Shooter { get; set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.velocity = transform.forward * muzzleSpeed;
    }

    private void FixedUpdate()
    {
        float speed = Rigidbody.velocity.magnitude;
        Ray ray = new Ray(Rigidbody.position, Rigidbody.velocity);
        if (Physics.SphereCast(ray, collisionSize, out RaycastHit hit, speed * Time.deltaTime + 0.01f, collisionMask))
        {
            if (speed > minDamageSpeed)
            {
                Health health = hit.transform.GetComponentInParent<Health>();
                if (health)
                {
                    health.Damage(new DamageArgs(Shooter, damage * speed));
                }
            }

            if (speed > embedSpeed)
            {
                Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(gameObject);
            }
        }

        if (speed < embedSpeed)
        {
            slowCollider.enabled = true;
        }
        else
        {
            slowCollider.enabled = false;
            transform.forward = Rigidbody.velocity;
        }
    }
}