using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletControl : MonoBehaviour {

	public float Speed = 1000;

	void Start () {
		
	}

	void Update () {
		transform.position = new Vector3 (transform.position.x, transform.position.y + Speed * Time.deltaTime, transform.position.z); 
		if (transform.position.y >= transform.parent.GetComponent<Image> ().rectTransform.rect.height) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.GetComponent<BonusEnemyMovement>() != null){
			Camera.main.GetComponent<BonusGameManager> ().HitEnemy (other.transform);
			Destroy (other.gameObject);
			Destroy (gameObject);
		}
	}
}
