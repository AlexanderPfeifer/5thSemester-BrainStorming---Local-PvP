using UnityEngine;

public class DisplayColorAttribute : PropertyAttribute
{
    public Color color;

    public DisplayColorAttribute(float r, float g, float b, float a = 1f)
    {
        color = new Color(r, g, b, a);
    }
}
