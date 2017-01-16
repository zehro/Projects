using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Data;
using Assets.Scripts.Util;

namespace Assets.Scripts.Attacks
{
	/// <summary>
	/// Attack that steals arrow attributes from the hit player and gives them to the from player
	/// </summary>
	public class VirusAttack : SpawnAttack
	{
		Controller hitController;
		Controller fromController;
		Dictionary<Enums.Arrows, GameObject> lights;
		private float speed = 0.2f;
		private float timeElapsed = 0f;
		private Vector3 hitUp = Vector3.up;
		private Vector3 fromUp = Vector3.up;

		[SerializeField]
		private float waitTime = 1f;
		[SerializeField]
		private float explodeForce = 2.5f;

		void Start()
		{
			damage = 0f;

			fromController = GameManager.instance.GetPlayer(fromPlayer);
			hitController = GameManager.instance.GetPlayer(hitPlayer);

			transform.position = hitController.transform.position + fromUp;

			lights = new Dictionary<Enums.Arrows, GameObject>();
			for(int i = 1; i < (int) Enums.Arrows.NumTypes; i++)
			{
				if(Bitwise.IsBitOn(hitController.ArcheryComponent.ArrowTypes, i))
				{
					//hitController.ArcheryComponent.RemoveArrowType((Enums.Arrows) i);

					GameObject light;
					if(lights.Count > transform.childCount)
					{
						light = (GameObject) Instantiate(transform.GetChild(0).gameObject);
					}
					else
					{
						light = transform.GetChild(0).gameObject;
					}

					float r = ((float) i / ((float) Enums.Arrows.NumTypes - 1)) % 1f;
					float g = (0.33f + (float) i / ((float) Enums.Arrows.NumTypes - 1)) % 1f;
					float b = (0.67f + (float) i / ((float) Enums.Arrows.NumTypes - 1)) % 1f;
					light.GetComponent<Light>().color = new Color(r, g, b);
						
					light.transform.SetParent(transform);
					light.transform.localPosition = Vector3.zero;
					light.GetComponent<Rigidbody>().AddForce(randomInSemicircle(hitUp) * explodeForce, ForceMode.Impulse);
					lights.Add((Enums.Arrows) i, light);
				}
			}

            hitController.ArcheryComponent.ArrowTypes = hitController.ArcheryComponent.PermanentArrowTypes;
				
			if(lights.Count == 0) 
			{
				Destroy(gameObject);
			}
		}

		private Vector3 randomInSemicircle(Vector3 direction)
		{
			Vector3 rand = Random.insideUnitSphere;
			rand.z = 0f;
			if(Mathf.Abs(Vector3.Angle(direction, rand)) > 90)
			{
				rand = -rand;
			}
			return rand;
		}

		void Update()
		{
			timeElapsed += GameManager.instance.deltaTime;
			if(timeElapsed > waitTime)
			{
				speed = Mathf.Min(speed + 0.02f, 1f);
			}

			List<Enums.Arrows> removeLights = new List<Enums.Arrows>();
			Vector3 dest = fromController.transform.position + fromUp;//stopgap for some method in Controller that determines height of a player
			foreach(Enums.Arrows key in lights.Keys)
			{
				if(timeElapsed > waitTime)
				{
					lights[key].transform.position = Vector3.MoveTowards(lights[key].transform.position, dest, speed);
				}

				if(Vector3.Distance(lights[key].transform.position, dest) < 0.5f)
				{
					removeLights.Add(key);
				}
			}

			foreach(Enums.Arrows key in removeLights)
			{
				lights[key].SetActive(false);
				lights.Remove(key);

				fromController.ArcheryComponent.AddArrowType(key);
			}

			if(lights.Count == 0)
			{
				Destroy(gameObject);
			}
		}
	} 
}
