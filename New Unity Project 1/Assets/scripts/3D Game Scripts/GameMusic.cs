using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMusic : MonoBehaviour {

	public GameObject MusicNameText;
    public Sprite[] MusicTitle;
	//private float MusicTextPos;
	//private float MovePos;
	private bool MovingText = false;
	private bool iswaiting = false;
	private AudioSource AudioPlayBack;
	public AudioClip LevelClear;
	public AudioClip GameOver;
	public AudioClip[] Music;
	private float PauseTime = 0;

	private int NextClip;

	void Start () {
		//MusicTextPos = MusicNameText.transform.position.y;
		//MusicNameText.transform.position = new Vector3 (MusicNameText.transform.position.x, MusicTextPos * -2, MusicNameText.transform.position.z);
		PauseTime = 0;
		AudioPlayBack = GetComponent<AudioSource> ();
		NextClip = 0;
		PlayMusicInOrder ();
	}
	void Update (){
		if (MovingText) {
			MoveText ();
		}
	}

	private void MoveText(){
        //MusicNameText.transform.position = new Vector3 (MusicNameText.transform.position.x, MusicNameText.transform.position.y + MovePos, MusicNameText.transform.position.z);
        Color co = MusicNameText.GetComponent<Image>().color;
        co.a = Mathf.Max(co.a - 0.02f, 0);
        MusicNameText.GetComponent<Image>().color = co;


        if (co.a <= 0)
        {
            MovingText = false;
        }
	}
	private IEnumerator WaitToMove(){
		iswaiting = true;
		yield return new WaitForSeconds (3);
		iswaiting = false;
        MovingText = true;
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
        MusicNameText.GetComponent<Image>().sprite = MusicTitle[NextClip];
		AudioPlayBack.clip = Music [NextClip];
		AudioPlayBack.loop = true;
		AudioPlayBack.time = PauseTime;
		AudioPlayBack.Play ();
		//Invoke ("PlayMusicInOrder", Music [NextClip].length - PauseTime);

		PauseTime = 0;
        Color co = MusicNameText.GetComponent<Image>().color;
        co.a = 1;
        MusicNameText.GetComponent<Image>().color = co;
        StartCoroutine(WaitToMove());
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
	}
}
