using System.Text;
using UnityEngine;

namespace YeggQuest.NS_UI
{
    public class UIDigit : MonoBehaviour
    {
        public UnityEngine.UI.Text text;
        public NS_Logic.LogicalPaintDemand[] digits;

        void Update()
        {
            int a = 0;
            foreach (NS_Logic.LogicalPaintDemand button in digits)
            {
                bool val = button.Evaluate();
                a |= val ? 1 : 0;
                a = a << 1;
            }
            a = a >> 1;
            text.text = a + "";
        }
    }
}