﻿namespace ShiftingDungeon.Effects
{
    using UnityEngine;

    public class Spin : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private bool spinOnStart = false;
        [SerializeField]
        private int startSpins = 10;
        [SerializeField]
        private bool activeInCutscenes = false;

        private float theta;
        private float initRot;
        private int spins;
        private int spinCount;
        private bool spinForever;

        private void Start()
        {
            this.theta = 0f;
            this.initRot = this.transform.localEulerAngles.z;
            this.spins = 0;
            this.spinCount = 0;
            this.spinForever = false;
            if (this.spinOnStart)
                StartSpin(this.startSpins);
        }

        private void Update()
        {
            if (Managers.GameState.Instance.IsPaused &&
                !(this.activeInCutscenes && Managers.GameState.Instance.State == Util.Enums.GameState.Cutscene))
                return;

            if (this.spins < this.spinCount || this.spinForever)
            {
                this.theta += Time.deltaTime * this.speed;
                if (this.theta > 360f)
                {
                    this.theta = 0f;
                    this.spins++;
                }

                this.transform.localRotation = Quaternion.Euler(0f, 0f, this.theta + this.initRot);
            }
        }

        public void StartSpin(int count)
        {
            this.theta = 0f;
            this.spins = 0;
            this.spinCount = count;
            if (count == -1)
                this.spinForever = true;
        }
    }
}
