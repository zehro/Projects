using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Data
{
    public abstract class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;
        protected bool running = false;

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public abstract void InitializeMatch();
        protected abstract void GameOver();
    }
}
