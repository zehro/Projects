using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Profiles
{
    /// <summary>
    /// Displays a character in for name creation
    /// </summary>
    public class DisplayCharacter : MonoBehaviour
    {
        // Text component to display the character
        private Text textComponent;

        // Setting up events
        void OnEnable()
        {
            NameCreator.Uppercase += Upper;
            NameCreator.Lowercase += Lower;
        }
        void OnDisable()
        {
            NameCreator.Uppercase -= Upper;
            NameCreator.Lowercase -= Lower;
        }

        void Start()
        {
            // Get the text component
            textComponent = GetComponent<Text>();
        }

        /// <summary>
        /// Capitalize the text
        /// </summary>
        private void Upper()
        {
            textComponent.text = textComponent.text.ToUpper();
        }
        /// <summary>
        /// Uncapitalize the text
        /// </summary>
        private void Lower()
        {
            textComponent.text = textComponent.text.ToLower();
        }
    }
}