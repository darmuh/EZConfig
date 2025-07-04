using UnityEngine;

namespace EZConfig.UI;
#nullable disable
[RequireComponent(typeof(RectTransform))]
public class PeakElement : MonoBehaviour
{
    public RectTransform RectTransform { get; internal set; }
}

public static class PeakElementExtension { 

    public static T ParentTo<T>(this T instance, Transform transform) where T : PeakElement
    {
        instance.transform.SetParent(transform, true);

        return instance;
    }

    public static T SetPosition<T>(this T instance, Vector2 position) where T : PeakElement
    {
        instance.RectTransform.anchoredPosition = position;

        return instance;
    }
}
