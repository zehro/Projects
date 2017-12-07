using UnityEngine;
using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Util;

public class PermanentArrowTypeTester : MonoBehaviour {

	void Start () {
		GameManager.instance.GetPlayer(PlayerID.One).ArcheryComponent.ArrowTypes = 
			Bitwise.SetBit(GameManager.instance.GetPlayer(PlayerID.One).ArcheryComponent.ArrowTypes, (int)Enums.Arrows.Fireball);
	}
}
