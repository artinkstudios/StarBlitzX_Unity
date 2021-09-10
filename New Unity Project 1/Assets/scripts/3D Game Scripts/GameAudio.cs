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
    public AudioClip IronAsteroidPing;
    public AudioClip FireHit;
    public AudioClip LowAccuracy;
    public AudioClip MedAccuracy;
    public AudioClip HighAccuracy;
    public AudioClip AnnouncerA;
    public AudioClip AnnouncerB;
    private AudioClip CurrentAnnouncer;

	void Start () {
		AudioPlayBack = GetComponent<AudioSource> ();
        CurrentAnnouncer = AnnouncerA;
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

    public void PlayIronPing()
    {
        AudioPlayBack.PlayOneShot(IronAsteroidPing);
    }

    public void PlayHitFire()
    {
        AudioPlayBack.PlayOneShot(FireHit);
    }

    public void PlayLowAccuracy()
    {
        AudioPlayBack.PlayOneShot(LowAccuracy);
    }
    public void PlayMedAccuracy()
    {
        AudioPlayBack.PlayOneShot(MedAccuracy);
    }
    public void PlayHighAccuracy()
    {
        AudioPlayBack.PlayOneShot(HighAccuracy);
    }
    public void StopPlayback()
    {
        AudioPlayBack.Stop();
    }

    public void PlayAnnouncerMessage()
    {
        AudioPlayBack.PlayOneShot(CurrentAnnouncer);
    }
    public void SwitchAnnouncer()
    {
        if (CurrentAnnouncer.Equals(AnnouncerA))
        {
            CurrentAnnouncer = AnnouncerB;
        } else
        {
            CurrentAnnouncer = AnnouncerA;
        }
    }
}
