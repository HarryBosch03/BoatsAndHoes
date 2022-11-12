using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    GameObject gameObject { get; }
    Transform transform { get; }

    Vector2 MoveDirection { get; }
    bool Jump { get; }

    bool Shoot { get; }
    bool Aim { get; }
    bool Reload { get; }

    bool Interact { get; }
}