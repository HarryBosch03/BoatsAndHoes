using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class TerminalInteractable : InteractableBase
{
    [SerializeField] GameObject terminalUI;
    [SerializeField] TMP_Text terminalText;
    [SerializeField] TMP_InputField inputField;

    TerminalUser user;

    public override string InteractionDisplayName => "Open Terminal.";

    public override bool InteractionValid => !user;

    public override void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out user))
        {
            inputField.text += $"Terminal Session Started.\nUsername: {user.UserID}\nPassword: ********\n\nAuth Successfull!\nWelcome User {user.UserID}.";
            user.OpenTerminal();
        }
    }

    public void CloseTerminal ()
    {
        inputField.text += "Terminal Session Ended.\n\n";
        user = null;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseTerminal();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ParseCommand();
        }
    }

    private void ParseCommand()
    {
        var inputText = inputField.text;
        inputField.text = string.Empty;
        terminalText.text += "\n\n";

        terminalText.text += $"\"{inputText}\" is not a valid command.";
    }
}
