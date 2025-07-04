using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EZConfig.UI;
#nullable disable
public class PeakMenuButton : PeakElement
{
    public Button Button { get; private set; }
    public Image Panel { get; private set; }
    public Image BorderTop { get; private set; }
    public Image BorderBottom { get; private set; }
    public TextMeshProUGUI Text { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Button = GetComponent<Button>();
        Panel = transform.Find("Panel").GetComponent<Image>();
        Text = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        // There are two objects named "Border" so we need to do this hack to get the bottom one
        BorderTop = transform.Find("Border").GetComponent<Image>(); 
        BorderBottom = transform.GetChild(BorderTop.transform.GetSiblingIndex() + 1).GetComponent<Image>();
    }

    public PeakMenuButton SetText(string text)
    {
        if (Text != null) Text.text = text;

        return this;
    }

    public PeakMenuButton SetColor(Color color, bool automaticBorderColor = true)
    {
        if (Panel != null) Panel.color = color;
        if (automaticBorderColor) SetBorderColor(UIUtilities.GetContrastingColor(color));

        return this;
    }

    public PeakMenuButton SetBorderColor(Color color)
    {
        if (BorderTop != null && BorderBottom != null)
        {
            BorderTop.color = color;
            BorderBottom.color = color;
        }

        return this;
    }

    public PeakMenuButton SetWidth(float width)
    {
        if (RectTransform != null)
            RectTransform.sizeDelta = new Vector2(width, RectTransform.sizeDelta.y);

        return this;
    }

    public PeakMenuButton OnClick(UnityAction onClickEvent)
    {
        Button.onClick.AddListener(onClickEvent);
        return this;
    }
}
