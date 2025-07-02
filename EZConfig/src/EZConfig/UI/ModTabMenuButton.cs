using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zorro.UI;

namespace EZConfig.UI
{
    public class ModTabMenuButton : TAB_Button
    {
        public GameObject SelectedGraphic = null!;
        public string SectionName = "Settings";

        public void Update()
        {
            if (SelectedGraphic == null)
                return;

            Color b = (base.Selected ? Color.black : Color.white);
            text.color = Color.Lerp(text.color, b, Time.unscaledDeltaTime * 7f);
            SelectedGraphic.gameObject.SetActive(base.Selected);
        }
    }
}
