using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;

namespace YeggQuest.NS_Cam
{
    public class CamStrategyPlayerControlled : CamStrategy
    {
        private Bird bird;
        private Cam cam;

        // Camera variables

        private float rotSpeed = 2f;
        private float rotSpeedLimit = 6f;
        private float rotDrag = 0.1f;
        private float rotX = 20;
        private float rotXTarg = 20;
        private float rotXMin = 0;
        private float rotXMax = 80;
        private float rotY = 0;
        private float rotYTarg = 0;

        private float zoom = 0.4f;
        private float zoomSpeed = 0.025f;
        private float zoomTarg = 0.4f;
        private float zoomDrag = 0.02f;
        private float zoomMinDist = 1.5f;
        private float zoomMaxDist = 9f;

        // Camera result variables

        private Vector3 birdPos;
        private Vector3 anchorPos;
        private Vector3 offsetPos;
        private Vector3 lookAtPos;
        private float fov;

        void Start()
        {
            bird = FindObjectOfType<Bird>();
            cam = FindObjectOfType<Cam>();
            birdPos = bird.animator.transform.position;
        }

        void Update()
        {
            // First, always update the birdPos so it rubberbands to the bird.

            float dt = Time.deltaTime * 60;
            birdPos = Vector3.Lerp(birdPos, bird.animator.transform.position, 0.125f * dt);

            // If the camera is currently being controlled by the player, allow for
            // manual aiming and zooming.

            float c = cam.GetPlayerControl();

            if (c > 0)
            {
                // Aiming control

                float h = Yinput.CameraHorizontal() * c;
                float v = Yinput.CameraVertical() * c;
                h = Mathf.Clamp(h, -rotSpeedLimit, rotSpeedLimit);
                v = Mathf.Clamp(v, -rotSpeedLimit, rotSpeedLimit);
                float s = rotSpeed * dt * Mathf.Lerp(1.25f, 0.75f, zoom);
                rotYTarg += h * s;
                rotXTarg = Mathf.Clamp(rotXTarg + v * s, rotXMin, rotXMax);

                rotY += (rotYTarg - rotY) * rotDrag * dt;
                rotX += (rotXTarg - rotX) * rotDrag * dt;

                // Zooming (distance from bird) control

                float i = Yinput.CameraZoom() * c;
                zoomTarg = Mathf.Clamp01(zoomTarg + i * dt * zoomSpeed);
                zoom += (zoomTarg - zoom) * zoomDrag * dt;
            }

            // If the camera is being overridden, set the variables to closely match the
            // current state (for the smoothest transition back to player control.)

            else
            {
                rotXTarg = cam.transform.localEulerAngles.x;
                if (rotXTarg > rotXMax)
                    rotXTarg = rotXMin;
                rotYTarg = cam.transform.localEulerAngles.y;
                rotX = rotXTarg;
                rotY = rotYTarg;

                float d = Vector3.Distance(birdPos, cam.transform.position);
                zoomTarg = Mathf.Clamp01(Mathf.InverseLerp(zoomMinDist, zoomMaxDist, d));
                zoom = zoomTarg;
            }

            // Setting positions

            float dist = Mathf.Lerp(zoomMinDist, zoomMaxDist, zoom);
            anchorPos = birdPos + Vector3.up * dist / 8;
            offsetPos = Quaternion.Euler(rotX, rotY, 0) * Vector3.back * dist;
            lookAtPos = birdPos + Vector3.up * dist / 4;
            fov = Mathf.Lerp(60, 55, zoom);
        }

        public override CamStrategyResult Direct()
        {
            CamStrategyResult result = new CamStrategyResult();

            result.anchorPosition = anchorPos;
            result.offsetPosition = offsetPos;
            result.lookAtPosition = lookAtPos;
            result.roll = 0;
            result.fov = fov;

            return result;
        }
    }
}