using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject brownasteroidPrefab;
    public int brownasteroidDensity;
    public int seed;
    public float innerRadius;
    public float outerRadius;
    public float height;
    public bool rotatingClockwise;

    [Header("Asteroid Settings")]
    public float minOrbitSpeed;
    public float maxOrbitSpeed;
    public float minRotationSpeed;
    public float maxRotationSpeed;

    private Vector3 localPosition;
    private Vector3 worldOffset;
    private Vector3 worldPosition;
	private int[,] rotationPool;
	private int rotationPoolOffset = 0;
    private float randomRadius;
    private float randomRadian;
    private float x;
    private float y;
    private float z;

    //================================================
    // Random Point on a Circle given only the Angle.
    // x = cx + r * cos(a)
    // y = cy + r* sin(a)
    //================================================
    void Start()
    {
        Random.InitState(seed);
		Debug.Assert (brownasteroidDensity > 0);
		rotationPool = new int[Mathf.CeilToInt (Mathf.Sqrt (brownasteroidDensity)),3];
		//Debug.Log (Mathf.CeilToInt (Mathf.Sqrt (brownasteroidDensity)));
		//Debug.Log (rotationPool.GetLength(0));
		InitRotationPool ();

        for (int i = 0; i < brownasteroidDensity; i++)
        {
            do
            {
                randomRadius = Random.Range(innerRadius, outerRadius);
                randomRadian = Random.Range(0, (2 * Mathf.PI));

                y = Random.Range(-(height /2), (height / 2));
                x = randomRadius * Mathf.Cos(randomRadian);
                z = randomRadius * Mathf.Sin(randomRadian);
				if (float.IsNaN(z) || float.IsNaN(x)){
					Debug.Log("Returned a NAN:\nX = " + x + "\nZ = " + z + "\n");
				}
            }
            while (float.IsNaN(z) && float.IsNaN(x));

            localPosition = new Vector3(x, y, z);
            worldOffset = transform.rotation * localPosition;
            worldPosition = transform.position + worldOffset;

			GameObject _brownasteroid = Instantiate(brownasteroidPrefab, worldPosition, Quaternion.Euler(rotationPool[rotationPoolOffset,0], rotationPool[rotationPoolOffset,1], rotationPool[rotationPoolOffset,2]));
            _brownasteroid.AddComponent<BeltObject>().SetupBeltObject(Random.Range(minOrbitSpeed, maxOrbitSpeed), Random.Range(minRotationSpeed, maxRotationSpeed), gameObject, rotatingClockwise);
            _brownasteroid.transform.SetParent(transform);

			if (rotationPoolOffset == rotationPool.GetLength(0) - 1) {
				rotationPoolOffset = -1;
			}
			rotationPoolOffset++;
        }
    }

	private void InitRotationPool(){
		for (int i = 0; i < rotationPool.GetLength(0); i++) {
			for (int j = 0; j < 3; j++) {
				rotationPool [i,j] = Random.Range (0, 360);
			}
		}
	}
}  
                    
