using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Spline : MonoBehaviour
{
    [SerializeField] Color color = Color.yellow;
    [SerializeField] bool persistGizmos;
    [SerializeField] float gizmosResolution;

    private void OnDrawGizmos()
    {
        if (persistGizmos)
        {
            DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    private void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).name = $"Spline Point {i + 1}";
        }
    }

    private void DrawGizmos()
    {
        Gizmos.color = color;

        if (gizmosResolution < 0.01f) gizmosResolution = 0.01f;

        

        for (float i = 0; i <= 1.0000001f; i += gizmosResolution)
        {
            Gizmos.DrawLine(Get(i), Get(i + gizmosResolution));
        }
    }

    public Vector3 Get (float percent)
    {
        if (percent <= 0.0f)
        {
            return transform.GetChild(0).position;
        }

        if (percent >= 1.0f)
        {
            return transform.GetChild(transform.childCount - 1).position;
        }

        int semgents = transform.childCount - 1;
        percent *= semgents;

        int offset = Mathf.FloorToInt(percent);
        percent %= 1.0f;

        if (offset > transform.childCount - 2) offset = transform.childCount - 2;

        return Get(transform.GetChild(offset), transform.GetChild(offset + 1), percent);
    }

    private float EstimateDistance (Transform handleA, Transform handleB, int itterations = 100)
    {
        float dist = 0.0f;

        for (int i = 0; i < itterations; i++)
        {
            dist += (Get(handleA, handleB, i / (float)itterations) - Get(handleA, handleB, (i - 1.0f) / itterations)).magnitude;
        }

        return dist;
    }

    public float EstimateTotalCurveLength ()
    {
        float total = 0.0f;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            total += EstimateDistance(transform.GetChild(i), transform.GetChild(i + 1));
        }
        return total;
    }

    public List<Vector3> GetEvenlyDistributedPointsOverLine (float increment, float accuracy)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(transform.GetChild(0).position);

        float workingDistance = 0.0f;
        for (float p = 0; p < 1.0f; p += accuracy)
        {
            Vector3 a = Get(p);
            Vector3 b = Get(p + accuracy);

            float dist = (b - a).magnitude;
            workingDistance += dist;

            if (workingDistance > increment)
            {
                workingDistance -= increment;
                points.Add(Vector3.Lerp(a, b, workingDistance / increment));
            }
        }

        points.Add(transform.GetChild(transform.childCount - 1).position);
        return points;
    }

    private Vector3 Get (Transform handleA, Transform handleB, float percent)
    {
        Vector3 head = handleA.position;
        Vector3 tail = handleB.position;

        Vector3 headSub = head + handleA.transform.forward * handleA.transform.localScale.z;
        Vector3 tailSub = tail + -handleB.transform.forward * handleB.transform.localScale.z;

        Vector3 a = Vector3.Lerp(head, headSub, percent);
        Vector3 b = Vector3.Lerp(headSub, tailSub, percent);
        Vector3 c = Vector3.Lerp(tailSub, tail, percent);

        Vector3 i = Vector3.Lerp(a, b, percent);
        Vector3 j = Vector3.Lerp(b, c, percent);

        return Vector3.Lerp(i, j, percent);
    }
}
