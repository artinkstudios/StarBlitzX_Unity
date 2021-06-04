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

public class ToShieldProjectile : MonoBehaviour 
{
	public float t;
	public float endT;
	public Ray path;
	public bool active;
	public Vector3 hitPoint;
	Rigidbody rigidBody;

	public float intensity = 50.0f;
	public float radius = 0.1f;
	public float lifetime = 2.5f;

	public float shotSpeed = 3.0f;

	void Start()
	{
		shotSpeed = Random.Range(3.0f, 6.0f);
		var trail = gameObject.AddComponent<TrailRenderer>();
		trail.startWidth = 0.05f;
		trail.endWidth = 0.05f;
		trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
		trail.sharedMaterial.color = Color.white;
		gameObject.GetComponent<MeshRenderer>().enabled = false;

	}

	void OnCollisionEnter(Collision collision)
	{
		var effect = collision.collider.gameObject.GetComponent<IHittable>();
		if (effect != null)
		{
			if (collision.contacts.Length > 0)
			{
				effect.Add((collision.contacts[0].point),intensity, radius, lifetime, shotSpeed);
			}
			else 
			{
				effect.Add(transform.position, 2.0f, 2.0f, 2.0f, shotSpeed);
			}

			GameObject.Destroy (this.gameObject);

		}
	}

	void Update()
	{
		t += Time.deltaTime;
		transform.position = path.origin + path.direction * t * shotSpeed;
	}
}
