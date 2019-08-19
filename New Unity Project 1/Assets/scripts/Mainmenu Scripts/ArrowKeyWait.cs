using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ArrowKeyWait : MonoBehaviour {
	private bool isSelected = false;
	public string audioname;
	public int currentValue;
	public Image Parent;
	public Sprite[] unselected;
	public Sprite[] selected;
	public AudioClip LaserSample;
	public AudioSource sample;

	// Use this for initialization
	void Start () {
		currentValue = 9;
		//changelater
		float temp;
		ApplicationValues.GameMixer.GetFloat(audioname, out temp);
		currentValue = (int)((temp+50) / (50.0f / 9));
		//Debug.Log("recieved sound: "+currentValue);
		GetComponent<Image> ().sprite = unselected [currentValue];
	}
	
	// Update is called once per frame
	void Update () {
		if (isSelected && Input.GetButtonDown ("Horizontal")) {
			//Debug.Log ((1/Time.timeScale)* (22* Input.GetAxis ("Horizontal")));
			//Debug.Log("moving sound");
			currentValue += (int) ((1/Time.timeScale)* (22* Input.GetAxis ("Horizontal")));

			if (currentValue < 0) {
				currentValue = 0;
			} else if (currentValue > 9) {
				currentValue = 9;
			}
			GetComponent<Image> ().sprite = selected [currentValue];
			float volume = ((50.0f / 9) * currentValue) - 50;
			ApplicationValues.GameMixer.SetFloat (audioname, volume);
			if (audioname.CompareTo ("SFXVolume") == 0) {
				//Camera.main.GetComponent<ButtonScript> ().LaserSamplePlay ();
				sample.PlayOneShot(LaserSample);
			}
		}
	}

	public void Change(int value){
		currentValue = value;
		GetComponent<Image> ().sprite = unselected [currentValue];
	}

	public void Selected(){
		isSelected = true;
		GetComponent<Image> ().sprite = selected [currentValue];
		Parent.rectTransform.localScale = new Vector3 (1.05f, 1.05f, 1.05f);
	}
	public void notSelected(){
		isSelected = false;
		GetComponent<Image> ().sprite = unselected [currentValue];
		Parent.rectTransform.localScale = new Vector3 (1, 1, 1);
	}
}
