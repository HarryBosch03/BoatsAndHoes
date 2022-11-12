using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuOption : MonoBehaviour
{
    [SerializeField] float selectionOffset;
    [SerializeField] float smoothTime;

    float offset;
    float velocity;

    System.Action callback;

    public bool IsSelected { get; set; }

    private void Update()
    {
        offset = Mathf.SmoothDamp(offset, IsSelected ? selectionOffset : 0.0f, ref velocity, smoothTime);
        RectTransform transform = this.transform as RectTransform;
        transform.anchoredPosition = transform.right * offset;
    }

    public void Apply(RadialMenuConfig config, int index, int count)
    {
        Image image = GetComponentInChildren<Image>();
        TMPro.TMP_Text text = GetComponentInChildren<TMPro.TMP_Text>();

        image.color = config.color;
        text.text = config.text;
        callback = config.callback;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)index / count * 360.0f);

        image.fillAmount = 1.0f / count;
        image.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, -image.fillAmount * 90.0f);

        text.transform.rotation = transform.parent.rotation;
    }

    internal void Execute()
    {
        callback?.Invoke();
    }
}
