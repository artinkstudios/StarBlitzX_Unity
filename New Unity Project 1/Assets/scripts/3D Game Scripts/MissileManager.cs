using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour {

	public float speed = 2;
	public Transform Earth;
	private AudioSource MissileAudio;
	public AudioClip MissileDestroyed;

	void Start () {
		MissileAudio = GetComponent<AudioSource> ();
		if (ApplicationValues.isHard) {
			speed = speed * 2;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, Earth.position, step);
	}

	void OnDestroy(){
		MissileAudio.PlayOneShot (MissileDestroyed);
        Camera.main.GetComponent<GameManager>().RemoveAsteroid(gameObject);
    }
}
