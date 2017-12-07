using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Menu
{
	class Instructions : MonoBehaviour
	{
		[SerializeField]
		private MoveToPoint[] images;
		[SerializeField]
		private Transform left;
		[SerializeField]
		private Transform middle;
		[SerializeField]
		private Transform right;
		[SerializeField]
		private GameObject button;
		
		public int currentImage;
		
		void Start()
		{
			Init();
		}
		
		void Update()
		{
            if (EventSystem.current.currentSelectedGameObject == button)
            {
                if (currentImage > 0 && Util.CustomInput.BoolFreshPress(Util.CustomInput.UserInput.Left))
                    RightShift();
                if (currentImage < images.Length - 1 && Util.CustomInput.BoolFreshPress(Util.CustomInput.UserInput.Right))
                    LeftShift();

                if (Util.CustomInput.BoolFreshPress(Util.CustomInput.UserInput.Cancel))
                {
                    Init();
                    Navigator.CallSubmit();
                }
            }
		}
		
		private void Init()
		{
			currentImage = 0;
			images[0].gameObject.transform.position = middle.position;
			images[0].MoveTo(middle);
			for (int i = 1; i < images.Length; i++)
			{
				images[i].gameObject.transform.position = right.position;
				images[i].MoveTo(right);
			}
		}
		
		private void LeftShift()
		{
			images[currentImage].gameObject.transform.SetAsLastSibling();
			button.transform.SetAsLastSibling();
			images[currentImage].MoveTo(left);
			images[currentImage + 1].MoveTo(middle);
			currentImage++;
		}
		
		private void RightShift()
		{
			images[currentImage].gameObject.transform.SetAsLastSibling();
			button.transform.SetAsLastSibling();
			images[currentImage].MoveTo(right);
			images[currentImage - 1].MoveTo(middle);
			currentImage--;
		}
		
		public void Left()
		{
			if (currentImage > 0)
				RightShift();
		}
		
		public void Right()
		{
			if (currentImage < images.Length - 1)
				LeftShift();
		}
	}
}