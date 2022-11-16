using System;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class TerminalUser : MonoBehaviour
{
    [SerializeField] string userID;

    public string UserID => userID;

    public void OpenTerminal()
    {
        PlayerController.InputBlockers.Add(this);
    }

    public void CloseTerminal()
    {
        PlayerController.InputBlockers.Remove(this);
    }
}
