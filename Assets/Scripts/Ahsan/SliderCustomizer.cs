using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SliderCustomizer : MonoBehaviour
{
    public Image fillImage;
    public Image backgroundImage;
    
    public Color fillColor;
    public Color backgroundColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyColors();
    }
#endif
    

    void ApplyColors()
    {
        if (fillImage)
        {
            fillImage.color = fillColor;
        }

        if (backgroundImage)
        {
            backgroundImage.color = backgroundColor;
        }
    }
}
