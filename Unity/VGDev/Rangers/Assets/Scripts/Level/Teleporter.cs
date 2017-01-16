using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Player;
using Assets.Scripts.Timers;

namespace Assets.Scripts.Levels
{
    public class Teleporter : MonoBehaviour
{ 
        public Transform portal;
		private Queue<TrailRenderer> toDisable;

		void Start()
		{
			toDisable = new Queue<TrailRenderer>();
		}

        void OnTriggerEnter(Collider other)
        {
			if(!other.transform.root.gameObject.name.Equals("Level")) {
				other.transform.root.position = portal.position + transform.forward*2f + Vector3.up;
			}
        }

		void OnTriggerExit(Collider other)
		{
			if(other.transform.root.gameObject.layer == LayerMask.NameToLayer("Arrow")) {
				TrailRenderer trail = other.transform.root.GetComponentInChildren<TrailRenderer>();
				toDisable.Enqueue(trail);toDisable.Dequeue().Clear();
				CountdownTimer timer = CountdownTimer.CreateTimer(trail.gameObject, 0.001f, "TrailOff", EnableTrailRenderer);
			}
		}

		void EnableTrailRenderer(CountdownTimer t)
		{
			if(toDisable.Count > 0)
			{
				toDisable.Dequeue().Clear();
			}
		}
    }
}