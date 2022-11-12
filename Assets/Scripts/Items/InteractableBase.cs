using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public virtual string InteractionDisplayName => "Test";

    public virtual void Interact (GameObject interactor)
    {
        Debug.Log("Boop!", gameObject);
    }
}
