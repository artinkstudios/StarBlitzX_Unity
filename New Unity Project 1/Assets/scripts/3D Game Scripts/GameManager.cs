using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	//timer variables
	public GameObject TimerText;
	public GameObject MessageText;
	public GameObject PressButtonText;
	public GameObject ScoreText;
	public GameObject HighScoreText;
	public GameObject PauseMenu;
	public GameObject GameOverMenu;
	public GameObject GameOverContinue;
	public GameObject GameEnd;
	public GameObject HighScoreMenu;
	public InputField Initials;
	public float TimeBetweenLevels;
	public bool WaitBetween = false;
	public GameAudio SFXForGame;
	public GameMusic MusicForGame;


	//spawn variables
	public float SpawnTime;
	public float LevelSpawnRate;
	public Transform[] spawnPoints;
	public Transform[] MissileSpawnPoints;
	public GameObject[] MissileIndicators;
	public GameObject[] BrownAsteroids;
	public GameObject[] GreyAsteroids;
	public GameObject[] ElectroAsteroids;
	public GameObject[] PhantomAsteroids;
	public GameObject Missile;
	public GameObject Earth;
	public GameObject EarthExplosion;
	public GameObject LargeExplosion;
	public GameObject SmallExplosion;
	//public MoonMovement Moon;
	//public GameObject PhantomExplosion;
	public bool SpawnEnabled;
	public float SpawnForce;

	//cursor display variables
	public Texture2D CursorTexture = null;
	public Texture2D ReverseCursorTexture = null;
	public Texture2D AutoLockCursorTexture = null;
	private bool mouseControl = false;
	public float NormalKeyboardSpeed = 5;
	public LineRenderer laser1;
	public LineRenderer laser2;
	public GameObject VLaser1;
	public GameObject VLaser2;
	public GameObject LeftPointer;
	public GameObject RightPointer;
	public float LaserRange;


	public int level = 1;
	public int LastLevel = 20;
	public int DebugLevel = 0;

	private List<GameObject> AsteroidsOnScreen;
	private Texture2D DisplayCursorTexture = null;
	private int nextSpawnPoint;
	private float GameTime;
	private float SpecialTime = 0;
	private bool LevelStarted = false;
	private bool paused = false;
	private float FirstEnemyTime = 0.5f;
	private float Score;
	private int ScoreSize;
	private int BigScoreSize;
	private float keyboardx;
	private float keyboardy;
	private float KeyboardSpeed;
	private bool inverted = false;
	private bool autoLocking = false;
	private GameObject CurrentlyFollowingAsteroid;
	private int AsteroidsInPlay;
	private int LevelEarthHealth;


	void Start () {
		AsteroidsOnScreen = new List<GameObject>();
		DisplayCursorTexture = CursorTexture;
		UnityEngine.Cursor.visible = false;

		WaitBetween = false;
		nextSpawnPoint = 0;
		AsteroidsInPlay = 0;
		ScoreSize = ScoreText.GetComponent<Text> ().fontSize;
		BigScoreSize = ScoreSize + 5;
		keyboardx = (Screen.width / 2) - 25;
		keyboardy = (Screen.height / 2) - 25;
		KeyboardSpeed = NormalKeyboardSpeed;

		if (DebugLevel >= 51) {
			FinishedGame ();
		} else if (DebugLevel >= 41) {
			ApplicationValues.Part = 3;
		} else if (DebugLevel >= 21) {
			ApplicationValues.Part = 2;
		}
		if (ApplicationValues.Part == 1) {
			level = 1;
			LastLevel = 20;
			Score = 0;
			ApplicationValues.Score = 0;
		} else if (ApplicationValues.Part == 2) {
			level = 21;
			LastLevel = 40;
			Score = ApplicationValues.Score;
		} else {
			level = 41;
			LastLevel = 50;
			Score = ApplicationValues.Score;
		}
		if (DebugLevel > 0) {
			level = DebugLevel;
		}
		ScoreText.GetComponent<Text> ().text = ApplicationValues.Score.ToString ();
		HighScoreText.GetComponent<Text> ().text = ApplicationValues.HighScore.ToString ();

		StartCoroutine(ReadyMessage());
	}


	void Update () {
		//update game clock
		if (LevelStarted) {
			GameTime -= Time.deltaTime;
			TimerText.GetComponent<Text>().text = string.Format("{0}:{1:00}", (int)GameTime / 60, (int)GameTime % 60);
			if (GameTime <= 0) {
				StartCoroutine (StopLevel ());
			}
		}

		if (SpecialTime > 0) {
			SpecialTime -= Time.deltaTime;
			if (SpecialTime <= 0) {
				NormalInput ();
			}
		}
		if (ScoreText.GetComponent<Text> ().fontSize != ScoreSize) {
			ScoreText.GetComponent<Text> ().fontSize -= 1;
		}


		//button input
		if (mouseControl && (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && !paused && !WaitBetween)
		{
			//fire lasers
			Fire(Input.mousePosition);
		}
		if (Input.GetButtonDown ("Pause") || Input.GetButtonDown("Cancel")) {
			//pause game
			Pause();
		}
		if (!mouseControl && !paused && !WaitBetween) {
			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) { 
				CancelAutoLock (); 
				Vector2 newKey = new Vector2 (keyboardx, keyboardy);
				if (inverted) {
					newKey.x -= Input.GetAxis ("Horizontal") * KeyboardSpeed;
					newKey.y += Input.GetAxis ("Vertical") * KeyboardSpeed;
				} else {
					newKey.x += Input.GetAxis ("Horizontal") * KeyboardSpeed;
					newKey.y -= Input.GetAxis ("Vertical") * KeyboardSpeed;
				}

				//temp cursor restrictions.  top 1/3 and bottow 1/3 in middle 1/3, left and right 1/3s 45 to edge
				//screen height / 3 < cursor < screen height * 2/3 when screen width / 3 < cursor < screen width * 2/3
				//when cursor > screen width * 2/3 or 
				//when cursor < screen width / 3, y>width/height (x) && y< ((width/height (x))*-1) + height
				//x=0, y=0, , x=1/3, y=1/3, , x=0, y=max, , x=1/3, y=2/3
				//x=max, y=0, , x=2/3, y=1/3, , x=max, y=max, , x=2/3, y=2/3
				float line = (Screen.height -70) * (newKey.x / (Screen.width - 70));
				if (newKey.x < 0) {
					newKey.x = 0;
				} else if (newKey.x > Screen.width - 70) {
					newKey.x = Screen.width - 70;
				}//hard value of cursor size

				if (newKey.x <= (Screen.width / 4) * 3 && newKey.x >= Screen.height / 4) {
					if (newKey.y < (Screen.height / 4) - 45) {
						newKey.y = (Screen.height / 4) - 45;
					} else if (newKey.y > ((Screen.height / 4) * 3) - 15) {
						newKey.y = ((Screen.height / 4) * 3) - 15;
					}
				} else if (newKey.x < Screen.height / 4) {
					if (newKey.y < (line*1.40f)) {
						newKey.y = (line*1.40f);  //played with numbers to smooth lines
					} else if (newKey.y > ((line * -1.3f) + Screen.height - 70)) {
						newKey.y = ((line * -1.3f) + Screen.height - 70);
					}
				} else {
					if (newKey.y > (line*1.01f)) {
						newKey.y = (line*1.01f);
					} else if (newKey.y < ((line - Screen.height) * -1) - 70) {
						newKey.y = ((line - Screen.height) * -1) - 70;
					}
				}

				/*if (newKey.x > keyboardx) {
				Moon.RotatingRight (Mathf.Abs(newKey.x-keyboardx));
			} else if (newKey.x < keyboardx) {
				Moon.RotatingLeft (Mathf.Abs(keyboardx-newKey.x));
			}*/

				/*if (newKey.y < 0) {							newKey.y = 0;
			} else if (newKey.y > Screen.height - 70) {  newKey.y = Screen.height - 70;  }*/

				keyboardx = newKey.x;
				keyboardy = newKey.y;
				transform.rotation = Quaternion.Euler ((keyboardy - (Screen.height / 2)) / 25, (keyboardx - (Screen.width / 2)) / 25, 0);
				//Pointers 0=center, +-170=max
				//keyx==half screen width, p=0
				//pointerpos = currentx, keyx-half width * (170/half width), currentz
				Vector3 PointPos = LeftPointer.transform.localPosition;
				PointPos.Set (PointPos.x, (keyboardx + 35 - (Screen.width / 2)) * (120.0f / (Screen.width / 2)), PointPos.z);
				LeftPointer.transform.localPosition = PointPos;
				PointPos.Set (RightPointer.transform.localPosition.x, (keyboardx + 35 - (Screen.width / 2)) * (-120.0f / (Screen.width / 2)), PointPos.z);
				RightPointer.transform.localPosition = PointPos;
			}


			if ((Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Jump")) && !paused && !WaitBetween) {
				Fire (new Vector3 (keyboardx + 35, Screen.height - 35 - keyboardy));
			}
		} else if (!paused && !WaitBetween){
			transform.rotation = Quaternion.Euler ((-(Input.mousePosition.y - (Screen.height / 2)) / 10), (Input.mousePosition.x - (Screen.width / 2)) / 10, 0);
		}

		if (Input.GetButtonDown ("Fire3")) {
			MusicForGame.RotateMusic ();
		}
	}


	void OnGUI () {
		//cursor display
		if (mouseControl) {
			float vectorx = Input.mousePosition.x;
			float vectory = Input.mousePosition.y;
			GUI.DrawTexture (new Rect (vectorx - 35, -vectory + Screen.height - 35, 70, 70), DisplayCursorTexture);
		} else {
			GUI.DrawTexture (new Rect (keyboardx, keyboardy, 70, 70), DisplayCursorTexture);
		}
	}

	private void StartLaser(){
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2));
	}

	private void Fire(Vector3 FirePosition){
		//animation for lasers
		Ray ray = Camera.main.ScreenPointToRay (FirePosition);
		LaserAnimation(ray);

		//physics for lasers
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, LaserRange)) {
			//hit something
			GameObject asteroid = hit.collider.gameObject;

			if (asteroid.GetComponent<AsteroidManager> () != null) {
				AsteroidManager asteroidm = asteroid.GetComponent<AsteroidManager> ();
				if (autoLocking && asteroid == CurrentlyFollowingAsteroid) { CancelAutoLock (); }
				if (asteroidm.type.CompareTo ("phantom") == 0) {
					AddScore (300);
					GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
					//RemoveAsteroid (asteroid.transform.parent.gameObject);
					Destroy (asteroid.transform.parent.gameObject);
				} else if (asteroidm.type.CompareTo ("electro") == 0) {
					asteroidm.health--;
					if (asteroidm.health <= 0) {
						AddScore (300);
						GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
						explode.SetActive (true);
						//RemoveAsteroid (asteroid);
						Destroy (asteroid);
						//RevertInvert ();
					}
				} else if (asteroidm.isBig) {
					AddScore (300);
					//need to check asteroid type
					if (asteroidm.type.CompareTo ("brown") == 0) {
						CreateTwo (asteroid);
					} else if (asteroidm.type.CompareTo ("grey") == 0) {
						CreateFour (asteroid);
					}

					GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
					Destroy (asteroid);
				} else {
					AddScore (100);
					GameObject explode = Instantiate (SmallExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
					Destroy (asteroid);
				}
			} else if (asteroid.GetComponent<MissileManager> () != null){
				AddScore (500);
				GameObject explode = Instantiate (SmallExplosion, asteroid.transform.position, asteroid.transform.rotation);
				explode.SetActive (true);
				Destroy (asteroid);
			}
		}
	}
	private void LaserAnimation(Ray ray){
		Vector3 OriginLaser1 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
		Vector3 OriginLaser2 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
		laser1.transform.LookAt (ray.GetPoint (LaserRange));
		Instantiate (VLaser1, laser1.transform.position, laser1.transform.rotation);
		laser2.transform.LookAt (ray.GetPoint (LaserRange));
		Instantiate (VLaser1, laser2.transform.position, laser2.transform.rotation);

		if (Debug.isDebugBuild) {
			Debug.DrawRay (ray.origin, ray.direction * LaserRange, Color.white);
		}
		SFXForGame.PlayLaserShot();
	}
	private IEnumerator Shot(){
		//play fire sound
		SFXForGame.PlayLaserShot();
		yield return new WaitForSeconds (0.07f);
	}


	void SlowInput(){
		KeyboardSpeed = NormalKeyboardSpeed / 2;
		SpecialTime = 5;
	}

	void FastInput(){
		KeyboardSpeed = NormalKeyboardSpeed * 2;
		SpecialTime = 5;
	}

	void InvertInput (){
		if (!inverted) {
			inverted = true;
			StartCoroutine (SwitchCursor ());
		}
		SpecialTime = 15;
	}

	void RevertInvert(){
			inverted = false;
			DisplayCursorTexture = CursorTexture;
	}

	void NormalInputSpeed(){
		KeyboardSpeed = NormalKeyboardSpeed;
	}

	void NormalInput(){
		KeyboardSpeed = NormalKeyboardSpeed;
		inverted = false;
		DisplayCursorTexture = CursorTexture;
	}

	private IEnumerator SwitchCursor(){
		while (inverted && !autoLocking) {
			DisplayCursorTexture = ReverseCursorTexture;
			yield return new WaitForSeconds (0.5f);
			DisplayCursorTexture = CursorTexture;
			yield return new WaitForSeconds (0.5f);
		}
	}

	private Vector3 FollowAsteroid(){
		Vector3 coord = Camera.main.WorldToScreenPoint (CurrentlyFollowingAsteroid.transform.position);
		coord.y = Screen.height - coord.y;
		return coord;
	}
	private Vector3 AutoLockCursor(){
		Vector3[] coords = new Vector3[AsteroidsOnScreen.Count];
		int i = 0;
		foreach (GameObject a in AsteroidsOnScreen) {
			Debug.AssertFormat (a != null, "foreach loop has gone beyond number of asteroids on screen. I = " + i);
			coords [i] = Camera.main.WorldToScreenPoint (a.transform.position);
			coords [i].y = Screen.height - coords [i].y;
			i++;
		}

		//find closest to cursor
		Vector3 closest = coords[0];
		CurrentlyFollowingAsteroid = AsteroidsOnScreen [0];
		for (int j = 1; j < coords.Length; j++) {
			if (Vector3.Distance (new Vector3 (keyboardx, keyboardy), closest) > Vector3.Distance (new Vector3 (keyboardx, keyboardy), coords [j])) {
				closest = coords [j];
				CurrentlyFollowingAsteroid = AsteroidsOnScreen [j];
			}
		}
		return closest;
	}
	private void CancelAutoLock(){
		autoLocking = false;
		DisplayCursorTexture = CursorTexture;
	}

	void AddScore(uint addedscore){
		Score += addedscore;
		ApplicationValues.Score += addedscore;
		ScoreText.GetComponent<Text> ().text = Score.ToString ();
		ScoreText.GetComponent<Text> ().fontSize = BigScoreSize;
	}

	void CreateFour(GameObject asteroid){
		Transform point = asteroid.transform;
		point.position += new Vector3(0, 1.5f, 0);
		GameObject asteroid1 = SpawnAsteroid(false, point, "grey");
		point.position += new Vector3(0, -3, 0);
		GameObject asteroid2 = SpawnAsteroid (false, point, "grey");
		point.position += new Vector3(1.5f, 1.5f, 0);
		GameObject asteroid3 = SpawnAsteroid(false, point, "grey");
		point.position += new Vector3(-3, 0, 0);
		GameObject asteroid4 = SpawnAsteroid(false, point, "grey");

		asteroid1.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid2.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid3.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid4.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
	}

	void CreateTwo(GameObject asteroid){
		//create two asteroids exploding outwards at location of current asteroid
		Transform point = asteroid.transform;
		point.position += new Vector3(0, 3, 0);
		GameObject asteroid1 = SpawnAsteroid(false, point, "brown");
		point.position += new Vector3(0, -3, 0);
		GameObject asteroid2 = SpawnAsteroid (false, point, "brown");

		asteroid1.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, point.position, 10);
		asteroid2.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, point.position, 10);

		//asteroid1.GetComponent<AsteroidManager> ().AddForce (new Vector3(0,1.5f,0));
		//asteroid2.GetComponent<AsteroidManager> ().AddForce (new Vector3(0, -3, 0));
	}
		

	void SpawnAsteroid(){
		//spawn big asteroid at next spawn point
		if (level <= 10) {
			SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
		} else if (level <= 20) {
			if (Random.Range (1, 10) == 1) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "grey");
			} else {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
			}
		} else if (level <= 30) {
			//missles appear
			if (Random.Range (1, 8) == 1) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "grey");
			} else {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
			}
		} else if (level <= 40) {
			int ran = Random.Range (1, 20);
			if (ran == 1) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "electro");
			} else if (ran < 10) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "grey");
			} else {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
			}
		} else if (level <= 50) {
			int ran = Random.Range (1, 10);
			if (ran <= 2) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "electro");
			} else if (ran < 7) {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "grey");
			} else {
				SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
			}
		} else {
			SpawnAsteroid (true, spawnPoints [nextSpawnPoint], "brown");
			//change later
		}

		nextSpawnPoint = Random.Range (0, 4);
		/*if (nextSpawnPoint == spawnPoints.Length - 1) {
			nextSpawnPoint = 0;
		} else {
			nextSpawnPoint++;
		}*/
	}
	void SpawnSideAsteroids(){
		SpawnAsteroid(false, spawnPoints[((int)GameTime%2)+4], "brown");
		Invoke ("SpawnSideAsteroids", SpawnTime * ((int)GameTime%5)+1);
	}

	public GameObject SpawnAsteroid(bool big, Transform spawnPoint, string type){
		//spawn random asteroid at location
		GameObject asteroid;
		if (type.CompareTo ("brown") == 0) {
			asteroid = BrownAsteroids [Random.Range (0, BrownAsteroids.Length)];
			asteroid.GetComponent<AsteroidManager> ().type = "brown";
		} else if (type.CompareTo ("grey") == 0) {
			asteroid = GreyAsteroids [Random.Range (0, GreyAsteroids.Length)];
			asteroid.GetComponent<AsteroidManager> ().type = "grey";
		} else if (type.CompareTo ("electro") == 0) {
			asteroid = ElectroAsteroids [Random.Range (0, ElectroAsteroids.Length)];
			asteroid.GetComponent<AsteroidManager> ().type = "electro";
			asteroid.GetComponent<AsteroidManager> ().health = 3;
		} else if (type.CompareTo ("phantom") == 0) {
			asteroid = PhantomAsteroids [Random.Range (0, PhantomAsteroids.Length)];
			asteroid.GetComponent<AsteroidManager> ().type = "phantom";
		} else {
			asteroid = null;
			Debug.LogError ("Did not get proper asteroid name");
		}

		asteroid.GetComponent<AsteroidManager> ().isBig = big;
		asteroid.GetComponent<AsteroidManager> ().Earth = Earth.transform.position;
		Vector3 DefaultScale = asteroid.transform.localScale;
		if (!big) {
			asteroid.transform.localScale = (asteroid.transform.localScale / 2);		//affects prefab
		}
		if (spawnPoint.position.x > Earth.transform.position.x) {
			asteroid.GetComponent<AsteroidManager> ().Earth += (new Vector3 (10, 0, 0));
		} else {
			asteroid.GetComponent<AsteroidManager> ().Earth += (new Vector3 (-10, 0, 0));
		}
		if (spawnPoint.position.y > Earth.transform.position.y) {
			asteroid.GetComponent<AsteroidManager> ().Earth += (new Vector3 (0, 10, 0));
		} else {
			asteroid.GetComponent<AsteroidManager> ().Earth += (new Vector3 (0, -10, 0));
		}
		GameObject asteroid1 = Instantiate (asteroid, spawnPoint.position, spawnPoint.rotation);
		asteroid.transform.localScale = DefaultScale;

		AddAsteroid (asteroid1);
		return asteroid1;
	}
	private void AddAsteroid(GameObject asteroid){
		AsteroidsOnScreen.Add (asteroid);
	}
	public void RemoveAsteroid(GameObject asteroid){
		if (AsteroidsOnScreen.Count != 0) {
			bool b = AsteroidsOnScreen.Remove (asteroid);
			Debug.Assert (b);
		}
	}
	private void ClearAsteroids(){
		AsteroidsOnScreen.ForEach (Destroy);
		AsteroidsOnScreen.Clear ();
	}

	public void AddAsteroidInPlay(){
		AsteroidsInPlay++;
	}
	public void RemoveAsteroidInPlay(){
		AsteroidsInPlay--;
		if (ApplicationValues.Part == 3 && level == LastLevel && AsteroidsInPlay <= 0) {

		}
	}

	void SpawnMissile(){
		//choose spawn point
		int point = Random.Range(0, 2);
		StartCoroutine (ActivateMissile (point));
	}

	private IEnumerator ActivateMissile(int point){
		SFXForGame.PlayAlert ();

		for (int i = 0; i < 8; i++) {
			MissileIndicators [point].SetActive (true);
			yield return new WaitForSeconds (0.25f);
			MissileIndicators [point].SetActive (false);
			yield return new WaitForSeconds (0.25f);
		}

		GameObject missle = Missile;
		missle.GetComponent<MissileManager> ().Earth = Earth.transform;
		Instantiate (missle, MissileSpawnPoints[point].position, MissileSpawnPoints[point].rotation);
	}

	public void ResetTime(){  //reset game time to 3 minutes
		//GameTime = 180;
		if (level < 6) {
			GameTime = 60;
		} else if (level < 11) {
			GameTime = 90;
		} else if (level < 16) {
			GameTime = 120;
		} else if (level < 21) {
			GameTime = 150;
		} else if (level < 26) {
			GameTime = 180;
		} else if (level < 31) {
			GameTime = 210;
		} else if (level < 36) {
			GameTime = 240;
		} else if (level < 41) {
			GameTime = 270;
		} else if (level < 46) {
			GameTime = 300;
		} else if (level < 51) {
			GameTime = 330;
		} else {
			GameTime = 300;
			Debug.LogError ("level is outside range");
		}
	}

	public void StartLevel(){  //start the game clock
		LevelStarted = true;
		LevelEarthHealth = ApplicationValues.EarthHealth;

		if (SpawnEnabled) {
			InvokeRepeating ("SpawnAsteroid", FirstEnemyTime, SpawnTime-(level*LevelSpawnRate));
		}
		if (SpawnEnabled && level >= 5) {
			Invoke ("SpawnSideAsteroids", SpawnTime * ((int)GameTime%5)+1);
		}
		if (SpawnEnabled && level >= 30 && level < 35) {
			InvokeRepeating ("SpawnMissile", 35, 35);
		} else if (SpawnEnabled && level >= 35 && level < 40) {
			InvokeRepeating ("SpawnMissile", 30, 30);
		} else if (SpawnEnabled && level >= 40 && level < 45) {
			InvokeRepeating ("SpawnMissile", 25, 25);
		} else if (SpawnEnabled && level >= 45 && level < 51) {
			InvokeRepeating ("SpawnMissile", 20, 20);
		}
	}

	private IEnumerator WaitForKeyDown(){
		do {
			yield return null;
		} while (!Input.GetButtonDown("Submit"));
		//} while (!Input.anyKeyDown);
	}

	private IEnumerator StopLevel(){
		//Debug.Log ("hit stop");
		LevelStarted = false;
		if (IsInvoking("SpawnAsteroid")) {
			CancelInvoke ("SpawnAsteroid");
			CancelInvoke ("SpawnSideAsteroids");
		}
		if (IsInvoking ("SpawnMissile")) {
			CancelInvoke ("SpawnMissile");
		}
		yield return new WaitForSeconds (TimeBetweenLevels);

		MusicForGame.PlayClear ();
		MessageText.GetComponent<Text> ().text = "Level Clear";
		MessageText.SetActive (true);
		PressButtonText.SetActive (true);
		WaitBetween = true;
		yield return StartCoroutine (WaitForKeyDown ());
		MessageText.SetActive (false);
		PressButtonText.SetActive (false);
		WaitBetween = false;
		MusicForGame.ResumePlayback ();
		ClearAsteroids ();

		if (level == LastLevel) {
			StartCoroutine (LoadNextLevel ());
		} else {
			level++;
			StartCoroutine (ReadyMessage ());
		}
	}

	private IEnumerator ReadyMessage(){
		//Debug.Log ("Ready");
		ResetTime ();
		yield return new WaitForSeconds (1);
		MessageText.SetActive (true);
		MessageText.GetComponent<Text> ().text = string.Format ("Level {0}", level);
		yield return new WaitForSeconds (2);
		MessageText.GetComponent<Text> ().text = "Ready";
		yield return new WaitForSeconds (1);
		MessageText.GetComponent<Text> ().text = "Go";
		yield return new WaitForSeconds (1);
		MessageText.SetActive (false);
		StartLevel ();
	}

	private IEnumerator LoadNextLevel(){

		yield return new WaitForSeconds (1);
		MessageText.GetComponent<Text> ().text = "Loading Bonus Level";
		MessageText.SetActive (true);
		yield return new WaitForSeconds (2);

		if (ApplicationValues.Part == 1) {
			SceneManager.LoadScene ("Bonus1");
		} else {
			SceneManager.LoadScene ("Bonus2");
		}
	}

	public void Pause(){
		if (paused) {
			SFXForGame.PlayUnPause ();
			Time.timeScale = 1;
			UnityEngine.Cursor.visible = false;
			paused = false;
			PauseMenu.SetActive (false);
		} else {
			SFXForGame.PlayPause ();
			Time.timeScale = 0;
			UnityEngine.Cursor.visible = true;
			paused = true;
			PauseMenu.SetActive (true);
		}
	}

	public void Continue(){
		ApplicationValues.FreeContinue--;
		Time.timeScale = 1;
		UnityEngine.Cursor.visible = false;
		WaitBetween = false;
		GameOverMenu.SetActive (false);
		LevelStarted = false;
		Earth.GetComponentInChildren<EarthManager> ().SetEarthHit (LevelEarthHealth);
		//Earth.GetComponent<EarthManager> ().SetEarthHit (LevelEarthHealth);  //when other earth
		//MusicForGame.ResumePlayback ();
		StartCoroutine (ReadyMessage ());
	}

	public void StartGameOver(){
		StartCoroutine (GameOverAnimation ());
	}
	private IEnumerator GameOverAnimation(){
		Instantiate (EarthExplosion, Earth.transform.position, Earth.transform.rotation);
		Earth.SetActive (false);
		SFXForGame.PlayEarthDestroyed ();
		WaitBetween = true;
		yield return new WaitForSeconds (2);

		GameOver ();
	}

	public void GameOver(){
		Time.timeScale = 0;
		GameOverMenu.SetActive (true);
		UnityEngine.Cursor.visible = true;
		MusicForGame.PlayGameOver ();

		if (ApplicationValues.FreeContinue > 0) {
			GameOverContinue.SetActive (true);
		} else {
			GameOverContinue.SetActive (false);
			StartCoroutine (ReturnToMenu ());
		}
	}
	private IEnumerator ReturnToMenu(){
		yield return new WaitForSecondsRealtime (2);
		Quit ();
	}

	public void FinishedGame(){
		Time.timeScale = 0.00001f;
		WaitBetween = true;
		UnityEngine.Cursor.visible = true;

		GameEnd.SetActive (true);
		/*HighScore hscore = HighScore.LoadData (ApplicationValues.FileName);

		if (hscore.isHighScore (ApplicationValues.Score)) {
			HighScoreMenu.SetActive (true);
		}*/
	}

	public void Quit(){
		AsteroidManager[] asters = GetComponentsInParent<AsteroidManager> ();
		for (int i = 0; i < asters.Length; i++){
			Destroy (asters [i].gameObject);
		}
		if (HighScoreMenu.activeInHierarchy) {
			char[] init = Initials.text.ToCharArray();
			for (int i = 0; i < 3; i++) {
				if (!char.IsLetterOrDigit(init[i])){
					init [i] = ' ';
				}
			}
			HighScore hscore = HighScore.LoadData (ApplicationValues.FileName);
			hscore.Add (init, ApplicationValues.Score);
			HighScore.SaveData (ApplicationValues.FileName, hscore);
		}
		SceneManager.LoadSceneAsync ("MainMenu");
	}
}
