using UnityEngine;
using System.Collections;
using Assets.Scripts.Grid;
using Assets.Scripts.Util;



namespace Assets.Scripts.Weapons
{
    public class TornadoHitbox : Hitbox
    {
        [SerializeField]
        public float zTargetDistance;

        [SerializeField]
        public float xTargetDistance;

        [SerializeField]
        public float zStartingPoint;

        [SerializeField]
        public float xStartingPoint;

        // Update is called once per frame
        public override void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.Battle)
            {
                if (transform.position.z > zTargetDistance && transform.position.x == xStartingPoint)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Speed * Time.deltaTime);
                }
                else if (transform.position.x < xTargetDistance)
                {
                    transform.position = new Vector3(transform.position.x + Speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else if (transform.position.z < zStartingPoint)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Speed * Time.deltaTime);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
