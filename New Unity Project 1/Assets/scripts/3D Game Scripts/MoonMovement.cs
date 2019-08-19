using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMovement : MonoBehaviour {

	public Transform EarthPosition;
	public float speed = 20;
	private Vector3 point;
	private Vector3 v;

	// Use this for initialization
	void Start () {
		EarthPosition = transform.parent;
		point = EarthPosition.position;
		v = transform.position - point;
	}
	
	// Update is called once per frame
	void Update () {
		//v = Quaternion.AngleAxis(Time.deltaTime * speed, (new Vector3(1,-1,0))) * v;
		//transform.position = point + v;
	}

	public void RotatingRight(float amount){
		v = Quaternion.AngleAxis (0.005f * amount * Vector3.Angle (point, transform.position), Vector3.down) * v;
		transform.position = point + v;
	}

	public void RotatingLeft(float amount){
		v = Quaternion.AngleAxis (0.005f * amount * Vector3.Angle (point, transform.position), Vector3.up) * v;
		transform.position = point + v;
	}

}
