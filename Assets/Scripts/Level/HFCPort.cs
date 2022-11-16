using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(PowerInterface))]
public sealed class HFCPort : InteractableBase
{
    [SerializeField] float drainRate;
    [SerializeField] GameObject cellContainer;

    [Space]
    [SerializeField] Image statusIndicator;
    [SerializeField] Color noCell;
    [SerializeField] Color drained;
    [SerializeField] Color working;

    [Space]
    [SerializeField] HFC cell;

    PowerInterface pInterface;

    float deadTime;

    public override string InteractionDisplayName => "Extract Cell";
    public override bool InteractionValid => cell;

    private void Awake()
    {
        pInterface = GetComponent<PowerInterface>();
        cellContainer.SetActive(cell);
    }

    private void Update()
    {
        pInterface.Supply = 0.0f;
        statusIndicator.color = noCell;
        statusIndicator.fillAmount = 1.0f;
        if (cell)
        {
            statusIndicator.color = drained;
            if (cell.Fill > 0.0f)
            {
                pInterface.Supply = drainRate;
                statusIndicator.fillAmount = cell.Fill / cell.Capacity;
                statusIndicator.color = working;
            }
        }

        deadTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (cell)
        {
            cell.Fill -= pInterface.FrameSupply;
        }
    }

    public override void Interact(GameObject interactor)
    {
        if (!cell) return;
        
        cellContainer.SetActive(false);
        cell.gameObject.SetActive(true);
        cell = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cell) return;
        if (deadTime > 0.0f) return;

        if (other.TryGetComponent(out HFC hfc))
        {
            cell = hfc;
            cellContainer.SetActive(true);
            hfc.gameObject.SetActive(false);
            deadTime = 1.0f;
        }
    }
}
