using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PickupInteraction : InteractableBase
{
    public ItemStack stack;

    public override string InteractionDisplayName => "Add to Inventory";

    public override void Interact(GameObject interactor)
    {
        InventoryParent inventory = interactor.GetComponentInChildren<InventoryParent>();
        if (inventory)
        {
            if (inventory.TryFitItem(stack.Clone()))
            {
                Destroy(gameObject);
            }
        }
    }
}
