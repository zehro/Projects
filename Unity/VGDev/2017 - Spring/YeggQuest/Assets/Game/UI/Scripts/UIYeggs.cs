using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YeggQuest.NS_UI
{
    public class UIYeggs : UIElement
    {
        public Text text;
        public Image glow1;
        public Image glow2;

        new void Awake()
        {
            base.Awake();
            Recount();
        }

        new void Update()
        {
            base.Update();

            glow1.rectTransform.localRotation = Quaternion.Euler(0, 0, Time.time * +10f);
            glow2.rectTransform.localRotation = Quaternion.Euler(0, 0, Time.time * -15f);
        }

        public void Recount()
        {
            text.text = GameData.GetYeggCount().ToString();
        }
    }
}