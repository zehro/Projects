using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Util;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Used to circumvent Unity's UI navigation
    /// </summary>
    public static class Navigator
    {
        /// <summary>
        /// The default gameobject to select if nothing is selected when moving
        /// </summary>
        public static GameObject defaultGameObject;

        /// <summary>
        /// Navigates the current open UI
        /// </summary>
        /// <param name="direction">The direction to move</param>
        public static void Navigate(Enums.MenuDirections direction)
		{
            // Get the currently selected gameobject
            GameObject next = EventSystem.current.currentSelectedGameObject;
            if (next == null)
            {
                // Select the default if no object is selected already
                if (defaultGameObject != null) EventSystem.current.SetSelectedGameObject(defaultGameObject);
                return;
            }

            // This will go through the next UI elements until a valid match is found
            bool nextIsValid = false;
            while (!nextIsValid)
            {
                switch (direction)
                {
                    case Enums.MenuDirections.Up:
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() != null)
                            next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject;
                        else next = null;
                        break;
                    case Enums.MenuDirections.Down:
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown() != null)
                            next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject;
                        else next = null;
                        break;
                    case Enums.MenuDirections.Left:
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft() != null)
                            next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().gameObject;
                        else next = null;
                        break;
                    case Enums.MenuDirections.Right:
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight() != null)
                            next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().gameObject;
                        else next = null;
                        break;
                }
                if (next != null)
                {
                    // Stop of the next componenet is active and enabled
                    EventSystem.current.SetSelectedGameObject(next);
                    nextIsValid = next.GetComponent<Selectable>().interactable;
					SFXManager.instance.PlayWhoosh();
                }
                else nextIsValid = true;
            }
        }

        /// <summary>
        /// Calls the submit on the currently selected UI object
        /// </summary>
        public static void CallSubmit()
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.submitHandler);
				SFXManager.instance.PlayAffirm();
            }
        }

		public static void CallCancel()
		{
			if (EventSystem.current.currentSelectedGameObject)
			{
				var pointer = new PointerEventData(EventSystem.current);
				ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.cancelHandler);
				SFXManager.instance.PlayNegative();
			}
		}
    } 
}