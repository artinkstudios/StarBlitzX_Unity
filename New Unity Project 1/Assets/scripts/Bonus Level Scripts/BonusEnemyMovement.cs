using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusEnemyMovement : MonoBehaviour {

	//rotating, 45 = right, -45 = left

	public Transform EndPosition;
	//public bool StartAtTop = true;
	public bool ClockwiseCircle = true;
	public bool OutsideLine = false;
	public bool LastWave = false;
	public bool Part1 = true;
	private int stage;	//several stages for movement
	public float CircleSpeed = 1.5f;
	public float TravelSpeed = 10;
	public float CircleRadius = 20;
	private float CircleStartTime;
	private float CircleTimeOffset;
	private Vector2 offset;
	private bool exiting = false;

	void Start () {
		exiting = false;
		offset = new Vector2(0,0);
		CircleTimeOffset = 0;
		stage = 1;
		RotateEnemy (new Vector3 (offset.x, offset.y + CircleRadius, 0));

		if (!Part1 && LastWave) {
			stage = 3;
			RotateEnemy (EndPosition.localPosition);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (stage == 1) {
			Vector3 destination;
			destination = new Vector3 (offset.x, offset.y + CircleRadius, 0);
			if (LastWave && OutsideLine) {
				destination += (Vector3.right * (GetComponent<Image> ().rectTransform.rect.width));
			} else if (LastWave) {
				destination += (Vector3.left * (GetComponent<Image> ().rectTransform.rect.width));
			}
			//RotateEnemy (destination);
			MoveToPosition (destination);
			if (transform.localPosition == destination) {
				stage++;
				CircleStartTime = Time.time;
				if (LastWave) {
					CircleTimeOffset -= 0.05f;
				}
				CircleStartTime += CircleTimeOffset;
			}
		} else if (stage == 2) {
			if (ClockwiseCircle) {
				ClockwiseFromTop ();
			} else {
				CounterClockwiseFromTop ();
			}

			if ((Time.time - CircleStartTime + CircleTimeOffset) * CircleSpeed >= 4 * Mathf.PI) {
				stage++;
				RotateEnemy (EndPosition.position);
			}
		} else if (stage == 3) {
			Vector3 destination;
			if (LastWave) {
				if (OutsideLine) {
					destination = new Vector3 (EndPosition.localPosition.x + (GetComponent<Image> ().rectTransform.rect.width), EndPosition.localPosition.y, EndPosition.localPosition.z);
				} else {
					destination = new Vector3 (EndPosition.localPosition.x - (GetComponent<Image> ().rectTransform.rect.width), EndPosition.localPosition.y, EndPosition.localPosition.z);
				}
			} else {
				destination = EndPosition.localPosition;
			}
			//RotateEnemy (destination);
			MoveToPosition (destination);
			if (transform.localPosition == destination) {
				Destroy (gameObject);
			}
		}
	}

	void OnDestroy(){
		if (!exiting && Part1) {
			Camera.main.GetComponent<BonusSpawn> ().EnemyKilled ();
		} else if (!exiting) {
			Camera.main.GetComponent<Bonus2Spawn> ().EnemyKilled ();
		}
	}
	void OnApplicationQuit(){
		exiting = true;
	}


	void MoveToPosition(Vector3 destination){
		float step = TravelSpeed * Time.deltaTime;
		transform.localPosition = Vector3.MoveTowards (transform.localPosition, destination, step);
	}

	void ClockwiseFromTop(){
		float RotateZ = (((Time.time - CircleStartTime) * CircleSpeed) * (180 / Mathf.PI))*-1 + 90 - transform.eulerAngles.z;
		transform.Rotate(0,0,RotateZ);
		transform.localPosition = new Vector3(
			(CircleRadius * Mathf.Sin((Time.time-CircleStartTime)*CircleSpeed))+offset.x,
			(CircleRadius * Mathf.Cos((Time.time-CircleStartTime)*CircleSpeed))+offset.y, 0);
	}

	void CounterClockwiseFromTop(){
		float RotateZ = (((Time.time - CircleStartTime) * CircleSpeed) * (180 / Mathf.PI)) - 90 - transform.eulerAngles.z;
		transform.Rotate (0, 0, RotateZ);
		transform.localPosition = new Vector3(
			(CircleRadius * -Mathf.Sin((Time.time-CircleStartTime)*CircleSpeed))+offset.x,
			(CircleRadius * Mathf.Cos((Time.time-CircleStartTime)*CircleSpeed))+offset.y, 0);
	}

	void RotateEnemy(Vector3 destination){
		//z = 90 if direction x = +, y = 0
		Vector3 direction = destination - transform.position;
		/*if (stage == 3 && !LastWave && destination.x > 0) {
			direction += (Vector3.right * 500);
		} else if (stage == 3 && !LastWave && destination.x < 0) {
			direction += (Vector3.left * 500);
		}*/
		if (Part1 && LastWave && OutsideLine) { //now normally face right
			transform.Rotate (0, 0, (Mathf.Atan2 (direction.y, direction.x) * (180 / Mathf.PI)) - transform.eulerAngles.z + 90);
		} else {  //normally face down
			transform.Rotate (0, 0, (Mathf.Atan2 (direction.x, -direction.y) * (180 / Mathf.PI)) - transform.eulerAngles.z);
		}
	}
}
