using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCameraRotate : MonoBehaviour {
	private Vector3 cam = new Vector3(-0.3f,0,0);
	private bool start = false;
	public GameObject Satelite;
	public GameObject[] fireworks;

	void Start () {
		StartCoroutine (RotateCamera ());
		StartCoroutine (StartFireworks ());
	}

	void Update () {
		if (start && this.transform.rotation.eulerAngles.x > 360- 60) {
			this.transform.Rotate (cam);
			if (this.transform.rotation.eulerAngles.x > 360 - 60) {
				Satelite.GetComponent<SataliteMove> ().StartMoving ();
			}
		}
	}

	private IEnumerator RotateCamera(){
		yield return new WaitForSeconds (3f);
		start = true;
	}

	private IEnumerator StartFireworks(){
		float time = 5.0f / fireworks.Length;
		for (int i = 0; i < fireworks.Length; i++) {
			fireworks [i].GetComponent<ParticleSystem> ().Play ();
			yield return new WaitForSeconds (time);
		}
	}
}
