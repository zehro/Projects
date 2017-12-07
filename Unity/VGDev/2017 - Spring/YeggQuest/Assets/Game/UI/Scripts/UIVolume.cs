using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YeggQuest.NS_Bird;

namespace YeggQuest.NS_UI
{
    public class UIVolume : MonoBehaviour
    {
        public UIVolumeType type;
        public string tooltip;
        public Vector3 size = Vector3.one;
        public float showTime = 1;

        private Bird bird;
        private Bounds bounds;
        private UI ui;

        void Start()
        {
            bird = FindObjectOfType<Bird>();
            bounds = new Bounds();
            ui = FindObjectOfType<UI>();
        }

        void Update()
        {
            bounds.center = transform.position;
            bounds.size = size;

            if (bounds.Contains(bird.GetPosition()))
            {
                switch (type)
                {
                    case UIVolumeType.BPaint:
                        ui.bPaint.Show(showTime);
                        break;

                    case UIVolumeType.Yeggs:
                        ui.yeggs.Show(showTime);
                        break;

                    case UIVolumeType.GPaint:
                        ui.gPaint.Show(showTime);
                        break;

                    case UIVolumeType.Tooltip:
                        ui.tooltip.SetTip(tooltip);
                        ui.tooltip.Show(showTime);
                        break;
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, size);
        }
    }

    public enum UIVolumeType
    {
        BPaint, Yeggs, GPaint, Tooltip
    }
}