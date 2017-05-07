using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YeggQuest.NS_UI;

// A LogicalYeggDemand is a Logical object which returns whether or not
// the player has obtained the given required amount of yeggs.
// It shows this demand in an attached UIBalloon.

namespace YeggQuest.NS_Logic
{
    public class LogicalYeggDemand : Logical
    {
        public int requiredAmount = 1;
        public bool checkAutomatically = true;

        [Space(10)]
        [Header("UI References")]
        public UIBalloon balloon;
        public Text text;
        public UIVolume volume;

        private bool satisfied;
        
        void Update()
        {
            text.text = requiredAmount.ToString();

            if (Application.isPlaying)
            {
                if (checkAutomatically)
                    Check();
                balloon.SetOpen(!satisfied);
                volume.enabled = !satisfied;
            }
        }

        void OnValidate()
        {
            if (requiredAmount < 1)
                requiredAmount = 1;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "LogicalYeggDemand.png");
        }
        
        public void Check()
        {
            satisfied = GameData.GetYeggCount() >= requiredAmount;
        }

        public override bool Evaluate()
        {
            return satisfied;
        }
    }
}