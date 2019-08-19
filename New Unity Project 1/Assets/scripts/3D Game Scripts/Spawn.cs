using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

	public bool enable;
	public float SpawnTime;
	public float FirstEnemyTime;
	public List<GameObject> asteroids;
	public Transform[] spawnPoints;
	private int nextSpawnPoint;

	void Start () {
		nextSpawnPoint = 0;

		if (enable) {
			InvokeRepeating ("SpawnAsteroid", FirstEnemyTime, SpawnTime);
		}
	}

	void SpawnAsteroid(){
		
		SpawnAsteroid (true, spawnPoints[nextSpawnPoint]);

		if (nextSpawnPoint == spawnPoints.Length - 1) {
			nextSpawnPoint = 0;
		} else {
			nextSpawnPoint++;
		}
	}

	public GameObject SpawnAsteroid(bool big, Transform spawnPoint){
		GameObject asteroid = asteroids [Random.Range (0, asteroids.Count)];
		asteroid.GetComponent<AsteroidManager> ().isBig = big;
		GameObject asteroid1 = Instantiate (asteroid, spawnPoint.position, spawnPoint.rotation);

		return asteroid1;
	}
}
