using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour 
{
	public Vector3 Rot;

	void Start () 
	{
		
	}

	void Update () 
	{
		transform.Rotate (Rot.x, Rot.y, Rot.z);
	}
}
