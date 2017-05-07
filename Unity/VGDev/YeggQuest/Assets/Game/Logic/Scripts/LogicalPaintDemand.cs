using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YeggQuest.NS_Paint;
using YeggQuest.NS_UI;

// A LogicalPaintDemand is a Logical object which returns whether or not
// the given PaintableSimple object is painted the given PaintColor.
// It shows this demand in an attached UIBalloon.

namespace YeggQuest.NS_Logic
{
    public class LogicalPaintDemand : Logical
    {
        public PaintableSimple paintable;
        [Header("... must be...")]
        public PaintColor color;

        [Space(10)]
        [Header("UI References")]
        public UIBalloon balloon;
        public Image icon;

        void Start()
        {
            if (Application.isPlaying && paintable == null)
                paintable = GetComponent<PaintableSimple>();
        }

        void Update()
        {
            if (icon)
                icon.color = PaintColors.ToColor(color);
            if (Application.isPlaying && balloon)
                balloon.SetOpen(!Evaluate());
        }

        void OnDrawGizmos()
        {
            Gizmos.color = PaintColors.ToColor(color);
            if (paintable)
                Gizmos.DrawSphere(paintable.transform.position, 0.25f);
            Gizmos.DrawIcon(transform.position, "LogicalPaintDemand.png");
        }

        public override bool Evaluate()
        {
            if (!paintable)
                return false;
            return paintable.IsPainted(color);
        }
    }
}