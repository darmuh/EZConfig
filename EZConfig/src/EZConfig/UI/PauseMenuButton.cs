using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EZConfig.UI;

public class PauseMenuButton : MonoBehaviour
{
    public GameObject Shadow;
    public Image Panel;
    public GameObject BorderTop;
    public GameObject BorderBottom;
    public GameObject Glow;
    public TextMeshProUGUI Text;
    public SFX_PlayOneShot[] SoundEffects;

    void Awake()
    {
        Shadow = transform.Find("Shadow").gameObject;
        Panel = transform.Find("Panel").GetComponent<Image>();
        Glow = transform.Find("Glow").gameObject;
        Text = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        // There are two objects named "Border" so we need to do this hack to get the bottom one
        BorderTop = BorderTop.transform.Find("Border").gameObject; 
        BorderBottom = transform.GetChild(BorderTop.transform.GetSiblingIndex() + 1).gameObject;
    }

    public PauseMenuButton SetText(string text)
    {
        if (Text != null) Text.text = text;

        return this;
    }

    public PauseMenuButton SetColor(Color color)
    {
        if (Panel != null) Panel.color = color;

        return this;
    }

    public PauseMenuButton SetBorderColor(Color color)
    {
        if (BorderTop != null && BorderBottom != null)
        {
            BorderTop.GetComponent<Image>().color = color;
            BorderBottom.GetComponent<Image>().color = color;
        }

        return this;
    }


    public PauseMenuButton SetParent(Transform parent)
    {
        transform.SetParent(parent);

        return this;
    }
}
