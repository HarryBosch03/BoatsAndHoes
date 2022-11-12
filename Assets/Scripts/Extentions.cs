using UnityEngine;

public static class Extentions
{
    public static bool ChildOf (this Transform transform, Transform query)
    {
        Transform head = transform;
        while (head)
        {
            if (head == query) return true;
            head = head.parent;
        }

        return false;
    }
}
