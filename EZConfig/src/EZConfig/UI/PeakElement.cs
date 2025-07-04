using UnityEngine;

namespace EZConfig.UI;
#nullable disable
[RequireComponent(typeof(RectTransform))]
public class PeakElement : MonoBehaviour
{
    public RectTransform RectTransform { get; internal set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
}

public static class PeakElementExtension { 

    public static T ParentTo<T>(this T instance, Transform transform) where T : PeakElement
    {
        instance.transform.SetParent(transform, false);

        return instance;
    }

    public static GameObject ParentTo(this GameObject instance, Transform transform) 
    {
        instance.transform.SetParent(transform, false);

        return instance;
    }

    public static T SetPosition<T>(this T instance, Vector2 position) where T : PeakElement
    {
        instance.RectTransform.anchoredPosition = position;

        return instance;
    }

    public static T SetSize<T>(this T instance, Vector2 size) where T : PeakElement
    {
        instance.RectTransform.sizeDelta = size;

        return instance;
    }

    public static T SetAnchorMin<T>(this T instance, Vector2 anchorMin) where T : PeakElement
    {
        instance.RectTransform.anchorMin = anchorMin;

        return instance;
    }

    public static T SetAnchorMax<T>(this T instance, Vector2 anchorMax) where T : PeakElement
    {
        instance.RectTransform.anchorMax = anchorMax;

        return instance;
    }

    public static T SetPivot<T>(this T instance, Vector2 pivot) where T : PeakElement
    {
        instance.RectTransform.pivot = pivot;

        return instance;
    }
}
