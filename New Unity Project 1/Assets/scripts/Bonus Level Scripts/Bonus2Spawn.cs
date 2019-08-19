using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Bonus2Spawn : MonoBehaviour {

	public GameObject Background;
	public GameObject Enemy;

	public float TimeBetweenEnemies = 1;
	private float TimeWaveStarted;
	public Transform[] spawnPoints;
	private int EnemiesInPlay;
	private int Wave;
	public int DebugWave = 0;
	private bool FirstWaveDone = false;

	void Start () {
		Wave = 1;
		EnemiesInPlay = 0;
		FirstWaveDone = false;

		if (DebugWave > 0) {
			Wave = DebugWave;
		}

		StartCoroutine (FirstWave ());
	}

	// Update is called once per frame
	void Update () {
		if (IsInvoking ("SpawnEnemy") && Time.time >= TimeWaveStarted + (TimeBetweenEnemies * 10.5f)) {
			CancelInvoke ("SpawnEnemy");
		}

	}


	private IEnumerator StartNextWave(){
		//Debug.Log ("Start Wave");
		Wave++;
		yield return new WaitForSeconds (3);
		Camera.main.GetComponent<BonusGameManager> ().ResetHitEnemies ();
		TimeWaveStarted = Time.time;
		InvokeRepeating ("SpawnEnemy", TimeBetweenEnemies, TimeBetweenEnemies);
	}

	void SpawnEnemy(){
		GameObject enemy = Enemy;
		Transform start;
		BonusEnemyMovement move = enemy.GetComponent<BonusEnemyMovement> ();

		if (Wave == 2) {
			start = spawnPoints [5];
			move.EndPosition = spawnPoints [2];
			move.ClockwiseCircle = false;
		} else if (Wave == 3) {
			start = spawnPoints [0];
			move.EndPosition = spawnPoints [7];
			move.ClockwiseCircle = true;
		} else {
			start = null;
			Debug.LogError ("Passed over last wave");
			Debug.Break ();
		}
			
		move.LastWave = false;
		move.Part1 = false;
		EnemiesInPlay ++;
		Camera.main.GetComponent<BonusGameManager> ().TotalEnemiesSpawned ++;
		GameObject e = Instantiate (enemy, start.position, start.rotation, Background.transform);
		e.transform.SetAsFirstSibling();
	}

	private IEnumerator FirstWave(){
		yield return new WaitForSeconds (5);

		SpawnFirstWaveEnemy (spawnPoints [3], spawnPoints [8]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [3], spawnPoints [8]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [3], spawnPoints [8]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [3], spawnPoints [8]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [3], spawnPoints [8]);
		yield return new WaitForSeconds (TimeBetweenEnemies);

		SpawnFirstWaveEnemy (spawnPoints [4], spawnPoints [9]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [4], spawnPoints [9]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [4], spawnPoints [9]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [4], spawnPoints [9]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [4], spawnPoints [9]);
		yield return new WaitForSeconds (TimeBetweenEnemies);

		SpawnFirstWaveEnemy (spawnPoints [1], spawnPoints [6]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [1], spawnPoints [6]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [1], spawnPoints [6]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [1], spawnPoints [6]);
		yield return new WaitForSeconds (TimeBetweenEnemies);
		SpawnFirstWaveEnemy (spawnPoints [1], spawnPoints [6]);

		FirstWaveDone = true;
	}

	private void SpawnFirstWaveEnemy(Transform start, Transform end){
		GameObject enemy = Enemy;
		BonusEnemyMovement move = enemy.GetComponent<BonusEnemyMovement> ();
		move.EndPosition = end;
		move.LastWave = true;
		move.Part1 = false;
		GameObject e = Instantiate (enemy, start.position, start.rotation, Background.transform);
		e.transform.SetAsFirstSibling();
		EnemiesInPlay++;
		Camera.main.GetComponent<BonusGameManager> ().TotalEnemiesSpawned ++;
	}

	public void EnemyKilled(){
		//Debug.Log ("Enemy Killed");
		EnemiesInPlay--;
		if (EnemiesInPlay < 0) {
			Debug.Log ("Issue with enemy count");
		}
		if (FirstWaveDone && EnemiesInPlay == 0 && Wave < 3) {
			StartCoroutine (StartNextWave ());
		} else if (FirstWaveDone && EnemiesInPlay == 0) {
			//load next thing
			Camera.main.GetComponent<BonusGameManager>().GameOver();
		}
	}
}