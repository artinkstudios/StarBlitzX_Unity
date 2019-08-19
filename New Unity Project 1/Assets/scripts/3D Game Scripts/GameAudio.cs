using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudio : MonoBehaviour {

	private AudioSource AudioPlayBack;
	public AudioClip MissileAlert;
	public AudioClip Pause;
	public AudioClip UnPause;
	public AudioClip LaserFire;
	public AudioClip EarthDestroyed;

	void Start () {
		AudioPlayBack = GetComponent<AudioSource> ();
	}

	public void PlayAlert(){
		AudioPlayBack.PlayOneShot (MissileAlert);
	}

	public void PlayPause(){
		AudioPlayBack.PlayOneShot (Pause);
	}
	public void PlayUnPause(){
		AudioPlayBack.PlayOneShot (UnPause);
	}

	public void PlayLaserShot(){
		AudioPlayBack.PlayOneShot (LaserFire);
	}

	public void PlayEarthDestroyed(){
		AudioPlayBack.PlayOneShot (EarthDestroyed);
	}
}
