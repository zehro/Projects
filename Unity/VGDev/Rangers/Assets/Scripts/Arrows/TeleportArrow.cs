using System;
using UnityEngine;

namespace Assets.Scripts.Arrows
{
    public class TeleportArrow : ArrowProperty
    {  
       
       public override void Effect(PlayerID hitPlayer)
        {
            if (hitPlayer == PlayerID.None)
            {
                Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(fromPlayer)).gameObject.transform.position = transform.position;
            } else
            {
                Vector3 holder = Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(hitPlayer)).gameObject.transform.position;
                Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(hitPlayer)).gameObject.transform.position = Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(fromPlayer)).gameObject.transform.position;
                Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(fromPlayer)).gameObject.transform.position = holder;
            }            
        }
    }
}


