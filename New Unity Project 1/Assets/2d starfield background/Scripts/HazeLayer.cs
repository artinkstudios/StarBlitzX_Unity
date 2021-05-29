using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazeLayer : MonoBehaviour
{
	public float	ParallaxFactor = 0f;

	Transform 		theCamera;
	Vector3				theDimension;

	Vector3				theStartPosition;

	void Start () 
	{
		theCamera = Camera.main.transform;
		theStartPosition = transform.position;

		theDimension = GetComponent<Renderer>().bounds.size;
	}

	void Update ()
	{
		Vector3 newPos = theCamera.position * ParallaxFactor;						// Calculate the position of the object
		newPos.z = 0;																										// Force Z-axis to zero, since we're in 2D
		newPos.x += theStartPosition.x;
		newPos.y += theStartPosition.y;
		transform.position = newPos;

		EndlessRepeater ();
	}

	void EndlessRepeater ()
	{
		if ( theCamera.position.y > ( transform.position.y + theDimension.y ) )
		{
			theStartPosition.y += theDimension.y + theDimension.y;
		}
	}
}
