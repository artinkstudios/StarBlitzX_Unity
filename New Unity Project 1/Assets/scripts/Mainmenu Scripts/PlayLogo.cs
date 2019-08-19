using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayLogo : MonoBehaviour {

	public GameObject MainCanvas;

	void Start () {
		Invoke ("AfterVideo", (float)GetComponent<VideoPlayer> ().clip.length);
	}
	
	private void AfterVideo(){
		MainCanvas.SetActive (true);
	}
}
