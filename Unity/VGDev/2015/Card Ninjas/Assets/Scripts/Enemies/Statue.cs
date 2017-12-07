using UnityEngine;

namespace Assets.Scripts.Enemies
{
    class Statue : Enemy
    {
        [SerializeField]
        private MeshRenderer body;

        protected override void Initialize()
        {
        }

        protected override void RunAI()
        {
        }

        protected override void Render(bool render)
        {
            body.enabled = render;
        }
    }
}
