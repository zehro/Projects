using UnityEngine;
using Assets.Scripts.Util;

namespace Assets.Test
{
    class InputTest : MonoBehaviour
    {
        public int playerNum=0;
        private string[] inputs = { CustomInput.LEFT_STICK_RIGHT, CustomInput.LEFT_STICK_LEFT, CustomInput.LEFT_STICK_UP, CustomInput.LEFT_STICK_DOWN,
                                    CustomInput.RIGHT_STICK_RIGHT, CustomInput.RIGHT_STICK_LEFT, CustomInput.RIGHT_STICK_UP, CustomInput.RIGHT_STICK_DOWN,
                                    CustomInput.DPAD_RIGHT, CustomInput.DPAD_LEFT, CustomInput.DPAD_UP, CustomInput.DPAD_DOWN, CustomInput.LEFT_TRIGGER,
                                    CustomInput.RIGHT_TRIGGER, CustomInput.A, CustomInput.B, CustomInput.X, CustomInput.Y, CustomInput.LB, CustomInput.RB,
                                    CustomInput.BACK, CustomInput.START, CustomInput.LEFT_STICK_CLICK, CustomInput.RIGHT_STICK_CLICK };
        private int currentInput = 0;

        void Start()
        {
            Debug.Log(inputs[currentInput]);
            CustomInput.setGamepadButton(CustomInput.UserInput.Accept, inputs[currentInput], playerNum);
        }

        void Update()
        {
            if (CustomInput.Bool(CustomInput.UserInput.Accept, playerNum))
                transform.position = new Vector3(transform.position.x, CustomInput.Raw(CustomInput.UserInput.Accept, playerNum), transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            if(Input.GetKeyUp(KeyCode.P))
            {
                currentInput++;
                if (currentInput >= inputs.Length)
                    currentInput = 0;
                Debug.Log(inputs[currentInput]);
                CustomInput.setGamepadButton(CustomInput.UserInput.Accept, inputs[currentInput], playerNum);
            }
        }
    }
}
