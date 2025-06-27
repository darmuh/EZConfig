using System;
using System.Collections.Generic;
using System.Text;
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

        BorderTop = BorderTop.transform.Find("Border").gameObject;
        BorderBottom = transform.GetChild(BorderTop.transform.GetSiblingIndex() + 1).gameObject;

    }

    public void SetText(string text)
    {
        if (Text != null) Text.text = text;
    }

    public void SetColor(Color color)
    {
        if (Panel != null) Panel.color = color;
    }

    public void SetBorderColor(Color color)
    {
        if (BorderTop != null && BorderBottom != null)
        {
            BorderTop.GetComponent<Image>().color = color;
            BorderBottom.GetComponent<Image>().color = color;
        }
    }
}
