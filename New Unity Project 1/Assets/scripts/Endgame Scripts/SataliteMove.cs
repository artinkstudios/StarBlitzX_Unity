using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SataliteMove : MonoBehaviour {

	public float speed = 3;
	private bool moving = false;
	public Transform EndPosition;
	public Text EndText;
	private bool FadeIn = false;

	void Start () {
		
	}

	void Update () {
		if (moving) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, EndPosition.position, step);

			if (transform.position == EndPosition.position) {
				moving = false;
				FadeIn = true;
			}
		} else if (FadeIn && EndText.color.a < 255){
			Color TextColor = EndText.color;
			TextColor.a += 0.01f;
			EndText.color = TextColor;
		}
	}

	public void StartMoving(){
		moving = true;
	}

	public void MainMenu(){
		if (EndText.color.a >= 255) {
			SceneManager.LoadScene ("MainMenu");
		}
	}
}
