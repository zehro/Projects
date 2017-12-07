using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{ 
    public class CanvasHider : MonoBehaviour
    {
        void OnEnable()
        {
            LoadingScreen.BeginLoadLevel += HideCanvas;
        }
        void OnDisable()
        {
            LoadingScreen.BeginLoadLevel -= HideCanvas;
        }

        private void HideCanvas()
        {
            this.GetComponent<Canvas>().enabled = false;
        }
    }
}
