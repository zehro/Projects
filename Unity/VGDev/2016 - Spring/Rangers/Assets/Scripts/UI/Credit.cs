using UnityEngine;

namespace Assets.Scripts.UI
{
    public class Credit : MonoBehaviour
    {
        private TextMesh t;
        private float alpha = 0f;

        void Start()
        {
            t = GetComponent<TextMesh>();
            Color c = t.GetComponent<Renderer>().material.GetColor("_Color");
            Color withAlpha = new Color(c.r, c.g, c.b, alpha);
            t.GetComponent<Renderer>().material.SetColor("_Color", withAlpha);
        }

        void Update()
        {
            alpha = Mathf.Lerp(alpha, 1f, 0.01f);
            Color c = t.GetComponent<Renderer>().material.GetColor("_Color");
            Color withAlpha = new Color(c.r, c.g, c.b, alpha);
            t.GetComponent<Renderer>().material.SetColor("_Color", withAlpha);
        }
    } 
}
