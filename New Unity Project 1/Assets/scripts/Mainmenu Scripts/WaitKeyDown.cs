using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WaitKeyDown : MonoBehaviour {

	public VideoClip clip;
	public ButtonScript Game;

	// Use this for initialization
	void Start () {
		clip = GetComponent<VideoPlayer> ().clip;
		StartCoroutine (VideoTime ());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			StopCoroutine (VideoTime ());
			SendMessage ();
		}
	}

	private IEnumerator VideoTime(){
		yield return new WaitForSeconds ((float)clip.length);
		SendMessage ();
	}
	private void SendMessage(){
		Game.LoadDone ();
	}
}
