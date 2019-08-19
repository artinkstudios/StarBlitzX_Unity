using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BonusSpawn : MonoBehaviour {

	public GameObject Background;
	public GameObject Enemy;

	public float TimeBetweenEnemies = 1;
	private float TimeWaveStarted;
	public Transform[] spawnPoints;
	private int EnemiesInPlay;
	private int Wave;
	public int DebugWave = 0;
	//public float DebugTime = 1;

	void Start () {
		Wave = 1;
		EnemiesInPlay = 0;

		if (DebugWave > 0) {
			Wave = DebugWave;
		}
		//Time.timeScale = DebugTime;

		StartCoroutine (StartNextWave ());
	}

	// Update is called once per frame
	void Update () {
		if (IsInvoking ("SpawnEnemy") && ((Wave < 5 && Time.time >= TimeWaveStarted + (TimeBetweenEnemies * 10.5f)) || (Wave >= 5 && Time.time >= TimeWaveStarted + (TimeBetweenEnemies * 5.5f)))) {
			CancelInvoke ("SpawnEnemy");
			Wave++;
		} 
	}


	private IEnumerator StartNextWave(){
		yield return new WaitForSeconds (3);
		Camera.main.GetComponent<BonusGameManager> ().ResetHitEnemies ();
		TimeWaveStarted = Time.time;
		InvokeRepeating ("SpawnEnemy", TimeBetweenEnemies, TimeBetweenEnemies);
	}

	void SpawnEnemy(){
		GameObject enemy = Enemy;
		Transform start;
		Transform end;
		BonusEnemyMovement move = enemy.GetComponent<BonusEnemyMovement> ();
		move.Part1 = true;
		move.LastWave = false;
		move.OutsideLine = false;

		if (Wave == 1) {
			start = spawnPoints [1];
			end = spawnPoints [0];
			move.ClockwiseCircle = false;
		} else if (Wave == 2) {
			start = spawnPoints [0];
			end = spawnPoints [1];
			move.ClockwiseCircle = true;
		} else if (Wave == 3) {
			start = spawnPoints [3];
			end = spawnPoints [0];
			move.ClockwiseCircle = false;
		} else if (Wave == 4) {
			start = spawnPoints [2];
			end = spawnPoints [1];
			move.ClockwiseCircle = true;
		} else if (Wave == 5) {
			start = spawnPoints [4];
			end = start;
			move.EndPosition = start;
			move.OutsideLine = true;
			move.ClockwiseCircle = true;
			move.LastWave = true;
			GameObject e = Instantiate (enemy, start.position+(Vector3.right* (enemy.GetComponent<Image> ().rectTransform.rect.width/2)), start.rotation, Background.transform);
			e.GetComponent<Image> ().transform.localPosition = start.localPosition+(Vector3.right* (enemy.GetComponent<Image> ().rectTransform.rect.width/2));
			e.transform.SetAsFirstSibling ();
		} else {
			start = null;
			end = null;
			Debug.LogError ("Passed over last wave");
			Debug.Break ();
		}

		move.EndPosition = end;
		move.OutsideLine = false;
		if (Wave < 5) {
			GameObject e = Instantiate (enemy, start.position, start.rotation, Background.transform);
			e.GetComponent<Image> ().transform.localPosition = start.localPosition;
			e.transform.SetAsFirstSibling ();
		} else {
			move.ClockwiseCircle = false;
			EnemiesInPlay++;
			Camera.main.GetComponent<BonusGameManager> ().TotalEnemiesSpawned ++;
			GameObject e = Instantiate (enemy, start.position+(Vector3.left* (enemy.GetComponent<Image> ().rectTransform.rect.width/2)), start.rotation, Background.transform);
			e.GetComponent<Image> ().transform.localPosition = start.localPosition+(Vector3.left* (enemy.GetComponent<Image> ().rectTransform.rect.width/2));
			e.transform.SetAsFirstSibling();
		}

		EnemiesInPlay++;
		Camera.main.GetComponent<BonusGameManager> ().TotalEnemiesSpawned ++;
	}

	public void EnemyKilled(){
		EnemiesInPlay--;
		if (EnemiesInPlay < 0) {
			Debug.Log ("Issue with enemy count");
		}
		if (EnemiesInPlay == 0 && Wave < 6) {
			StartCoroutine (StartNextWave ());
		} else if (EnemiesInPlay == 0) {
			//load next thing
			Camera.main.GetComponent<BonusGameManager> ().GameOver ();
		}
	}
}
