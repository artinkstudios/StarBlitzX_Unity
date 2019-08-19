using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WebGLVideo : MonoBehaviour {

	public bool wait = false;
	public VideoClip clip;
	//public AudioSource audio;
	private VideoPlayer player;

	// Use this for initialization
	void Start () {
		player = gameObject.GetComponent<VideoPlayer> ();
		//Debug.Log (clip.name);
		if (System.IO.File.Exists (Application.streamingAssetsPath+"/"+ clip.name + ".mp4")) {
		//if (true) {
			player.url = System.IO.Path.Combine (Application.streamingAssetsPath, clip.name + ".mp4");
			//player.url = "https://lang5050.itch.io/star-blitz-files/"+ clip.name + ".mp4";
			player.Prepare ();

			if (wait) {
				StartCoroutine (WaitTime ());
			} else {
				player.Play ();
			}
			Debug.Log ("now playing "+clip.name);
		} else {
			Debug.Log ("issue getting video");
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator WaitTime(){
		yield return new WaitForSeconds (3);
		player.Play ();
	}
}
