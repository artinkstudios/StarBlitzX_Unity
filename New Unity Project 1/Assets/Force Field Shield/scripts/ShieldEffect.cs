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
using System.Collections.Generic;

public interface IHittable
{
	void Add(Vector3 impactPosition, float maxIntensity, float radius, float maxLifeTime, float speed);
}

public class ShieldEffect : MonoBehaviour, IHittable
{
    public List<ShieldEffectNode> nodes = new List<ShieldEffectNode>();
    public MeshRenderer meshR;

	Matrix4x4 _PositionMatrix = Matrix4x4.zero;
	Vector4 _Instensities = Vector4.zero;
	Vector4 _Radius = Vector4.zero;

    [System.Serializable]
    public class ShieldEffectNode
    {
        public Vector3 pos = Vector3.zero;
        public float startIntensity = 2;
        public float minRadius = 1;
        public float maxRadius = 3;

        public float t = 0;
        public float maxLifeTime = 1;
		public float speed;
        public void Step(float deltaT)
        {
            t += deltaT*(speed*0.5f);
        }

        public float intensity
        {
            get
            {
                return Mathf.Lerp(startIntensity, 0, t / maxLifeTime);
            }
        }

        public float radius
        {
            get
            {
                return Mathf.Lerp(minRadius, maxRadius, t / maxLifeTime);
            }
        }

        public bool free
        {
            get
            {
                return t >= maxLifeTime;
            }
        }
    }

    void Update()
    {
		Vector3 cam = Camera.main.transform.position;

		for(int m = 0; m < 4; m++)
		{
	        for (int i = 0; i < 4; i++)
			{
				int n = m*4 + i;

				if (nodes.Count >= n+1)
				{
		            ShieldEffectNode node = nodes[n];
		            if (!node.free)
		            {
		                node.Step(Time.deltaTime);
						_PositionMatrix.SetRow(i,new Vector4(node.pos.x, node.pos.y, node.pos.z, 1.0f));
						_Radius[i] = node.radius;
						_Instensities[i] = node.intensity;
		            }
					else
					{
						nodes.RemoveAt(i);
						_PositionMatrix.SetRow(i, new Vector4(cam.x, cam.y, cam.z, 1.0f));
						_Radius[i] = 0.0f;

						_Instensities[i] = 0.0f;
						i--;
					}
				}
				else
				{
					_PositionMatrix.SetRow(i, new Vector4(cam.x, cam.y, cam.z, 1.0f));
					_Instensities[i] = 0.0f;
					_Radius[i] = 0.0f;	
				}

				meshR.material.SetMatrix("_Positions"+m, _PositionMatrix);
				meshR.material.SetVector("_Intensities"+m, _Instensities);
				meshR.material.SetVector("_Radius"+m, _Radius);

			}
		}

		transform.rotation = Quaternion.identity;
    }

    public void Add(Vector3 impactPosition, float maxIntensity, float radius, float maxLifeTime, float speed)
    {
        ShieldEffectNode node = null;
        if (nodes.Count < 16)
        {
            node = new ShieldEffectNode();
            nodes.Add(node);
            
        }
        else
        {
            node = nodes.Find(x => x.free == true);
        }

        if (node != null)
        {
			node.pos = transform.InverseTransformPoint(impactPosition);

            node.maxRadius = radius;
			node.speed = speed;
            node.minRadius = 0.0f;

            node.t = 0;
			node.maxLifeTime = maxLifeTime;

            node.startIntensity = maxIntensity;
        }
    }
}
