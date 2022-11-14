using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] Transform cameraRotor;
    [SerializeField] float interactDistance;

    [Space]
    [SerializeField] Transform holdTarget;
    [SerializeField] float holdForce;
    [SerializeField] float holdDamping;
    [SerializeField] float maxForce;
    [SerializeField] float maxDistance;
    [SerializeField] LineRenderer line;

    Rigidbody heldObject;
    RadialMenu radialMenu;
    List<RadialMenuConfig> radialMenuConfig = new List<RadialMenuConfig>();

    bool lastInteractInput;

    public static GameObject Highlighted { get; private set; }
    public static GameObject Selected { get; private set; }

    private void Awake()
    {
        radialMenu = GetComponentInChildren<RadialMenu>();
    }

    private void FixedUpdate()
    {
        Highlighted = null;
        IController controller = GetComponentInParent<IController>();

        if (heldObject)
        {
            MoveHeldObject();
        }
        else
        {
            line.enabled = false;
        }

        if (controller != null)
        {
            if (heldObject)
            {
                if (controller.Interact && !lastInteractInput)
                {
                    Drop();
                }
            }
            else if (Selected)
            {
                if (!controller.Interact && lastInteractInput)
                {
                    Selected = null;
                    radialMenu.Execute();
                }
            }
            else if (TryGetLookingAt(out var lookingAt))
            {
                radialMenuConfig.Clear();
                GetInteractions(lookingAt);

                if (radialMenuConfig.Count > 0) Highlighted = lookingAt.transform.gameObject;

                if (controller.Interact && !lastInteractInput)
                {
                    Select(lookingAt.transform.gameObject);
                }
            }

            lastInteractInput = controller.Interact;
        }
    }

    private void GetInteractions(RaycastHit lookingAt)
    {
        if (lookingAt.rigidbody)
        {
            if (!lookingAt.rigidbody.isKinematic)
            {
                radialMenuConfig.Add(new RadialMenuConfig("Pick Up", () => Pickup(lookingAt.rigidbody)));
            }
        }

        foreach (var interaction in lookingAt.transform.GetComponentsInChildren<InteractableBase>())
        {
            radialMenuConfig.Add(new RadialMenuConfig(interaction.InteractionDisplayName, () => interaction.Interact(gameObject)));
        }
    }

    private void Select(GameObject selection)
    {
        if (radialMenuConfig.Count == 0) return;

        Selected = selection;

        if (radialMenuConfig.Count > 1)
        {
            radialMenu.Show(radialMenuConfig.ToArray());
        }
        else
        {
            radialMenuConfig[0].callback?.Invoke();
            radialMenuConfig.Clear();
        }
    }

    private void MoveHeldObject()
    {
        Vector3 difference = holdTarget.position - heldObject.position;
        Vector3 force = difference * holdForce * Time.deltaTime;
        force -= heldObject.velocity * holdDamping * Time.deltaTime;

        line.enabled = true;
        line.useWorldSpace = true;
        line.SetPosition(0, holdTarget.position);
        line.SetPosition(1, heldObject.position);

        if (force.sqrMagnitude > maxForce * maxForce || difference.sqrMagnitude > maxDistance * maxDistance)
        {
            Drop();
        }
        else
        {
            Rigidbody self = GetComponentInParent<Rigidbody>();
            self.velocity -= force / self.mass;

            heldObject.velocity += force / heldObject.mass;
        }
    }

    private bool TryGetLookingAt(out RaycastHit hit)
    {
        Ray ray = new Ray(cameraRotor.position, cameraRotor.forward);
        return Physics.Raycast(ray, out hit, interactDistance);
    }

    private void Pickup(Rigidbody rigidbody)
    {
        heldObject = rigidbody;
        Selected = null;
    }

    private void Drop()
    {
        heldObject = null;
        Selected = null;
    }
}
