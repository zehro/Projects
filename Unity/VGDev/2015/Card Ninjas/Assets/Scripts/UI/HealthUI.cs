using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HealthUI : MonoBehaviour
    {
        private float xScale = 1f, initHealth = 100f;

        private static int playerIndex = 0;

        private int thisPlayerIndex;

        private Transform healthBar;

        void OnEnable()
        {
            Player.Player.UpdateHealth += UpdateUI;
        }
        void OnDisable()
        {
            Player.Player.UpdateHealth -= UpdateUI;
        }

        void Start()
        {
            thisPlayerIndex = ++playerIndex;
            healthBar = GameObject.Find("Health Bar " + thisPlayerIndex.ToString()).transform;
        }

        public void UpdateUI(float health, int playerIndex)
        {
            if (thisPlayerIndex != playerIndex) return;

            xScale = Mathf.Clamp(health / initHealth, 0, 1);

            healthBar.localScale = new Vector3(xScale, 1, 1);
        }

        void OnLevelWasLoaded(int i)
        {
            playerIndex = 0;
        }
    }
}