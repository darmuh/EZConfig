using System.Linq;
using TMPro;
using UnityEngine;
namespace EZConfig.UI;

#nullable disable
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent (typeof(TextMeshProUGUI))]
public class PeakText : PeakElement
{
    private static TMP_FontAsset _darumaFontAsset;
    private static TMP_FontAsset DarumaDropOne 
    { 
        get 
        {
            if (_darumaFontAsset == null)
            {
                var assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                _darumaFontAsset = assets.FirstOrDefault(fontAsset => fontAsset.faceInfo.familyName == "Daruma Drop One");
            }

            return _darumaFontAsset;
        } 
    }

    public TextMeshProUGUI TextMesh { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        TextMesh = GetComponent<TextMeshProUGUI>();
        RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 1);
        RectTransform.pivot = new Vector2(0, 1);

        TextMesh.font = DarumaDropOne;
        TextMesh.color = Color.white;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();
    }

    public PeakText SetText(string text)
    {
        TextMesh.text = text;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();

        return this;
    }

    public PeakText SetFontSize(float size)
    {
        TextMesh.fontSize = size;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();

        return this;
    }

    public PeakText SetColor(Color color)
    {
        TextMesh.color = color;

        return this;
    }
}
