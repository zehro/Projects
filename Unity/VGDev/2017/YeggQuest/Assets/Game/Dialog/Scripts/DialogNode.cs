using UnityEngine;

namespace YeggQuest.NS_Dialog
{
    /// <summary>
    /// Node class for the dialoge graph.  Set up to be immutable at runtime.
    /// </summary>
    public class DialogNode : MonoBehaviour
    {
        /// <summary> The text file holding the dialog for this node. </summary>
        [SerializeField]
        [Tooltip("The text file holding the dialog for this node.")]
        private TextAsset dialog;

        /// <summary> The dialog for this node. </summary>
        public string Dialog { get { hasBeenRead = true; return dialog.text; } }

        /// <summary> Retrieve the dialog string for this node without setting HasBeenRead. </summary>
        public string DialogNoRead { get { return dialog.text; } }

        /// <summary> Name of the object saying this dialog. </summary>
        [SerializeField]
        [Tooltip("Name of the object saying this dialog.")]
        private string speaker;

        /// <summary> Name of the object saying this dialog. </summary>
        public string Speaker { get { return speaker; } }

        /// <summary> Boolean saving if this dialog has been read. </summary>
        internal bool hasBeenRead;

        /// <summary> Boolean saving if this dialog has been read. </summary>
        public bool HasBeenRead { get { return hasBeenRead; } }

        /// <summary> Require this node has been read before allowing transition out. </summary>
        [SerializeField]
        internal bool requireRead;

#if UNITY_EDITOR
        /// <summary> Internal var used to change the debug display. </summary>
        internal bool isSelected;

        private void OnDrawGizmos()
        {
            if (isSelected)
                Gizmos.color = Color.green;
            else if (hasBeenRead)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, .1f);
        }
#endif
    }
}
