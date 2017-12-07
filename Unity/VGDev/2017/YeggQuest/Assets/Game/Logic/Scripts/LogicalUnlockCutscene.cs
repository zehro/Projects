using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Cam;

namespace YeggQuest.NS_Logic
{
    public class LogicalUnlockCutscene : Logical
    {
        public Logical input;
        public CamCutscene cutscene;
        public float unlockTime;

        private bool trigger;
        private bool unlock;

        void Update()
        {
            if (!Application.isPlaying)
                return;

            if (!trigger && Logic.SafeEvaluate(input, false))
            {
                trigger = true;
                FindObjectOfType<Cam>().PlayCutscene(cutscene);
                Invoke("Unlock", unlockTime);
            }
        }

        void Unlock()
        {
            unlock = true;
        }

        public override bool Evaluate()
        {
            return unlock;
        }
    }
}