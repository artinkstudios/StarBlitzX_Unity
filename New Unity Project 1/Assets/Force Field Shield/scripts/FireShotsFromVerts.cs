//Copyright (c) 2016 Kyle Halladay

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to 
//deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;


public class FireShotsFromVerts : MonoBehaviour 
{
	public Vector3[] firePoints = null;
	public float frequency = 1.0f;
	private float _cur = 0.0f;
	public Collider projectileTarget;

	public float radius = 0.1f;
	public float intensity = 50.0f;
	public float lifetime = 2.5f;
	void Start () 
	{
		var filter = GetComponent<MeshFilter>();
		if (filter != null)
		{
			var mesh = filter.sharedMesh;
			Vector3[] verts = mesh.vertices;

			firePoints = new Vector3[verts.Length];

			for (int i = 0; i < verts.Length; i++)
			{
				firePoints[i] = transform.TransformPoint(verts[i]);
			}
		}

		if (firePoints != null)
		{
			InvokeRepeating("Fire", frequency, frequency);
		}
	}

	void Fire()
	{
		int source = Random.Range(0, firePoints.Length);

		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.localScale *= 0.25f;
		var proj = obj.AddComponent<ToShieldProjectile>();
		proj.t = 0.0f;
		proj.path.origin = firePoints[source];
		proj.path.direction = (projectileTarget.transform.position-proj.path.origin).normalized;
		proj.active = true;
		proj.intensity = intensity;
		proj.radius = radius;
		proj.lifetime = lifetime;
		obj.transform.position = proj.path.origin;


	}
	
}
