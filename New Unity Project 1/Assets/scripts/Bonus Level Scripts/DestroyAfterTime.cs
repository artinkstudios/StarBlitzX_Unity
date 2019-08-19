using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

	public float TimeUntilDestroy = 3;


	void Update () {
		TimeUntilDestroy -= Time.deltaTime;
		if (TimeUntilDestroy <= 0) {
			Destroy (gameObject);
		}
	}
}
