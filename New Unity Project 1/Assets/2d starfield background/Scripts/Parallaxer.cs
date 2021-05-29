using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour 
{
	public float ParallaxFactor = 0;

	Transform 		theCamera;
	Vector3				theStartPosition;

	void Start ()
	{
		theCamera = Camera.main.transform;
		theStartPosition = transform.position;
	}
	
	void Update ()
	{
		Vector3 newPos = theCamera.position * ParallaxFactor;						// Caculate the position of the object
		newPos.z = 0;																										// Force Z-axis to zero, since we're in 2D
		newPos.x += theStartPosition.x;
		newPos.y += theStartPosition.y;
		transform.position = newPos;
	}
}
