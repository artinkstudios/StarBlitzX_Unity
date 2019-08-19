using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * 300f;
		if (transform.position.z >= 300) {
			Destroy (gameObject);
		}
	}
}
