using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BonusGameManager : MonoBehaviour {

	public GameObject Canvas;
	public GameObject EndScreen;
	public GameObject PauseMenu;
	public GameObject EndScore;
	public GameObject ScoreText;
	public GameObject HighScoreText;
	public GameObject ShipsText;
	public GameObject PerfectText;
	public Button ExitAfterPlay;
	private int DefaultScoreSize;

	private float keyboardx;
	private float keyboardy;
	private float KeyboardSpeed = 5;
	private float screenWidth;
	private float screenHeight;

	public Image Player;
	//public Image Lazer;
	public GameObject Bullet;
	public GameObject Background;
	public GameObject Explosion;
	public GameObject PerfectWaveScore;
	public AudioSource LaserFire;
	public AudioClip LaserS;
	public AudioClip PauseS;
	public AudioClip UnPauseS;

	private float PlayerWidth;
	private float PlayerHeight;
	private bool paused = false;
	public int TotalEnemiesSpawned = 0;
	private bool PerfectRun = false;
	private int EnemiesHitThisWave = 0;
	private uint BonusScore = 0;
	private static bool FadeIn = false;
	private static bool FadeOut = false;

	void Start () {
		keyboardx = Player.transform.localPosition.x;
		keyboardy = Player.transform.localPosition.y;
		//Debug.Log (keyboardx + ", " + keyboardy);

		screenWidth = Canvas.GetComponent<RectTransform> ().rect.width;
		screenHeight = Canvas.GetComponent<RectTransform> ().rect.height;
		PlayerWidth = Player.rectTransform.rect.width;
		PlayerHeight = Player.rectTransform.rect.height;
		DefaultScoreSize = ScoreText.GetComponent<Text> ().fontSize;
		ScoreText.GetComponent<Text> ().text = ApplicationValues.Score.ToString();
		HighScoreText.GetComponent<Text> ().text = ApplicationValues.HighScore.ToString ();
		TotalEnemiesSpawned = 0;
		PerfectRun = false;
		BonusScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		keyboardx += Input.GetAxis ("Horizontal") * KeyboardSpeed;
		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
			Player.GetComponent<Animator> ().Play ("God-Hammer-Animation-Moving");
		}

		if (keyboardx < 0-(screenWidth/2)+(PlayerWidth/2)) {
			keyboardx = 0-(screenWidth/2)+(PlayerWidth/2);
		} else if (keyboardx > (screenWidth/2)-(PlayerWidth/2)) {
			keyboardx = (screenWidth/2)-(PlayerWidth/2);
		}
		/*if (keyboardy < 0-(Screen.height/2)) {
			keyboardy = 0-(Screen.height/2);
		} else if (keyboardy > (Screen.height/2)-PlayerHeight) {
			keyboardy = (Screen.height/2)-PlayerHeight;
		}*/
		Player.transform.localPosition = new Vector3 (keyboardx, keyboardy, 0);

		if (ScoreText.GetComponent<Text> ().fontSize > DefaultScoreSize) {
			ScoreText.GetComponent<Text> ().fontSize -= 1;
		}
		if (FadeIn) {
			FadeInText ();
		} else if (FadeOut) {
			FadeOutText ();
		}


		//next is fire
		//play at top of player
		//keyboardx+ playerwidth/2, keyboardy+playerheight
		if (Input.GetButtonDown ("Pause") || Input.GetButtonDown("Cancel")) {
			//pause game
			Pause();
		}
		if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) {
			Fire ();
		}
	}


	private void Fire (){
		GameObject b = Instantiate (Bullet, Player.transform.position, Player.transform.rotation, Background.transform);
		b.GetComponent<Image> ().transform.localPosition = new Vector3(keyboardx, keyboardy + PlayerHeight, 0);
		LaserFire.Play ();
	}
	public void HitEnemy(Transform t){
		AddScore (500);
		EnemiesHitThisWave++;
		GameObject explode = Instantiate (Explosion, t.position, t.rotation, Background.transform);
		Destroy (explode, explode.GetComponent<ParticleSystem> ().main.duration);
		if (EnemiesHitThisWave >= 10) {
			Instantiate(PerfectWaveScore, t.position, Quaternion.identity, Background.transform);
		}
	}
	public void ResetHitEnemies(){
		EnemiesHitThisWave = 0;
	}

	private IEnumerator Shot(){
		//play fire sound
		LaserFire.Play();
		yield return new WaitForSeconds (0.07f);
	}

	void AddScore(uint addedscore){
		ApplicationValues.Score += addedscore;
		BonusScore += addedscore;
		ScoreText.GetComponent<Text> ().text = ApplicationValues.Score.ToString();
		ScoreText.GetComponent<Text> ().fontSize = DefaultScoreSize + 5;
	}
	void FadeInText(){
		Renderer[] r = EndScreen.GetComponentsInChildren<Renderer>(true);
		Image[] im = EndScreen.GetComponentsInChildren<Image> (true);
		Text[] te = EndScreen.GetComponentsInChildren<Text> (true);
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = Mathf.Min (co [i].a + 0.01f, 255);
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
		}
		if (co[0].a == 255) {
			FadeIn = false;
		}
	}
	void FadeOutText(){
		Renderer[] r = Background.GetComponentsInChildren<Renderer>(true);
		Image[] im = Background.GetComponentsInChildren<Image> (true);
		Text[] te = Background.GetComponentsInChildren<Text> (true);
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = Mathf.Max (co [i].a - 0.01f, 0);
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
		}
		if (co[0].a == 0) {
			FadeOut = false;
			Background.SetActive (false);
		}
	}

	public void LoadNextLevel(){
		ApplicationValues.Part++;
		//load level
		SceneManager.LoadScene ("hud");
	}

	public void MainMenu(){
		SceneManager.LoadScene ("MainMenu");
		//Application.Quit();
	}

	public void Pause(){
		if (paused) {
			Time.timeScale = 1;
			LaserFire.PlayOneShot (UnPauseS);
			paused = false;
			Background.GetComponent<AudioSource> ().Play ();
			PauseMenu.SetActive (false);
		} else {
			Time.timeScale = 0;
			LaserFire.PlayOneShot (PauseS);
			paused = true;
			Background.GetComponent<AudioSource> ().Pause ();
			PauseMenu.SetActive (true);
		}
	}

	Func<bool> FOF = () => FadeOut == false;
	Func<bool> FIF = () => FadeIn == false;
	private IEnumerator FadeEnd(){
		yield return new WaitForSeconds(2);
		ShipsText.SetActive (true);
		yield return new WaitForSeconds(3);
		if (PerfectRun) {
			PerfectText.SetActive (true);
			yield return new WaitForSeconds (3);
		}
		FadeOut = true;
		yield return new WaitUntil (FOF);
		Renderer[] r = EndScreen.GetComponentsInChildren<Renderer>();
		Image[] im = EndScreen.GetComponentsInChildren<Image> ();
		Text[] te = EndScreen.GetComponentsInChildren<Text> (true);
		Color[] co = new Color[r.Length+im.Length+te.Length];
		for (int i = 0; i < r.Length; i++) {
			co [i] = r [i].material.color;
			co [i].a = 0;
			r [i].material.SetColor ("_Color", co [i]);
		}
		for (int i = r.Length; i < r.Length+im.Length; i++) {
			co [i] = im [i-r.Length].color;
			co [i].a = 0;
			im [i - r.Length].color = co [i];
		}
		for (int i = r.Length+im.Length; i < r.Length + im.Length+te.Length; i++) {
			co [i] = te[i - r.Length - im.Length].color;
			co [i].a = 0;
			te [i - r.Length-im.Length].color = co [i];
		}
		EndScreen.SetActive (true);
		FadeIn = true;
		yield return new WaitUntil (FIF);
		UnityEngine.Cursor.visible = true;
	}

	public void GameOver(){

		if (BonusScore / 500 == TotalEnemiesSpawned) {
			PerfectRun = true;
		}
		if (!ApplicationValues.demo && PerfectRun) {
			PerfectText.SetActive (true);
			ApplicationValues.FreeContinue++;
		}
		if (ApplicationValues.demo) {
			EndScore.GetComponent<Text>().text = "Your Score: "+ApplicationValues.Score;
			ShipsText.GetComponent<Text> ().text = "Ships Destroyed: "+(BonusScore/500);
			ExitAfterPlay.GetComponent< Text> ().text = "Main Menu";
			ExitAfterPlay.onClick.AddListener (MainMenu);
		} else {
			EndScore.GetComponent<Text>().text = "Bonus Score Earned: "+BonusScore;
			ShipsText.GetComponent<Text> ().text = "Ships Destroyed: " + (BonusScore / 500);
			ExitAfterPlay.GetComponent<Text> ().text = "Continue";
			ExitAfterPlay.onClick.AddListener (LoadNextLevel);
		}
		StartCoroutine (FadeEnd ());
	}
}
