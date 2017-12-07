using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Util
{
	public static class AddElement
	{
		#region Materials
		public static Material FireMat = Resources.Load("Elements/FireMat") as Material;
		public static Material EarthMat = Resources.Load("Elements/EarthMat") as Material;
		public static Material ThunderMat = Resources.Load("Elements/ThunderMat") as Material;
		public static Material WaterMat = Resources.Load("Elements/WaterMat") as Material;
		public static Material WoodMat = Resources.Load("Elements/WoodMat") as Material;
		public static Material[] Mats = {FireMat, EarthMat, ThunderMat, WaterMat, WoodMat};
		#endregion
		
		/// <summary>
		/// Adds an Elements visual effects to a Gameobject by enum
		/// </summary>
		/// <param name="obj">Object which needs element effects added.</param>
		/// <param name="element">Element added to the game object.</param>
		public static void AddElementByEnum(GameObject obj, Enums.Element element, bool replaceMat) {
            if (element == Enums.Element.None)
                return;
			GameObject particle = GameObject.Instantiate(Resources.Load("Elements/" + element + "ParticleSys"), obj.transform.position, obj.transform.rotation) as GameObject;
			particle.GetComponent<ParticleFollow>().target = obj.transform.GetChild(0).gameObject;

			if (replaceMat) obj.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = Mats[(int)element];
		}
	}
}
