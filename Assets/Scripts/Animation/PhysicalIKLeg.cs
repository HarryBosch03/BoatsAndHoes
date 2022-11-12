using UnityEngine;

public class PhysicalIKLeg : MonoBehaviour
{
    [SerializeField] float groundOffset;

    [Space]
    [SerializeField] Vector3 footOffset;
    [SerializeField] Vector3 footAxis;

    [Space]
    [SerializeField] Vector3 localAxis;

    private void LateUpdate()
    {
        Transform a = transform;
        Transform b = a.GetChild(0);
        Transform c = b.GetChild(0);

        float aAngleOffset = Mathf.Acos(Vector3.Dot((c.position - a.position).normalized, (b.position - a.position).normalized));
        float bAngleOffset = Mathf.Acos(Vector3.Dot((c.position - a.position).normalized, (c.position - b.position).normalized));

        Vector3 aAxis = a.rotation * localAxis;
        Vector3 bAxis = b.rotation * localAxis;

        float l1 = (b.position - a.position).magnitude;
        float l2 = (c.position - b.position).magnitude;
        float d = float.MaxValue;

        if (Physics.Linecast(a.position, c.position, out var hit))
        {
            d = hit.distance - groundOffset;
            c.transform.rotation = Quaternion.LookRotation(hit.normal, c.rotation * footAxis) * Quaternion.Euler(footOffset);
        }

        if (d > l1 + l2) return;

        float theta1 = Mathf.Acos((l1 * l1 + d * d - l2 * l2) / (2.0f * l1 * d));
        float theta2 = Mathf.Acos((l1 * l1 + l2 * l2 - d * d) / (2.0f * l1 * l2));
        float theta3 = Mathf.PI - theta2;

        Quaternion aRot = a.rotation;
        Quaternion bRot = b.rotation;
        Quaternion cRot = c.rotation;

        a.rotation = Quaternion.AngleAxis((theta1 - aAngleOffset) * Mathf.Rad2Deg, aAxis) * aRot;
        b.rotation = Quaternion.AngleAxis((theta1 - theta3 + bAngleOffset) * Mathf.Rad2Deg, bAxis) * bRot;

        c.rotation = cRot;
    }
}
