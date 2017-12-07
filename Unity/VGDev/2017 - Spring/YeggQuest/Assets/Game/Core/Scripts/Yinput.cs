using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An input wrapper for the game. Used to genericize keyboard and joystick
// controls (TODO: may be replaced with a better one later)

namespace YeggQuest
{
    public static class Yinput
    {
        // The horizontal movement of the bird

        public static float MovementHorizontal()
        {
            return Input.GetAxisRaw("Movement Horizontal");
        }

        // The vertical movement of the bird

        public static float MovementVertical()
        {
            return Input.GetAxisRaw("Movement Vertical");
        }

        // The horizontal movement of the camera

        public static float CameraHorizontal()
        {
            return Input.GetAxisRaw("Camera Horizontal") * (GameData.camHInvert ? -1 : 1);
        }

        // The vertical movement of the camera

        public static float CameraVertical()
        {
            return Input.GetAxisRaw("Camera Vertical") * (GameData.camVInvert ? -1 : 1);
        }

        // The zoom of the camera

        public static float CameraZoom()
        {
            float joy = Input.GetAxisRaw("Camera Zoom (Gamepad)");
            float mouse = Input.GetAxisRaw("Camera Zoom (Mouse)") * 5;

            return joy + mouse;
        }

        // The bird jumping / honking

        public static bool BirdJump()
        {
            return Input.GetButtonDown("Jump");
        }

        // The bird switching between rolling and walking

        public static bool BirdSwap()
        {
            return Input.GetButtonDown("Start Rolling");
        }

        // The ability to view the UI elements

        public static bool ViewUI()
        {
            return Input.GetButton("View UI");
        }

        // Pausing

        public static bool Pause()
        {
            return Input.GetButtonDown("Pause");
        }

        // Canceling in menus

        public static bool Cancel()
        {
            return Input.GetButtonDown("Cancel");
        }
    }
}