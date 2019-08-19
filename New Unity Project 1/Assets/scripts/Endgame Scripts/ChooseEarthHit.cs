using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseEarthHit : MonoBehaviour {

	public Material[] HitMaterials;

	void Start () {
		if (ApplicationValues.EarthHealth < 10) {
			GetComponent<Renderer> ().material = HitMaterials [9 - ApplicationValues.EarthHealth];
		}
	}

}
