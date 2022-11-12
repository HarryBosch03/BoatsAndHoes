using System;
using System.Collections.Generic;
using UnityEngine;

public class TransposeRig : MonoBehaviour
{
    [SerializeField] TransposeRigMode mode;
    [SerializeField] Transform[] bones;
    [SerializeField] string key;

    TransposeRigMode _mode;

    public static Dictionary<string, HashSet<TransposeRig>> affectedRigs { get; } = new Dictionary<string, HashSet<TransposeRig>>();

    private void Awake()
    {
        _mode = mode;
    }

    private void OnEnable()
    {
        if (_mode == TransposeRigMode.Affected)
        {
            if (!affectedRigs.ContainsKey(key))
            {
                affectedRigs.Add(key, new HashSet<TransposeRig>());
            }
            affectedRigs[key].Add(this);
        }
    }

    private void OnDisable()
    {
        if (affectedRigs.ContainsKey(key))
        {
            affectedRigs[key].Remove(this);
            if (affectedRigs[key].Count == 0)
            {
                affectedRigs.Remove(key);
            }
        }
    }

    private void LateUpdate()
    {
        if (_mode == TransposeRigMode.Effector)
        {
            if (!affectedRigs.ContainsKey(key)) return;

            foreach (TransposeRig rig in affectedRigs[key])
            {
                Transpose(this, rig);
            }
        }
    }

    private static void Transpose(TransposeRig from, TransposeRig to)
    {
        if (from.bones.Length != to.bones.Length)
        {
            Debug.LogError(new Exception($"Rigs are not compatable \"{from}\" => \"{to}\""), from);
        }

        for (int i = 0; i < from.bones.Length; i++)
        {
            to.bones[i].position = from.bones[i].position;
            to.bones[i].rotation = from.bones[i].rotation;
            to.bones[i].localScale = from.bones[i].localScale;
        }
    }

    private void Reset()
    {
        key = name;
        AutoFillBones();
    }

    [ContextMenu("Autofill Bones")]
    private void AutoFillBones()
    {
        List<Transform> bones = new List<Transform>();
        AddChildrenToList(transform, bones);
        this.bones = bones.ToArray();
    }

    private void AddChildrenToList(Transform transform, List<Transform> bones)
    {
        bones.Add(transform);

        foreach (Transform child in transform)
        {
            AddChildrenToList(child, bones);
        }
    }
}

public enum TransposeRigMode
{
    Effector,
    Affected,
}
