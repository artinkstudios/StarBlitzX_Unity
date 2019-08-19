using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMusic : MonoBehaviour {

	public GameObject MusicNameText;
	private float MusicTextPos;
	private float MovePos;
	private bool MovingText = false;
	private bool iswaiting = false;
	private AudioSource AudioPlayBack;
	public AudioClip LevelClear;
	public AudioClip GameOver;
	public AudioClip[] Music;
	private float PauseTime = 0;

	private int NextClip;

	void Start () {
		MusicTextPos = MusicNameText.transform.position.y;
		MusicNameText.transform.position = new Vector3 (MusicNameText.transform.position.x, MusicTextPos * -2, MusicNameText.transform.position.z);
		PauseTime = 0;
		AudioPlayBack = GetComponent<AudioSource> ();
		NextClip = 0;
		PlayMusicInOrder ();
	}
	void Update (){
		if (MovingText) {
			MoveText ();
		}
		//if at top start scolling
		//MusicNameText.GetComponentInChildren<Text>().text.Substring(Scroll, sizeof);
		//increase scroll while scroll+size<text.length
		//stop when off screen
	}

	private void MoveText(){
		MusicNameText.transform.position = new Vector3 (MusicNameText.transform.position.x, MusicNameText.transform.position.y + MovePos, MusicNameText.transform.position.z);
		if (MusicNameText.transform.position.y >= MusicTextPos) {
			MovingText = false;
			StartCoroutine (WaitToMove ());
		} else if (MusicNameText.transform.position.y <= MusicTextPos * -2) {
			MovingText = false;
		}
	}
	private IEnumerator WaitToMove(){
		iswaiting = true;
		yield return new WaitForSeconds (2);
		iswaiting = false;
		MoveMusicTitle ();
	}
	public void MoveMusicTitle(){
		MovingText = true;
		if (MusicNameText.transform.position.y >= MusicTextPos && iswaiting) {
			MovingText = false;
			StopAllCoroutines ();
			StartCoroutine (WaitToMove ());
		} else if (MusicNameText.transform.position.y >= MusicTextPos) {
			MovePos = -2;
		} else {
			MovePos = 2;
		}
	}

	public void PausePlayback(){
		PauseTime = AudioPlayBack.time;
		if (IsInvoking ()) {
			CancelInvoke ();
		}
		AudioPlayBack.Stop ();
	}
	public void ResumePlayback(){
		AudioPlayBack.Stop ();
		PlayMusicInOrder ();
	}
	
	public void PlayClear(){
		PausePlayback ();
		AudioPlayBack.clip = LevelClear;
		AudioPlayBack.loop = true;
		AudioPlayBack.time = 0;
		AudioPlayBack.Play ();
	}

	public void Stop(){
		AudioPlayBack.Stop ();
	}

	public void PlayGameOver(){
		PausePlayback ();
		AudioPlayBack.clip = GameOver;
		AudioPlayBack.loop = true;
		AudioPlayBack.time = 0;
		AudioPlayBack.Play ();
	}

	public void PlayMusicInOrder(){
		//MusicNameText.GetComponentInChildren<Text> ().text.Substring (0, 12);
		MusicNameText.GetComponentInChildren<Text>().text = Music [NextClip].name;
		AudioPlayBack.clip = Music [NextClip];
		AudioPlayBack.loop = true;
		AudioPlayBack.time = PauseTime;
		AudioPlayBack.Play ();
		//Invoke ("PlayMusicInOrder", Music [NextClip].length - PauseTime);

		PauseTime = 0;
		/*NextClip++;
		if (NextClip >= Music.Length) {
			NextClip = 0;
		}*/
		//MoveMusicTitle ();
	}


	public void RotateMusic(){
		if (IsInvoking ()) {
			CancelInvoke ();
		}
		NextClip++;
		if (NextClip >= Music.Length) {
			NextClip = 0;
		}
		PlayMusicInOrder ();

		//MoveMusicTitle ();
	}
}
