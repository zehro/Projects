using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Data;

namespace Assets.Scripts.UI.Profiles
{
    /// <summary>
    /// Handles the creation of a player's name when creating a new profile
    /// </summary>
    public class NameCreator : MonoBehaviour
    {
        /// <summary>
        /// Events and Delegate for changing the case of the displayed characters
        /// </summary>
        public delegate void ChangeCase();
        public static event ChangeCase Uppercase, Lowercase;

        // Prefab to load all the available characters
        [SerializeField]
        private GameObject textPrefab;
        // Content panel to put the characters under
        [SerializeField]
        private Transform contentPanel;
        // Reference to the scroll rect
        private ScrollRect rect;

        // Debugging
        public Text t;

        // Character array to hold all available characters
        private char[] characters;

        // Target to scroll to and the step to increase the target by
        private float target = 0, step;

        // Simpler timer that would not work with the event-based timers
        private float delay = 0.1f, timer = 0;

        // Whether or not the displayed letters are capitalized
        private bool capital = true,
        // Used for Windows dpad which is an axis
        dpadPressed = false;

		//Which player is controlling this instance of the creator, Player one by default
		public PlayerID id = PlayerID.One;

        // Current letter highlighted
        private int index = 0;

        void Start()
        {
            // Get the reference to the scroll rect
            rect = GetComponent<ScrollRect>();
            // Available characters
            characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            // Create each character as an object to scroll through
            for(int i = 0; i < characters.Length; i++)
            {
                GameObject newLetter = Instantiate(textPrefab);
                newLetter.GetComponent<Text>().text = characters[i].ToString();
				newLetter.transform.SetParent(contentPanel,false);
            }
            // Step to increase target by
            step = (1.0f / (characters.Length - 1));
        }

        void Update()
        {
            // No axis is being pressed
			if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, id) == 0)
            {
                // Reset the timer so that we don't continue scrolling
                timer = 0;
            }
            // Horizontal joystick is held right
            // Use > 0.5f so that sensitivity is not too high
			else if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, id) > 0.5f)
            {
                // If we can move and it is time to move
                if(index < characters.Length && (timer >= delay || timer == 0))
                {
                    // Move and reset timer
                    Move(1);
                    timer = 0;
					SFXManager.instance.PlayClick();
                }
                timer += Time.deltaTime;
            }
            // Horizontal joystick is held left
            // Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, id) < -0.5f)
            {
                // If we can move and it is time to move
                if (index > 0 && (timer >= delay || timer == 0))
                {
                    // Move and reset timer
                    Move(-1);
                    timer = 0;
					SFXManager.instance.PlayClick();
                }
                timer += Time.deltaTime;
            }
            // A button is pressed
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, id))
            {
                // Debug - add letter to overall string
                t.text += capital ? characters[index].ToString() : characters[index].ToString().ToLower();
				SFXManager.instance.PlayAffirm();
            }
            // Left joystick button is pressed
			if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.LeftStickClick, id))
            {
                // Check capitalization
                if(capital)
                {
                    // Send event to make all characters lowercase
                    if (Lowercase != null) Lowercase();
                    capital = false;
                }
                else
                {
                    // Send event to make all letters uppercase
                    if (Uppercase != null) Uppercase();
                    capital = true;
                }
            }
            // Y button is pressed; add a space
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Y, id)) t.text += " ";
            // X button is pressed; delete last character added
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.X, id) && t.text != "") t.text = t.text.Remove(t.text.Length - 1); 
            // Have dpad functionality so that player can have precise control and joystick quick navigation
            // Check differently for Windows vs OSX

            // No dpad button is pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX, id) == 0) dpadPressed = false;
            // Dpad right is pressed; treating as DPADRightOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX, id) > 0 && !dpadPressed && index <characters.Length)
            {
                dpadPressed = true;
                Move(1);
				SFXManager.instance.PlayClick();
            }
            // Dpad right is pressed; treating as DPADLeftOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX, id) < 0 && !dpadPressed && index > 0)
            {
                dpadPressed = true;
                Move(-1);
				SFXManager.instance.PlayClick();
            }

			if(t.text.Length > 4) {
				t.text = t.text.Substring(0,4);
			}

            // Current position of the horizontal scrollrect amount
            Vector3 rectPos = new Vector3(rect.horizontalNormalizedPosition, 0, 0);
            // Position to scroll to
            Vector3 targetPos = new Vector3(target, 0, 0);
            // Lerpo from current to target
            rect.horizontalNormalizedPosition = Vector3.Lerp(rectPos, targetPos, 0.1f).x;
            // Clamp from 0-1 as is the range of horizontalNormalizedPosition
            rect.horizontalNormalizedPosition = Mathf.Clamp01(rect.horizontalNormalizedPosition);

//			if(ControllerManager.instance.GetButton(ControllerInputWrapper.Buttons.Start, PlayerID.One))
//            {
//                ProfileData data;
//                if (t.text != "")
//                    data = new ProfileData(t.text);
//            }
        }

        /// <summary>
        /// Scroll the list of characters in a certain direction
        /// </summary>
        /// <param name="dir">The direction to scroll</param>
        public void Move(int dir)
        {
            // Increase/decrease the index of the current character
			if(index + dir >= 0 && index + dir <= characters.Length)
				index += dir;

            // Set the target of the scroll to lerp to
            target += step * dir;
        }

		/// <summary>
		/// Resets the name creator.
		/// </summary>
		public void Reset()
		{
			t.text = "";
			target = 0;
			index = 0;
			rect.horizontalNormalizedPosition = 0;
		}
    }
}