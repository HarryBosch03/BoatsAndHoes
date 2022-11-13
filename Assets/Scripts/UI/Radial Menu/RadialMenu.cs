using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] RadialMenuOption optionPrefab;
    [SerializeField] RectTransform selectionDot;
    [SerializeField] float clamp;
    [SerializeField] float smoothTime;

    float scale;
    float velocity;
    bool open;

    List<RadialMenuOption> options = new List<RadialMenuOption>();

    Vector2 selection;

    public int SelectionIndex
    {
        get
        {
            float percent = Mathf.Atan2(-selection.x, selection.y) / (Mathf.PI * 2.0f) + 0.5f;
            return (int)(percent * options.Count);
        }
    }

    private void OnEnable()
    {
        ClearMenu();
        Hide();
    }

    private void Update()
    {
        selection += Mouse.current.delta.ReadValue();
        selection = Vector2.ClampMagnitude(selection, clamp);
        selectionDot.anchoredPosition = selection;

        if (options.Count > 0)
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].IsSelected = i == SelectionIndex;
            }
        }

        scale = Mathf.SmoothDamp(scale, open ? 1.0f : 0.0f, ref velocity, smoothTime);
        transform.localScale = Vector3.one * scale;
    }

    public void Show (params RadialMenuConfig[] config)
    {
        ClearMenu();

        open = true;

        selection = Vector2.zero;
        selectionDot.gameObject.SetActive(true);

        int i = 0;
        foreach (var entry in config)
        {
            RadialMenuOption option = Instantiate(optionPrefab, transform);
            option.Apply(entry, i++, config.Length);
            options.Add(option);
        }
    }

    private void ClearMenu()
    {
        foreach (var option in options)
        {
            Destroy(option.gameObject);
        }

        options.Clear();
    }

    public void Hide ()
    {
        open = false;
        selectionDot.gameObject.SetActive(false);
    }

    public void Execute ()
    {
        if (options.Count == 0) return;

        options[SelectionIndex].Execute();

        Hide();
    }
}
