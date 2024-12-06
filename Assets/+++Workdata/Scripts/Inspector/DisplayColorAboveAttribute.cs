using UnityEngine;

public class DisplayColorAboveAttribute : PropertyAttribute
{
    public Color color;

    public DisplayColorAboveAttribute(float r, float g, float b, float a = 1f)
    {
        color = new Color(r, g, b, a);
    }
}
