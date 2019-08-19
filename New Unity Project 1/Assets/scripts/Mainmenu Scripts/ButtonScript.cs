using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ButtonScript : MonoBehaviour {
	public string FileName = "HighScore.dat";
	public AudioMixer mixer;
	private HighScore HighScoreFile = null;
	public VideoClip Load1;
	public VideoClip Load2;
	public GameObject main;
	public GameObject options;
	public GameObject highscore;
	public GameObject tutorial;
	public GameObject LoadScreen;
	public GameObject LoadScreen2;
	public GameObject OptionsBackButton;
	public GameObject HighScoreBackButton;
	public GameObject TutorialBackButton;
	public GameObject[] Scores;
	public Text MaxScore;
	public Button Difficulty;
	//public Button Hard;
	public Sprite NormalImage;
	public Sprite HardImage;
	public Sprite[] SoundSelections;
	public Sprite[] HighlightSoundSelections;
	public AudioSource MenuAudio;
	public AudioSource MenuMusic;
	public AudioClip MouseOverAudio;
	public AudioClip MouseClickClip;
	public AudioClip LaserSample;
	//private GameObject FadeObject;
	public GameObject fadeTexture;
	private static bool FadeIn = false;
	private static bool FadeOut = false;

	Func<bool> FOF = () => FadeOut == false;
	Func<bool> FIF = () => FadeIn == false;

	void Start(){
		Time.timeScale = 1;
		ApplicationValues.GameMixer = mixer;
		HighScoreFile = HighScore.LoadData (FileName);
		if (HighScoreFile.score (0) < 40000) {
			HighScoreFile.Add(new char[]{'d','f','t'}, 40000);
			HighScore.SaveData (FileName, HighScoreFile);
		}
		//StartCoroutine (BeginningLoad ());
	}
	void Update () {
		if (FadeIn) {
			FadeThisIn ();
		} else if (FadeOut) {
			FadeThisOut ();
		}
	}

	public void LoadDone(){
		StartCoroutine (BeginningLoad ());
	}
	private IEnumerator BeginningLoad(){
		//yield return new WaitForSeconds ((float)Load1.length);
		StartCoroutine (Fade (main, LoadScreen));
		yield return new WaitUntil (FIF);
		GetComponentInParent<VideoPlayer> ().Play ();
		MenuMusic.Play ();

		//LoadScreen.SetActive (false);
		//yield return new WaitForSeconds (0.1f);
		//main.SetActive (true);

	}

	public void StartGame(){
		main.SetActive (false);
		LoadScreen2.SetActive (true);
		HighScoreFile = HighScore.LoadData (FileName);
		ApplicationValues.FirstPlay = false;
		ApplicationValues.HighScore = HighScoreFile.score (0);
		ApplicationValues.Part = 1;
		ApplicationValues.FreeContinue = 0;
		ApplicationValues.Score = 0;
		ApplicationValues.EarthHealth = 10;
		ApplicationValues.FileName = FileName;
		//SceneManager.LoadScene ("hud");
		StartCoroutine(LoadSceenScreen());
	}
	private IEnumerator LoadSceenScreen(){
		yield return new WaitForSeconds (1);
		AsyncOperation async = SceneManager.LoadSceneAsync ("hud");
		/*while (!async.isDone) {
			yield return null;
		}*/
	}

	public void Quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}

	public void MouseOverPlay(){
		MenuAudio.PlayOneShot (MouseOverAudio);
	}
	public void MouseClickPlay(){
		MenuAudio.PlayOneShot (MouseClickClip);
	}
	public void LaserSamplePlay(){
		MenuAudio.PlayOneShot (LaserSample);
	}

	void FadeThisIn(){
		Color co = fadeTexture.GetComponent<Image> ().color;
		co.a = Mathf.Min (co.a + 0.04f, 1);
		fadeTexture.GetComponent<Image> ().color = co;
		/*
		Renderer[] r = FadeObject.GetComponentsInChildren<Renderer>();
		Image[] im = FadeObject.GetComponentsInChildren<Image> ();
		Text[] te = FadeObject.GetComponentsInChildren<Text> ();
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = Mathf.Min (co [i].a + 0.005f, 255);
			r [i].material.SetColor ("_Color", co [i]);
		}
		for (int i = r.Length; i < r.Length + im.Length; i++) {
			co [i] = im [i-r.Length].color;
			co [i].a = Mathf.Min (co [i].a + 0.005f, 255);
			im [i - r.Length].color = co [i];
		}
		for (int i = r.Length+im.Length; i < r.Length + im.Length+te.Length; i++) {
			co [i] = te [i - r.Length - im.Length].color;
			co [i].a = Mathf.Min (co [i].a + 0.005f, 255);
			te [i - r.Length-im.Length].color = co [i];
		}*/
		if (co.a >= 1) {
			FadeIn = false;
		}
	}
	void FadeThisOut(){
		Color co = fadeTexture.GetComponent<Image> ().color;
		co.a = Mathf.Max (co.a - 0.04f, 0);
		fadeTexture.GetComponent<Image> ().color = co;
		/*
		Renderer[] r = FadeObject.GetComponentsInChildren<Renderer>();
		Image[] im = FadeObject.GetComponentsInChildren<Image> ();
		Text[] te = FadeObject.GetComponentsInChildren<Text> ();
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = Mathf.Max (co [i].a - 0.005f, 0);
			r [i].material.SetColor ("_Color", co [i]);
		}
		for (int i = r.Length; i < r.Length + im.Length; i++) {
			co [i] = im [i-r.Length].color;
			co [i].a = Mathf.Max (co [i].a - 0.005f, 0);
			im [i - r.Length].color = co [i];
		}
		for (int i = r.Length+im.Length; i < r.Length + im.Length+te.Length; i++) {
			co [i] = te[i - r.Length - im.Length].color;
			co [i].a = Mathf.Max (co [i].a - 0.005f, 0);
			te [i - r.Length-im.Length].color = co [i];
		}*/
		if (co.a <= 0) {
			FadeOut = false;
		}
	}
	private void FadeFirst(){
		/*float fadevalue;
		if (FadeIn == true) {
			fadevalue = 0;
		} else {
			fadevalue = 1;
		}
		Renderer[] r = FadeObject.GetComponentsInChildren<Renderer>();
		Image[] im = FadeObject.GetComponentsInChildren<Image> ();
		Text[] te = FadeObject.GetComponentsInChildren<Text> ();
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = fadevalue;
			r [i].material.SetColor ("_Color", co [i]);
		}
		for (int i = r.Length; i < r.Length + im.Length; i++) {
			co [i] = im [i-r.Length].color;
			co [i].a = fadevalue;
			im [i - r.Length].color = co [i];
		}
		for (int i = r.Length+im.Length; i < r.Length + im.Length+te.Length; i++) {
			co [i] = te[i - r.Length - im.Length].color;
			co [i].a = fadevalue;
			te [i - r.Length-im.Length].color = co [i];
		}*/
	}
	private IEnumerator Fade(GameObject activate, GameObject deactivate){
		FadeIn = true;
		yield return new WaitUntil (FIF);
		activate.SetActive (true);
		deactivate.SetActive (false);
		FadeOut = true;
		yield return new WaitUntil (FOF);
		/*
		if (main.activeSelf == true) {
			activate.SetActive (true);
			FadeObject = activate;
			FadeIn = true;
			FadeFirst ();
			yield return new WaitUntil (FIF);
		} else {
			activate.SetActive (true);
			FadeObject = deactivate;
			FadeOut = true;
			//FadeFirst ();
			yield return new WaitUntil (FOF);
		}
		deactivate.SetActive (false);*/
	}
		
	public void Options(){
		/*main.SetActive (false);
		options.SetActive (true);*/
		//EventSystem.current.SetSelectedGameObject (OptionsBackButton);
		if (ApplicationValues.isHard) {
			Difficulty.GetComponent<Image> ().sprite = HardImage;
		} else {
			Difficulty.GetComponent<Image> ().sprite = NormalImage;
		}
		StartCoroutine (Fade (options, main));
	}

	public void HighScoreMenu(){
		//main.SetActive (false);

		HighScoreFile = HighScore.LoadData (FileName);

		for (int i = 0; i < 6; i++) {
			Text[] hi = Scores [i].GetComponentsInChildren<Text> (true);
			char[] init = HighScoreFile.GetInitials (i);
			hi [1].text = init [0].ToString () + init [1].ToString () + init [2].ToString ();
			hi [2].text = HighScoreFile.score (i).ToString ();
		}
		MaxScore.text = HighScoreFile.score (0).ToString ();

		//highscore.SetActive (true);
		//EventSystem.current.SetSelectedGameObject (HighScoreBackButton);
		StartCoroutine (Fade (highscore, main));
	}

	public void TutorialMenu(){
		//main.SetActive (false);
		//tutorial.SetActive (true);
		StartCoroutine (Fade (tutorial, main));
	}

	public void OptionsBack(){
		/*main.SetActive (true);
		options.SetActive (false);*/
		//EventSystem.current.SetSelectedGameObject (gameObject);
		StartCoroutine (Fade (main, options));
	}

	public void HighScoreBack(){
		//main.SetActive (true);
		//highscore.SetActive (false);
		//EventSystem.current.SetSelectedGameObject (gameObject);
		StartCoroutine (Fade (main, highscore));
	}

	public void TutorialBack(){
		//main.SetActive (true);
		//tutorial.SetActive (false);
		StartCoroutine (Fade (main, tutorial));
	}

	public void NormalDifficulty(){
		/*Normal.GetComponentInChildren<Image> ().color = Color.red;
		Hard.GetComponentInChildren<Image> ().color = Color.white;
		ApplicationValues.isHard = false;*/
		if (ApplicationValues.isHard) {
			Difficulty.GetComponent<Image> ().sprite = NormalImage;
			//Difficulty.image = NormalImage;
			ApplicationValues.isHard = false;
		} else {
			Difficulty.GetComponent<Image> ().sprite = HardImage;
			//Difficulty.image = HardImage;
			ApplicationValues.isHard = true;
		}
	}

	/*public void HardDifficulty(){
		Hard.GetComponentInChildren<Image> ().color = Color.red;
		//Hard.GetComponent<Text> ().color = Color.white;
		Normal.GetComponentInChildren<Image> ().color = Color.white;
		//Normal.GetComponent<Text> ().color = Color.grey;
		ApplicationValues.isHard = true;
		//Debug.Log (Normal.name);
	}*/
}
