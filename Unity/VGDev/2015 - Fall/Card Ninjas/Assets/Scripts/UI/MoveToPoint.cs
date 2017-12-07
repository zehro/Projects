using UnityEngine;

namespace Assets.Scripts.Menu
{
	class MoveToPoint : MonoBehaviour
	{
		[SerializeField]
		private float speed;
		
		private Transform moveTo;
		private bool finishedMoving;
		
		void Start()
		{
			finishedMoving = true;
		}
		
		void Update()
		{
			if(!finishedMoving)
			{
				transform.position = Vector3.MoveTowards(transform.position, moveTo.position, speed * Time.deltaTime);
				if (Mathf.Abs(transform.position.x - moveTo.position.x) < .1f)
					finishedMoving = true;
			}
		}
		
		public void MoveTo(Transform moveTo)
		{
			this.moveTo = moveTo;
			finishedMoving = false;
		}
	}
}