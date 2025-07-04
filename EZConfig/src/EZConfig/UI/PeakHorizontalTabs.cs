using EZConfig.Components;
using UnityEngine;
using UnityEngine.UI;

namespace EZConfig.UI;
#nullable disable
[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(RectMask2D))]
public class PeakHorizontalTabs : PeakElement
{
    public ScrollRect ScrollRect { get; private set; }
    public RectTransform Content {  get; private set; }
    public HorizontalLayoutGroup LayoutGroup { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        ScrollRect = GetComponent<ScrollRect>();

        RectTransform.anchoredPosition = new Vector2(0, 0);
        RectTransform.sizeDelta = new Vector2(0, 40);
        RectTransform.pivot = new Vector2(0.5f, 1);
        RectTransform.anchorMin = new Vector2(0, 1);
        RectTransform.anchorMax = new Vector2(1, 1);

        var contentObj = new GameObject("Content", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        Content = contentObj.GetComponent<RectTransform>();
        Content.SetParent(transform, false);
        Content.pivot = Vector2.zero;
        UIUtilities.ExpandToParent(Content);

        ScrollRect.content = Content;
        ScrollRect.scrollSensitivity = 50;
        ScrollRect.elasticity = 0;
        ScrollRect.vertical = false;
        ScrollRect.horizontal = true;
        ScrollRect.movementType = ScrollRect.MovementType.Clamped;

        LayoutGroup = Content.GetComponent<HorizontalLayoutGroup>();
        LayoutGroup.spacing = 20;
        LayoutGroup.childControlWidth = true;
        LayoutGroup.childForceExpandWidth = true;
        LayoutGroup.childForceExpandHeight = true;
        LayoutGroup.childControlHeight = true;

        var contentSizeFitter = Content.GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void AddTab(string tabName)
    {
        var gameObject = new GameObject(tabName, typeof(RectTransform), typeof(Button), typeof(LayoutElement));
        gameObject.transform.SetParent(Content, false);

        var layout = gameObject.GetComponent<LayoutElement>();
        layout.minWidth = 220;
        layout.flexibleWidth = 1;
        layout.preferredHeight = 40;

        var imageObject = new GameObject("Image", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(gameObject.transform, false);
        UIUtilities.ExpandToParent(imageObject.transform as RectTransform);

        var image = imageObject.GetComponent<Image>();
        image.color = new Color(0.1792453f, 0.1253449f, 0.09046815f, 0.7294118f);

        var selected = new GameObject("Selected", typeof(RectTransform), typeof(Image));
        selected.transform.SetParent(gameObject.transform, false);

        UIUtilities.ExpandToParent(selected.transform as RectTransform);

        var peakText = new GameObject("Text (TMP)", typeof(PeakText)).GetComponent<PeakText>();
        peakText.transform.SetParent(gameObject.transform, false);

        var textRect = peakText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        peakText.TextMesh.enableAutoSizing = true;
        peakText.TextMesh.fontSizeMin = 18;
        peakText.TextMesh.fontSizeMax = 22;
        peakText.TextMesh.fontSize = 22;
        peakText.TextMesh.fontStyle = TMPro.FontStyles.UpperCase;
        peakText.TextMesh.alignment = TMPro.TextAlignmentOptions.Center;
        peakText.TextMesh.text = tabName;

        var moddedButton = gameObject.AddComponent<ModdedTABSButton>();
        moddedButton.category = tabName;
        moddedButton.text = peakText.TextMesh;
        moddedButton.SelectedGraphic = selected;
    }
}
