using UnityEngine;

public struct RadialMenuConfig
{
    public Color color;
    public string text;
    public System.Action callback;

    public RadialMenuConfig(string text, System.Action callback)
    {
        color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        this.text = text;
        this.callback = callback;
    }

    public RadialMenuConfig(Color color, string text, System.Action callback) : this(text, callback)
    {
        this.color = color;
    }
}
