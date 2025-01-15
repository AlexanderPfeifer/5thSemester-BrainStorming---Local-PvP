using UnityEngine;

public class DisplayColorAttribute : PropertyAttribute
{
    public Color Color;

    public DisplayColorAttribute(float r, float g, float b, float a = 1f)
    {
        Color = new Color(r, g, b, a);
    }
}
