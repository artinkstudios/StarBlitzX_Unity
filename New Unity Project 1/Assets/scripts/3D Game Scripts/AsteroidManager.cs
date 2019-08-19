using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour {

	public bool isActive = false;
	public bool isBig;
	public float speed = 2;
	private Vector3 Rot;
	private Vector3 force = new Vector3 (0,0,0);
	public string type;
	public float MaxRotation = 3;
	public Vector3 Earth;
	public int health = 3;
	//public bool SpecialEffective = false;
	private bool exiting = false;

	// Use this for initialization
	void Start () {
		exiting = false;
		//SpecialEffective = false;

		if (!isBig) {
			speed = speed * 2;
			transform.localScale += (Random.insideUnitSphere / 8);
		} else {
			transform.localScale += (Random.insideUnitSphere / 4);
		}
		if (ApplicationValues.isHard) {
			speed = speed * 2;
		}
		Rot = Random.insideUnitSphere * MaxRotation;

		if (Camera.main.GetComponent<GameManager> ().level == Camera.main.GetComponent<GameManager> ().LastLevel) {
			Camera.main.GetComponent<GameManager> ().AddAsteroidInPlay ();
		}
		isActive = true;
	}
	

	void FixedUpdate () {
		transform.Rotate (Rot.x, Rot.y, Rot.z);

		if (!Camera.main.GetComponent<GameManager>().WaitBetween && type.CompareTo ("phantom") == 0) {
			float step = speed * Time.deltaTime;
			transform.parent.position = Vector3.MoveTowards (transform.parent.position, Earth, step);
			//GetComponentInParent<Transform> ().position = Vector3.MoveTowards (GetComponentInParent<Transform> ().position, Earth.position, step);
		} else if (!Camera.main.GetComponent<GameManager>().WaitBetween) {
			/*Vector3 direction = (Earth.position - transform.position).normalized;
			GetComponent<Rigidbody>().MovePosition(transform.position + direction * speed * Time.deltaTime);*/
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, Earth, step);
		}

		if (force != Vector3.zero) {
			//Debug.Log ("Force Changed");
			gameObject.GetComponent<Rigidbody> ().AddForce (force, ForceMode.Impulse);
		}
	}

	public void AddForce(Vector3 newforce){
		//Debug.Log ("Got new Force");
		Debug.Log (newforce);
		force += newforce;
	}

	void OnDestroy(){
		if (!exiting && Camera.main.GetComponent<GameManager> ().level == Camera.main.GetComponent<GameManager> ().LastLevel) {
			Camera.main.GetComponent<GameManager> ().RemoveAsteroidInPlay ();
		} else if (!exiting) {
			Camera.main.GetComponent<GameManager> ().RemoveAsteroid (gameObject);
		}
	}

	void OnApplicationQuit(){
		exiting = true;
	}
}
