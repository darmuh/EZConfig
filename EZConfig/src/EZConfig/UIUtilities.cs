using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EZConfig;

public static class UIUtilities
{
    public static Color GetContrastingColor(Color input, float shiftAmount = 0.2f)
    {
        Color.RGBToHSV(input, out float h, out float s, out float v);

        if (v > 0.5f)
            v = Mathf.Clamp01(v - shiftAmount);
        else
            v = Mathf.Clamp01(v + shiftAmount);

        return Color.HSVToRGB(h, s, v);
    }

    public static void ExpandToParent(RectTransform target)
    {
        if (target == null || target.parent == null) return;

        target.anchorMin = Vector2.zero;
        target.anchorMax = Vector2.one;
        target.offsetMin = Vector2.zero;
        target.offsetMax = Vector2.zero;
    }

}
