using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YeggQuest.NS_UI
{
    public class UITooltip : UIElement
    {
        public Text text;

        new void Awake()
        {
            base.Awake();
        }

        new void Update()
        {
            base.Update();
        }

        public void SetTip(string str)
        {
            text.text = str.Replace("<e>", "<color=#FFA34FFF>").Replace("</e>", "</color>").Replace("<n>", System.Environment.NewLine);
        }
    }
}