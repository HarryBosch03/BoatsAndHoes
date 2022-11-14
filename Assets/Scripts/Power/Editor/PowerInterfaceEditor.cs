using System.Text;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(PowerInterface))]
public sealed class PowerInterfaceEditor : Editor
{
    private void OnSceneGUI()
    {
        var target = base.target as PowerInterface;
        var allInterfaces = FindObjectsOfType<PowerInterface>();
        foreach (var pInterface in allInterfaces)
        {
            if (target.Channel == pInterface.Channel)
            {
                Handles.DrawLine(target.transform.position, pInterface.transform.position, 5.0f);
            }

            StringBuilder b = new StringBuilder();
            b.Append($"{pInterface.name} Listenting on {pInterface.Channel}\n");
            b.Append($"Charge: {pInterface.Fill}");

            Handles.Label(pInterface.transform.position + Vector3.up * 0.5f, b.ToString());
        }
    }
}
