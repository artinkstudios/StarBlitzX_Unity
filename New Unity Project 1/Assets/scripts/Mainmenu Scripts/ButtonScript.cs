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
	public AudioMixer mixer;
	private HighScore HighScoreFile = null;
	public VideoClip Load1;
	public VideoClip Load2;
	public GameObject main;
	public GameObject options;
	public GameObject credits;
	public GameObject highscore;
    public GameObject HighscoreEnter;
    public Text CurrentEnterScore;
    public InputField InitialsEnter;
	public GameObject tutorial;
	public GameObject LoadScreen;
	public GameObject LoadScreen2;
	public GameObject OptionsBackButton;
    public GameObject DifficultyHighlight;
	public GameObject HighScoreBackButton;
	public GameObject TutorialBackButton;
    public GameObject CreditsBackButton;
	public GameObject[] Scores;
	public Text MaxScore;
	public Button Normal;
	public Button Hard;
	public Sprite NormalRegular;
	public Sprite HardRegular;
	public Sprite NormalHighlight;
	public Sprite HardHighlight;
	public AudioSource MenuAudio;
	public AudioSource MenuMusic;
	public AudioClip MouseOverAudio;
	public AudioClip MouseClickClip;
	public AudioClip LaserSample;
	//private GameObject FadeObject;
	public GameObject fadeTexture;
    private EventSystem ThisEvent;
	private static bool FadeIn = false;
	private static bool FadeOut = false;
    
	Func<bool> FOF = () => FadeOut == false;
	Func<bool> FIF = () => FadeIn == false;

	void Start(){
        if (!OptionsSettings.InAcceptableResolutions(Screen.currentResolution))
        {
            Resolution te = OptionsSettings.GetHighestResolution();
            Screen.SetResolution(te.width, te.height, Screen.fullScreen);
        }

        ThisEvent = EventSystem.current;
		Time.timeScale = 1;
		ApplicationValues.GameMixer = mixer;
		HighScoreFile = HighScore.LoadData (ApplicationValues.FileName);
		if (HighScoreFile.score (0) < 40000) {
			HighScoreFile.Add(new char[]{'d','f','t'}, 40000);
			HighScore.SaveData (ApplicationValues.FileName, HighScoreFile);
		}
		ApplicationValues.ScoreFile = HighScoreFile;
		
        if (ApplicationValues.ScoreFile.isHighScore(ApplicationValues.Score))
        {
            StartCoroutine(BeginningLoad());
            StartCoroutine(FadeThis(HighscoreEnter, main, null));
            CurrentEnterScore.text = ApplicationValues.Score.ToString();
        }
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
        ThisEvent.SetSelectedGameObject(null);
		StartCoroutine (FadeThis (main, LoadScreen, ThisEvent.firstSelectedGameObject));
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
		HighScoreFile = HighScore.LoadData (ApplicationValues.FileName);
		ApplicationValues.FirstPlay = false;
		ApplicationValues.HighScore = HighScoreFile.score (0);
		ApplicationValues.Part = 1;
		ApplicationValues.FreeContinue = 0;
		ApplicationValues.Score = 0;
		ApplicationValues.EarthHealth = 10;
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
	private IEnumerator FadeThis(GameObject activate, GameObject deactivate, GameObject selected){
		FadeIn = true;
		yield return new WaitUntil (FIF);
		activate.SetActive (true);
		deactivate.SetActive (false);
        ThisEvent.SetSelectedGameObject(selected);
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
		//arrowkeywait send sound volumes
		if (ApplicationValues.isHard) {
			Normal.GetComponent<Image> ().sprite = NormalRegular;
			Hard.GetComponent<Image> ().sprite = HardHighlight;
		} else {
			Normal.GetComponent<Image> ().sprite = NormalHighlight;
			Hard.GetComponent<Image> ().sprite = HardRegular;
		}
        //EventSystem.current.SetSelectedGameObject (OptionsBackButton);
        StartCoroutine(FadeThis (options, main, DifficultyHighlight));
	}

    public void SubmitHighScore()
    {
        char[] init = (InitialsEnter.text + "   ").ToCharArray();
        for (int i = 0; i < 3; i++)
        {
            if (!char.IsLetterOrDigit(init[i]))
            {
                init[i] = ' ';
            }
        }
        ApplicationValues.ScoreFile.Add(init, ApplicationValues.Score);
        HighScore.SaveData(ApplicationValues.FileName, ApplicationValues.ScoreFile);

        for (int i = 0; i < 8; i++)
        {
            Text[] hi = Scores[i].GetComponentsInChildren<Text>(true);
            char[] init2 = ApplicationValues.ScoreFile.GetInitials(i);
            hi[1].text = init2[0].ToString() + init2[1].ToString() + init2[2].ToString();
            hi[2].text = ApplicationValues.ScoreFile.score(i).ToString();
        }
        StartCoroutine(FadeThis(highscore, HighscoreEnter, HighScoreBackButton));
    }

	public void HighScoreMenu(){
		//main.SetActive (false);

		HighScoreFile = HighScore.LoadData (ApplicationValues.FileName);

		for (int i = 0; i < 8; i++) {
			Text[] hi = Scores [i].GetComponentsInChildren<Text> (true);
			char[] init = HighScoreFile.GetInitials (i);
			hi [1].text = init [0].ToString () + init [1].ToString () + init [2].ToString ();
			hi [2].text = HighScoreFile.score (i).ToString ();
		}
		//MaxScore.text = HighScoreFile.score (0).ToString ();

		//highscore.SetActive (true);
		//EventSystem.current.SetSelectedGameObject (HighScoreBackButton);
		StartCoroutine (FadeThis (highscore, main, HighScoreBackButton));
	}

	public void TutorialMenu(){
        //main.SetActive (false);
        //tutorial.SetActive (true);
        //EventSystem.current.SetSelectedGameObject (TutorialBackButton);
        StartCoroutine(FadeThis (tutorial, main, TutorialBackButton));
	}

	public void CreditsMenu(){
        //EventSystem.current.SetSelectedGameObject (CreditsBackButton);
        StartCoroutine(FadeThis (credits, main, CreditsBackButton));
	}

	public void OptionsBack(){
		/*main.SetActive (true);
		options.SetActive (false);*/
		//EventSystem.current.SetSelectedGameObject (gameObject);
		StartCoroutine (FadeThis (main, options, ThisEvent.firstSelectedGameObject));
	}

	public void HighScoreBack(){
		//main.SetActive (true);
		//highscore.SetActive (false);
		//EventSystem.current.SetSelectedGameObject (gameObject);
		StartCoroutine (FadeThis (main, highscore, ThisEvent.firstSelectedGameObject));
	}

	public void TutorialBack(){
		//main.SetActive (true);
		//tutorial.SetActive (false);
		StartCoroutine (FadeThis (main, tutorial, ThisEvent.firstSelectedGameObject));
	}

	public void CreditsBack(){
		StartCoroutine (FadeThis (main, credits, ThisEvent.firstSelectedGameObject));
	}

	public void NormalDifficulty(){
		if (ApplicationValues.isHard) {
			Normal.GetComponent<Image> ().sprite = NormalHighlight;
			Hard.GetComponent<Image> ().sprite = HardRegular;
			ApplicationValues.isHard = false;
		} /*else {
			Difficulty.GetComponent<Image> ().sprite = HardImage;
			//Difficulty.image = HardImage;
			ApplicationValues.isHard = true;
		}*/
	}

	public void HardDifficulty(){
		if (!ApplicationValues.isHard) {
			Normal.GetComponent<Image> ().sprite = NormalRegular;
			Hard.GetComponent<Image> ().sprite = HardHighlight;
			ApplicationValues.isHard = true;
		} 
	}
    
}
