using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour {

    //timer variables
    public Joystick MobileJoystick;
	public GameObject MainHUD;
	public GameObject TimerText;
	public GameObject MessageText;
	public GameObject ReadyVideo;
	public GameObject PressButtonText;
	public GameObject LevelClearText;
    public GameObject BonusPointText;
	public GameObject ScoreText;
	public GameObject HighScoreText;
    public Text FinishedScoreText;
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
	public Animator[] MissileSparkles;
	public GameObject[] BrownAsteroids;
	public GameObject[] GreyAsteroids;
	public GameObject[] ElectroAsteroids;
	public GameObject[] PhantomAsteroids;
    public GameObject[] RedAsteroids;
    public GameObject YellowAsteroids;
	public GameObject FireAsteroid;
    public GameObject TowerrocAsteroid;
    public GameObject RagnarokAsteroid;
    public GameObject ZigzagAsteroid;
    public GameObject IronAsteroid;
    public GameObject Missile;
	public GameObject Earth;
	public GameObject EarthExplosion;
	public GameObject EarthExplosionVideo;
	public GameObject LargeExplosion;
	public GameObject SmallExplosion;
    public GameObject EStar;
	//public MoonMovement Moon;
	//public GameObject PhantomExplosion;
	public bool SpawnEnabled;
	public float SpawnForce;
    private bool YellowAsteroidDeployed;
    private int TowerrocDeployed = 0;
    private int RagnarokDeployed = 0;
    private int IronDeployed = 0;
    private int ZigzagDeployed = 0;


	//cursor display variables
	public Texture2D CursorTexture = null;
	public Texture2D ReverseCursorTexture = null;
	public Texture2D AutoLockCursorTexture = null;
	public float NormalKeyboardSpeed = 5;
	public LineRenderer laser1;
	public LineRenderer laser2;
	public GameObject VLaser1;
	public GameObject VLaser2;
	public GameObject LeftPointer;
	public GameObject RightPointer;
	public float LaserRange;
	public Fade FadeScript;
	public AccuracyDisplay AccuracyScript;
    public GameObject LoadScreenBonus;

    public Image TopLevelImage;
    public Sprite[] TopLevelSprites;
    public GameObject Announcer;
    public Sprite[] LevelImages;
    public Sprite BonusLevelImage;
	public int level = 1;
	public int LastLevel = 20;
    public int DebugLevel;

	private List<GameObject> AsteroidsOnScreen;
	private Texture2D DisplayCursorTexture = null;
	private bool DisplayCursor = true;
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
    private float MouseSpeed;
    private float JoystickSpeed;
	private bool inverted = false;
	private bool autoLocking = false;
	private bool isRotating = false;
	private float rotationfloat;
	private float amountRotated = 0;
	private GameObject CurrentlyFollowingAsteroid;
	private int AsteroidsInPlay;
	private int LevelEarthHealth;
	private int AccuracyVariable1;
	private int AccuracyVariable2;
    private float TimeBetweenShots = 0;
    

	void Start () {
		AsteroidsOnScreen = new List<GameObject>();
		DisplayCursorTexture = CursorTexture;
		DisplayCursor = true;
        Cursor.visible = false;

		WaitBetween = false;
		nextSpawnPoint = 0;
		AsteroidsInPlay = 0;
		ScoreSize = ScoreText.GetComponent<Text> ().fontSize;
		BigScoreSize = ScoreSize + 5;
		keyboardx = (Screen.width / 2);
		keyboardy = (Screen.height / 2);
		KeyboardSpeed = ApplicationValues.KeyboardSpeed;
        MouseSpeed = ApplicationValues.MouseSpeed;
        JoystickSpeed = ApplicationValues.JoystickSpeed;
		ResetAccuracy ();

		if (ApplicationValues.Part == 1) {
			level = 1;
			LastLevel = 10;
			Score = 0;
			ApplicationValues.Score = 0;
		} else if (ApplicationValues.Part == 2) {
			level = 11;
			LastLevel = 20;
			Score = ApplicationValues.Score;
		} else {
			level = 21;
			LastLevel = 50;
			Score = ApplicationValues.Score;
		}
        //level = DebugLevel;
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
        if (TimeBetweenShots > 0)
        {
            TimeBetweenShots -= Time.deltaTime;
        }

		if (isRotating) {
			transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotationfloat, transform.rotation.eulerAngles.z);
			amountRotated += rotationfloat;
			if (Mathf.Abs(amountRotated) >= 360 || Mathf.Abs(amountRotated) == 180 || Mathf.Abs(amountRotated) == 0){
				isRotating = false;
                rotationfloat = 0;
				//Debug.Log (amountRotated);
				if (Mathf.Abs (amountRotated) >= 360) {
					amountRotated = 0;
				}
			}
		}


        //button input
#if UNITY_STANDALONE
        if (Input.GetButton ("Jump") && !paused && !WaitBetween && !isRotating && TimeBetweenShots<=0) {
            //fire lasers
            TimeBetweenShots = 0.25f;
			Fire ();
			
		} else if (Input.GetButton ("Fire1") && ApplicationValues.MouseControl && !paused && !WaitBetween && !isRotating && TimeBetweenShots<=0) {
            TimeBetweenShots = 0.25f;
			Fire ();
		}
		if ((Input.GetButtonDown ("Pause") || Input.GetButtonDown("Cancel"))&& !WaitBetween) {
			Pause();
		}
#endif
        if (!paused && !WaitBetween && isRotating && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && (Mathf.Abs(amountRotated) == 90 || Mathf.Abs(amountRotated) == 270))
        {
            isRotating = false;
        } else if (!paused && !WaitBetween)
        {
            Vector2 newInput;
#if UNITY_STANDALONE
            if (ApplicationValues.MouseControl)
            {
                newInput = new Vector2(Input.GetAxis("Mouse X") * MouseSpeed, Input.GetAxis("Mouse Y") * MouseSpeed);
            } else
            {
                newInput = new Vector2(Input.GetAxis("Horizontal") * KeyboardSpeed, Input.GetAxis("Vertical") * KeyboardSpeed);
            }
            
#endif
#if UNITY_IOS || UNITY_ANDROID
            newInput = new Vector2(MobileJoystick.Horizontal *JoystickSpeed, MobileJoystick.Vertical * JoystickSpeed);
#endif

            if ((newInput.x != 0 || newInput.y != 0) && !isRotating) { 
				//CancelAutoLock (); 
				Vector2 newKey = new Vector2 (keyboardx, keyboardy);
				if (inverted) {
                    newKey.x -= newInput.x;
                    newKey.y += newInput.y;
				} else {
                    newKey.x += newInput.x;
                    newKey.y -= newInput.y;
				}

                newKey = ClampInput(newKey);

				keyboardx = newKey.x;
				keyboardy = newKey.y;

                ScreenPointers();
				
			} else if ((newInput.x != 0 || newInput.y != 0) && (Mathf.Abs(amountRotated) == 90 || Mathf.Abs(amountRotated) == 270))
            {
                isRotating = false;
            }
		}
#if UNITY_STANDALONE
        /*else if (!paused && !WaitBetween && !isRotating){
            Vector2 newInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            newInput = ClampInput(newInput);
            keyboardx = newInput.x;
            keyboardy = -newInput.y + Screen.height;
            ScreenPointers();
        }*/

		if (Input.GetButtonDown ("Fire3") && !WaitBetween) {
			RotateMusic ();
		}
		if (Input.GetButtonDown ("TurnLeft") && !isRotating && !WaitBetween) {
            TurnLeft();
		} else if (Input.GetButtonDown ("TurnRight") && !isRotating && !WaitBetween) {
            TurnRight();
		}
#endif
        if (Input.GetButtonDown("Test")) {
            //StartGameOver ();
            //AddScore(12000);
            //FinishedGame();
            level = 10;
            StartCoroutine (StopLevel ());
            //SpawnMissile();
            //ClearAsteroids();
            
        }
        
	}


	void OnGUI () {
		//cursor display
		/*if (ApplicationValues.MouseControl && DisplayCursor) {
            Vector2 MouseVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            MouseVector = ClampInput(MouseVector);
			GUI.DrawTexture (new Rect (MouseVector.x - 35, -MouseVector.y + Screen.height - 35, 70, 70), DisplayCursorTexture);
		} else*/ if (DisplayCursor){
			GUI.DrawTexture (new Rect (keyboardx-35, keyboardy-35, 70, 70), DisplayCursorTexture);
		}
	}

    private Vector2 ClampInput(Vector2 newInput)
    {
         newInput.x = Mathf.Clamp(newInput.x, 35, Screen.width - 35);
        
        //hard value of cursor size

        if (Mathf.Abs(amountRotated) != 180)
        {
            //h * x / w
            //x * h/w
            //(x-W) * h/w
            //x* h/w - h/w
            //float line = (Screen.height) * (newInput.x / (Screen.width));
            float line = ((float)Screen.height / Screen.width) * 1.4f;

            if (newInput.x <= ((Screen.width / 4) * 3)+135 && newInput.x >= Screen.height / 4)
            {
                newInput.y = Mathf.Clamp(newInput.y, (Screen.height / 4)-35 , ((Screen.height / 4) * 3) +35);
            }
            else if (newInput.x < Screen.height / 4)
            {
                //newInput.y = Mathf.Clamp(newInput.y, (line * 1.4f), ((line * -1.4f) + Screen.height));
                newInput.y = Mathf.Clamp(newInput.y, (line * newInput.x), ((line * -1 * newInput.x) + Screen.height));
            }
            else
            {
                //line - (H/W) + screen H = top right
                //line -1 -(H/W) = bottom right
                //(x-w) * -1 * line * -1 + w
                //x-W  line * -1
                //newInput.y = Mathf.Clamp(newInput.y, ((line * -1.4f) - (Screen.height / Screen.width)), (line * 1.4f) - (Screen.height / Screen.width) + Screen.height);
                newInput.y = Mathf.Clamp(newInput.y, ((newInput.x - Screen.width) * (line * -1)), ((newInput.x - Screen.width) * line) + Screen.height);

                //need to fix
            }
        } else {
            newInput.y = Mathf.Clamp(newInput.y, 35, Screen.height - 35);
        }
        
        return newInput;
    }
    private void ScreenPointers()
    {
        transform.rotation = Quaternion.Euler((keyboardy - (Screen.height / 2)) / 25, (keyboardx - (Screen.width / 2)) / 15 + amountRotated, 0);
        
        Vector3 PointPos = LeftPointer.transform.localPosition;
        PointPos.Set(PointPos.x, (keyboardx - (Screen.width / 2)) * (120.0f / (Screen.width / 2)), PointPos.z);
        LeftPointer.transform.localPosition = PointPos;
        PointPos.Set(RightPointer.transform.localPosition.x, (keyboardx - (Screen.width / 2)) * (-120.0f / (Screen.width / 2)), PointPos.z);
        RightPointer.transform.localPosition = PointPos;
    }

	private void StartLaser(){
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2));
	}

	public void Fire(){
        Vector3 FirePosition;
        FirePosition = new Vector3(keyboardx, Screen.height - keyboardy);
        /*if (ApplicationValues.MouseControl)
        {
            FirePosition = Input.mousePosition;
        } else
        {
            FirePosition = new Vector3(keyboardx, Screen.height - keyboardy);
            //FirePosition = new Vector3(keyboardx+35, Screen.height - 35 - keyboardy);
        }*/
		//animation for lasers
		Ray ray = Camera.main.ScreenPointToRay (FirePosition);
		LaserAnimation(ray);
		AccuracyVariable2++;

		//physics for lasers
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, LaserRange)) {
			//hit something
			GameObject asteroid = hit.collider.gameObject;

			if (asteroid.GetComponent<AsteroidManager> () != null) {
				AccuracyVariable1++;
				AsteroidManager asteroidm = asteroid.GetComponent<AsteroidManager> ();
                CalculateScore(asteroidm.type, asteroidm.isBig);

				if (autoLocking && asteroid == CurrentlyFollowingAsteroid) { CancelAutoLock (); }

				if (asteroidm.type.Contains ("phantom")) {
					//AddScore (300);
					GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
					//RemoveAsteroid (asteroid.transform.parent.gameObject);
					Destroy (asteroid.transform.parent.gameObject);
				} else if (asteroidm.type.Contains ("electro")) {
					asteroidm.health--;
					if (asteroidm.health <= 0) {
						//AddScore (300);
						GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
						explode.SetActive (true);
						//RemoveAsteroid (asteroid);
						Destroy (asteroid);
						//RevertInvert ();
					}
				} else if (asteroidm.type.Contains("yellow"))
                {
                    GameObject explode = Instantiate(LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
                    explode.SetActive(true);
                    Destroy(asteroid);
                    EStar.SetActive(true);
                    EStar.transform.position = asteroid.transform.position;
                } else if (asteroidm.type.Equals("towerroc") || asteroidm.type.Equals("ragnarok"))
                {
                    //AddScore(300);
                    BreakTowerroc(asteroid);
                } else if (asteroidm.type.Equals("iron"))
                {
                    SFXForGame.PlayIronPing();
                    asteroid.GetComponent<Rigidbody>().AddExplosionForce(300, hit.point, 5);
                } else if (asteroidm.type.Equals("fire"))
                {
                    //AddScore(310);
                    asteroidm.health--;
                    SFXForGame.PlayHitFire();
                    if (asteroidm.health == 0)
                    {
                        GameObject explode = Instantiate(LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
                        explode.SetActive(true);
                        Destroy(asteroid.transform.parent.gameObject);
                    }
                }
                else if (asteroidm.isBig) {
					//AddScore (300);
					//need to check asteroid type
                    if ((asteroidm.type.Contains("brown") && level>1 && (int)GameTime % 2 == 0) || (level == 2 && AsteroidsOnScreen.IndexOf(asteroid) <= 3 && GameTime > 45)) {
                        CreateThree(asteroid);
                    }
					else if (asteroidm.type.Contains ("brown") || asteroidm.type.Contains("red") || asteroidm.type.Contains("zigzag")) {
						CreateTwo (asteroid);
					} else if (asteroidm.type.Contains ("grey")) {
						CreateFour (asteroid);
					}

					GameObject explode = Instantiate (LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
                    Destroy(asteroid);
                    
				} else {
					//AddScore (100);
					
					GameObject explode = Instantiate (SmallExplosion, asteroid.transform.position, asteroid.transform.rotation);
					explode.SetActive (true);
                    if (asteroidm.type.Contains("fire"))
                    {
                        //AddScore(5);
                        Destroy(asteroid.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(asteroid);
                    }
				}
			} else if (asteroid.GetComponent<MissileManager> () != null){
				AccuracyVariable1++;
				AddScore (360);
				GameObject explode = Instantiate (SmallExplosion, asteroid.transform.position, asteroid.transform.rotation);
				explode.SetActive (true);
				Destroy (asteroid);
			} else if (asteroid.name.CompareTo("EStar") == 0) {
                EStar.SetActive(false);
                DetonateAsteroids();
                FadeScript.StartFlash();
            } else if (asteroid.GetComponent<EarthManager>() != null)
            {
                asteroid.GetComponent<EarthManager>().ShotHit(hit);
            }
		}
	}
    private void CalculateScore(string type, bool big)
    {
        switch (type)
        {
            case "yellow":
                AddScore(320);
                break;
            case "fire":
                AddScore(330);
                break;
            case "zigzag":
                AddScore(300);
                break;
            case "towerroc":
            case "ragnarok":
                AddScore(350);
                break;
            case "brownrag":
            case "towerrocchild":
                AddScore(160);
                break;
            case "iron":
                AddScore(140);
                break;
            case "electro":
            case "electro2":
                AddScore(450);
                break;
            case "phantom":
            case "phantom2":
                AddScore(450);
                break;
            case "brown":
            case "brown2":
            case "grey":
            case "grey2":
                if (big)
                {
                    AddScore(300);
                } else
                {
                    AddScore(100);
                }
                break;
            default:
                Debug.Log("missed asteroids type in score");
                break;
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

    //mobile buttons
    public void TurnRight()
    {
        if (!isRotating && rotationfloat != -2)
        {
            isRotating = true;
            rotationfloat = 2;
        }
    }
    public void TurnLeft()
    {
        if (!isRotating && rotationfloat != 2)
        {
            isRotating = true;
            rotationfloat = -2;
        }
    }
    public void RotateMusic()
    {
        if (!WaitBetween)
        {
            MusicForGame.RotateMusic();
        }
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
        if (ApplicationValues.Score > ApplicationValues.HighScore)
        {
            HighScoreText.GetComponent<Text>().text = ApplicationValues.Score.ToString();
        }
	}

    void BreakTowerroc(GameObject asteroid)
    {
        asteroid.GetComponent<AsteroidManager>().ReleaseTowerrocChildren(SpawnForce, asteroid.transform.position, 10);
        GameObject explode = Instantiate(LargeExplosion, asteroid.transform.position, asteroid.transform.rotation);
        explode.SetActive(true);
        Destroy(asteroid);
    }

	void CreateFour(GameObject asteroid){
		Transform point = asteroid.transform;
		point.position += new Vector3(0, 4, 0);
		GameObject asteroid1 = SpawnAsteroid(false, point, "grey");
		point.position += new Vector3(4, -4, 0);
		GameObject asteroid2 = SpawnAsteroid (false, point, "grey");
		point.position += new Vector3(-4, -4, 0);
		GameObject asteroid3 = SpawnAsteroid(false, point, "grey");
		point.position += new Vector3(-4, 4, 0);
		GameObject asteroid4 = SpawnAsteroid(false, point, "grey");
		point.position += new Vector3 (4, 0, 0);
		asteroid1.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid2.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid3.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
		asteroid4.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, asteroid.transform.position, 10);
        asteroid1.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
        asteroid2.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
        asteroid3.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
        asteroid4.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
    }

    void CreateThree(GameObject asteroid)
    {
        string type = asteroid.GetComponent<AsteroidManager>().type;
        Transform point = asteroid.transform;
        point.position += new Vector3(2, 4, 0);
        GameObject asteroid1 = SpawnAsteroid(false, point, type);
        point.position += new Vector3(0, -8, 0);
        GameObject asteroid2 = SpawnAsteroid(false, point, type);
        point.position += new Vector3(-6, 4, 0);
        GameObject asteroid3 = SpawnAsteroid(false, point, type);
        point.position += new Vector3(4, 0, 0);
        asteroid1.GetComponent<Rigidbody>().AddExplosionForce(SpawnForce, asteroid.transform.position, 10);
        asteroid2.GetComponent<Rigidbody>().AddExplosionForce(SpawnForce, asteroid.transform.position, 10);
        asteroid3.GetComponent<Rigidbody>().AddExplosionForce(SpawnForce, asteroid.transform.position, 10);
        asteroid1.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
        asteroid2.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
        asteroid3.GetComponent<AsteroidManager>().GreyRotatePos = point.position;
    }

	void CreateTwo(GameObject asteroid){
		//create two asteroids exploding outwards at location of current asteroid
		string type = asteroid.GetComponent<AsteroidManager>().type;
        if (type.Contains("zigzag")) {
            type = "brown";
        }
		Transform point = asteroid.transform;
		point.position += new Vector3(2, 4, 1);
		GameObject asteroid1 = SpawnAsteroid(false, point, type);
		point.position += new Vector3(-4, -8, -2);
		GameObject asteroid2 = SpawnAsteroid (false, point, type);
		point.position += new Vector3 (2, 4, 1);
		asteroid1.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, point.position, 10);
		asteroid2.GetComponent<Rigidbody> ().AddExplosionForce (SpawnForce, point.position, 10);
        asteroid1.GetComponentInChildren<AsteroidManager>().GreyRotatePos = Vector3.zero;
        asteroid2.GetComponentInChildren<AsteroidManager>().GreyRotatePos = Vector3.zero;
        
        //asteroid1.GetComponent<AsteroidManager> ().AddForce (new Vector3(0,1.5f,0));
        //asteroid2.GetComponent<AsteroidManager> ().AddForce (new Vector3(0, -3, 0));
    }

    void SpawnNewAsteroids()
    {
        int percent = Random.Range(1, 5);

        if (percent <= 1 && TowerrocDeployed < 3)
        {
            TowerrocDeployed++;
            SpawnAsteroid(true, spawnPoints[6], "towerroc");
        } else if (level >= 16 && percent <= 2 && RagnarokDeployed < 3)
        {
            RagnarokDeployed++;
            SpawnAsteroid(true, spawnPoints[7], "ragnarok");
        } else if (level >= 18 && percent <= 3 && IronDeployed < 5)
        {
            IronDeployed++;
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "iron");
        } else if (level >= 14 && ZigzagDeployed < 6)
        {
            ZigzagDeployed++;
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "zigzag");
        }

        nextSpawnPoint = Random.Range(0, 4);
        if (TowerrocDeployed == 3 && RagnarokDeployed == 3 && IronDeployed == 5 && ZigzagDeployed == 6)
        {
            CancelInvoke("SpwanNewAsteroids");
        }
    }

	void SpawnAsteroid(){
        //spawn big asteroid at next spawn point
        //yellow  = 1%
        //fire = 10% abobe level 8
        //GreyAsteroids at level 5
        //grey = 10% level 10-20, 20% level 20-30, 50% rest
        //electro = 5% level 30-40, 20% level 40-50
        //red and brown
        //no red for demo
        int percent = Random.Range(1, 100);

        if (percent == 1 && !YellowAsteroidDeployed)
        {
            YellowAsteroidDeployed = true;
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "yellow");
        } /*else if (level >= 8 && percent >= 90) {
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "red");
        }*/ else if (level >= 8 && percent <= 10) {
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "fire");
        } else if ((level >= 6 && percent < 10) || (level >= 8 && percent < 20) || (level >= 20 && percent < 30) || (level >= 30 && percent < 60)) {
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "grey");
        } else if ((level >= 30 && percent < 65) || (level >= 40 && percent < 80)) {
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "electro");
        } else {
            SpawnAsteroid(true, spawnPoints[nextSpawnPoint], "brown");
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

        if (type.Contains("brown"))
        {
            asteroid = BrownAsteroids[Random.Range(0, BrownAsteroids.Length)];
        }
        else if (type.Contains("grey"))
        {
            asteroid = GreyAsteroids[Random.Range(0, GreyAsteroids.Length)];
        }
        else if (type.Contains("electro"))
        {
            asteroid = ElectroAsteroids[Random.Range(0, ElectroAsteroids.Length)];
            asteroid.GetComponent<AsteroidManager>().health = 3;
        }
        else if (type.Contains("phantom"))
        {
            asteroid = PhantomAsteroids[Random.Range(0, PhantomAsteroids.Length)];
        }
        else if (type.Contains("fire"))
        {
            asteroid = FireAsteroid;
        }
        else if (type.Contains("red"))
        {
            asteroid = RedAsteroids[Random.Range(0, RedAsteroids.Length)];
        }
        else if (type.Contains("yellow"))
        {
            asteroid = YellowAsteroids;
        }
        else if (type.Contains("towerroc"))
        {
            asteroid = TowerrocAsteroid;
        }
        else if (type.Contains("ragnarok"))
        {
            asteroid = RagnarokAsteroid;
        }
        else if (type.Contains("zigzag"))
        {
            asteroid = ZigzagAsteroid;
        }
        else if (type.Contains("iron"))
        {
            asteroid = IronAsteroid;
        }
        else
        {
            asteroid = null;
            Debug.LogError("Did not get proper asteroid name");
        }
        AsteroidManager asteroidM = asteroid.GetComponentInChildren <AsteroidManager>();
        
		asteroidM.isBig = big;
		asteroidM.Earth = Earth.transform.position;
        asteroidM.GreyRotatePos = Vector3.zero;
        Vector3 DefaultScale = asteroid.transform.localScale;
		if (!big) {
			asteroid.transform.localScale = (asteroid.transform.localScale / 2);		//affects prefab
		}
		if (spawnPoint.position.x > Earth.transform.position.x) {
			asteroidM.Earth += (new Vector3 (10, 0, 0));
		} else {
			asteroidM.Earth += (new Vector3 (-10, 0, 0));
		}
		if ((type.Contains("red") && spawnPoint.position.y < Earth.transform.position.y) || (! type.Contains("red") && spawnPoint.position.y > Earth.transform.position.y)) {
			asteroidM.Earth += (new Vector3 (0, 10, -20));
		} else {
			asteroidM.Earth += (new Vector3 (0, -10, -20));
		}
        
        
        GameObject asteroid1;
        if (type.Contains("fire"))
        {
            asteroidM.health = 2;
            Quaternion quat = new Quaternion();
            quat.eulerAngles = new Vector3(0, 180, 0);
            asteroid1 = Instantiate(asteroid, spawnPoint.position, quat);
        } else
        {
            asteroid1 = Instantiate(asteroid, spawnPoint.position, spawnPoint.rotation);
        }
		asteroid.transform.localScale = DefaultScale;
        if (type.Contains("towerroc"))
        {
            asteroid1.GetComponent<AsteroidManager>().setRotation(new Vector3(0, 0, -2));
        }

        AddAsteroid (asteroid1);
		return asteroid1;
	}
	public void AddAsteroid(GameObject asteroid){
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
    private void DetonateAsteroids()
    {
        for (int i = 0; i<AsteroidsOnScreen.Count; i++)
        {
            if (AsteroidsOnScreen[i].GetComponent<AsteroidManager>().isBig)
            {
                GameObject explode = Instantiate(LargeExplosion, AsteroidsOnScreen[i].transform.position, AsteroidsOnScreen[i].transform.rotation);
                explode.SetActive(true);
            } else
            {
                GameObject explode = Instantiate(SmallExplosion, AsteroidsOnScreen[i].transform.position, AsteroidsOnScreen[i].transform.rotation);
                explode.SetActive(true);
            }
        }
        AddScore((uint)AsteroidsOnScreen.Count * 50);
        ClearAsteroids();
    }

	public void AddAsteroidInPlay(){
		AsteroidsInPlay++;
	}
	public void RemoveAsteroidInPlay(){
		AsteroidsInPlay--;
		if (ApplicationValues.Part == 3 && level == LastLevel && AsteroidsInPlay <= 0) {

		}
	}
    public GameObject[] GetAllAsteroids()
    {
        return AsteroidsOnScreen.ToArray();
    }

	void SpawnMissile(){
		//choose spawn point
		int point = Random.Range(0, 2);
		StartCoroutine (ActivateMissile (point));
	}

	private IEnumerator ActivateMissile(int point){
		SFXForGame.PlayAlert ();

		MissileSparkles [point].SetBool ("Play", true);
		for (int i = 0; i < 5; i++) {
			if (transform.rotation.eulerAngles.y < 90 || transform.rotation.eulerAngles.y > 270) {
				MissileIndicators [point].SetActive (true);
				yield return new WaitForSeconds (0.25f);
				MissileIndicators [point].SetActive (false);
			}
			yield return new WaitForSeconds (0.25f);
		}
		MissileSparkles [point].SetBool ("Play", false);

		GameObject missle = Missile;
		missle.GetComponent<MissileManager> ().Earth = Earth.transform;
		GameObject g = Instantiate (missle, MissileSpawnPoints[point].position, MissileSpawnPoints[point].rotation);
        AddAsteroid(g);
	}

	private void ResetAccuracy(){
		AccuracyVariable1 = 0;
		AccuracyVariable2 = 0;
	}
	private double CalculateAccuracy(){
		double variab;
		if (AccuracyVariable2 == 0) {
			variab = 0.00;
		} else {
			variab = ((double)AccuracyVariable1 / AccuracyVariable2) * 100;
		}
		
		return variab;
	}

	public void ResetTime(){  //reset game time to 3 minutes
		//GameTime = 180;
		if (level < 6) {
			GameTime = 60;
		} else if (level < 12) {
			GameTime = 90;
		} else if (level < 18) {
			GameTime = 120;
		} else if (level < 24) {
			GameTime = 150;
		} else if (level < 30) {
			GameTime = 180;
		} else if (level < 36) {
			GameTime = 210;
		} else if (level < 42) {
			GameTime = 240;
		} else if (level < 48) {
			GameTime = 270;
		} else if (level < 54) {
			GameTime = 300;
		} else if (level < 60) {
			GameTime = 330;
		} else {
			GameTime = 300;
			Debug.LogError ("level is outside range");
		}
	}

	public void StartLevel(){  //start the game clock
		LevelStarted = true;
        YellowAsteroidDeployed = false;
        EStar.SetActive(false);
		LevelEarthHealth = ApplicationValues.EarthHealth;

		if (SpawnEnabled) {
			InvokeRepeating ("SpawnAsteroid", FirstEnemyTime, SpawnTime-(level*LevelSpawnRate));
		}
		if (SpawnEnabled && level >= 4) {
			Invoke ("SpawnSideAsteroids", SpawnTime * ((int)GameTime%5)+1);
		}
        if (SpawnEnabled && level >= 12)
        {
            TowerrocDeployed = RagnarokDeployed = IronDeployed = ZigzagDeployed = 0;
            InvokeRepeating("SpawnNewAsteroids", 4, 10);
        }
		if (SpawnEnabled && level >= 30) {
			InvokeRepeating ("SpawnMissile", 65-level, 65-level);
		} 
	}

	private IEnumerator WaitForSubmitDown(){
		do {
			yield return null;
		} while (!Input.GetButtonDown("Submit") && !Input.GetButtonDown("Jump"));
		//} while (!Input.anyKeyDown);
	}
	private IEnumerator WaitForKeyDown(){
		do {
			yield return null;
		} while (!Input.anyKeyDown);
	}

	private IEnumerator StopLevel(){
		//Debug.Log ("hit stop");
		LevelStarted = false;
		if (IsInvoking("SpawnAsteroid")) {
			CancelInvoke ("SpawnAsteroid");
			CancelInvoke ("SpawnSideAsteroids");
            CancelInvoke("SpawnNewAsteroids");
		}
		if (IsInvoking ("SpawnMissile")) {
			CancelInvoke ("SpawnMissile");
		}
		yield return new WaitForSeconds (TimeBetweenLevels);

        WaitBetween = true;
        MusicForGame.PlayClear ();
		LevelClearText.SetActive(true);
        LevelClearText.GetComponentInChildren<Animator>().SetBool("Extra Point", CalculateAccuracy() == 100);
        yield return new WaitForSeconds(1);
        AccuracyScript.gameObject.SetActive(true);
		AccuracyScript.ShowPercent (CalculateAccuracy ());
		Time.timeScale = 0.00001f;
        yield return new WaitForSecondsRealtime(2);
        
        if (CalculateAccuracy() < 51)
        {
            SFXForGame.PlayLowAccuracy();
        } else if (CalculateAccuracy() < 100)
        {
            SFXForGame.PlayMedAccuracy();
        } else
        {
            SFXForGame.PlayHighAccuracy();
            AddScore(2500);
        }

        yield return StartCoroutine (WaitForSubmitDown ());
        //BonusPointText.SetActive(false);
        AccuracyScript.gameObject.SetActive(false);
		LevelClearText.SetActive(false);
        SFXForGame.StopPlayback();
		WaitBetween = false;
		Time.timeScale = 1;
		MusicForGame.ResumePlayback ();
		ClearAsteroids ();
		ResetAccuracy ();

        if ((ApplicationValues.demo && level == 20) || level == 50)
        {
            FinishedGame();
        } else if (level == LastLevel) {
			StartCoroutine (LoadNextLevel ());
		} else {
			level++;
			StartCoroutine (ReadyMessage ());
		}
	}

	private IEnumerator ReadyMessage(){
		//Debug.Log ("Ready");
		ResetTime ();
        TopLevelImage.sprite = TopLevelSprites[level - 1];
		yield return new WaitForSeconds (1);
        Announcer.SetActive(true);
        SFXForGame.PlayAnnouncerMessage();
        yield return new WaitForSeconds(1.33f);
		MessageText.SetActive (true);
		//MessageText.GetComponent<Text> ().text = string.Format ("Level {0}", level);
        MessageText.GetComponent<Image>().sprite = LevelImages[level - 1];
        yield return new WaitForSeconds(0.8f);
        MessageText.SetActive(false);
		yield return new WaitForSeconds (2);
        /*MessageText.GetComponent<Text> ().text = "Ready";
		yield return new WaitForSeconds (1);
		MessageText.GetComponent<Text> ().text = "Go";
		yield return new WaitForSeconds (1);*/
        //MessageText.SetActive (false);
        Announcer.SetActive(false);
		//ReadyVideo.SetActive (true);
		//yield return new WaitForSeconds (3);
		//ReadyVideo.SetActive (false);
		StartLevel ();
	}

	private IEnumerator LoadNextLevel(){

		yield return new WaitForSeconds (0.2f);
        //MessageText.GetComponent<Text> ().text = "Loading Bonus Level";
        //MessageText.GetComponent<Image>().sprite = BonusLevelImage;
        //MessageText.SetActive (true);
        FadeScript.FadeScreenToBlack();
		yield return new WaitForSeconds (3);
        LoadScreenBonus.SetActive(true);
		/*if (ApplicationValues.Part == 1) {
            AsyncOperation async = SceneManager.LoadSceneAsync("Bonus1");
		} else {
            AsyncOperation async = SceneManager.LoadSceneAsync("Bonus2");
		}*/
        AsyncOperation async = SceneManager.LoadSceneAsync("Bonus1");
    }

	public void Pause(){
		if (paused && !WaitBetween) {
			DisplayCursor = true;
			SFXForGame.PlayUnPause ();
			Time.timeScale = 1;
            Cursor.visible = false;
			paused = false;
			PauseMenu.SetActive (false);
		} else if (!WaitBetween) {
			DisplayCursor = false;
			SFXForGame.PlayPause ();
			Time.timeScale = 0;
            Cursor.visible = true;
			paused = true;
			PauseMenu.SetActive (true);
		}
	}

	public void Continue(){
		ApplicationValues.FreeContinue--;
		Time.timeScale = 1;
        Cursor.visible = false;
		DisplayCursor = true;
		WaitBetween = false;
		GameOverMenu.SetActive (false);
        MainHUD.SetActive(true);
        FadeScript.Disable();
		LevelStarted = false;
        Earth.SetActive(true);
		Earth.GetComponentInChildren<EarthManager> ().SetEarthHit (10);
        MusicForGame.ResumePlayback();
        ClearAsteroids();
        ResetAccuracy();
        SFXForGame.SwitchAnnouncer();
        //Earth.GetComponent<EarthManager> ().SetEarthHit (LevelEarthHealth);  //when other earth
        //MusicForGame.ResumePlayback ();
        StartCoroutine (ReadyMessage ());
	}

	public void StartGameOver(){
		LevelStarted = false;
		if (IsInvoking("SpawnAsteroid")) {
			CancelInvoke ("SpawnAsteroid");
			CancelInvoke ("SpawnSideAsteroids");
		}
		if (IsInvoking ("SpawnMissile")) {
			CancelInvoke ("SpawnMissile");
		}
		if (IsInvoking ("StopLevel")) {
			CancelInvoke ("StopLevel");
		}
		StartCoroutine (GameOverAnimation ());
	}
    public void PrepareGameOver()
    {
        GameOverMenu.SetActive(true);
        GameOverMenu.GetComponent<VideoPlayer>().Prepare();
    }
	private IEnumerator GameOverAnimation(){
		Instantiate (EarthExplosion, Earth.transform.position + new Vector3(15.3f, -21.7f, 0), Earth.transform.rotation);
		Earth.SetActive (false);
		//EarthExplosionVideo.SetActive (true);
		SFXForGame.PlayEarthDestroyed ();
        
        //GameOverMenu.GetComponent<VideoPlayer>().Prepare();
        WaitBetween = true;
		DisplayCursor = false;
        StartCoroutine(GameOverSkip());
		yield return new WaitForSeconds (0.5f);
		FadeScript.StartFade ();
		yield return new WaitForSeconds (4);
        GameOverMenu.SetActive(true);
        //GameOverMenu.GetComponent<VideoPlayer>().Play();
        yield return new WaitForSeconds (0.3f);
		MusicForGame.PlayGameOver ();
		MainHUD.SetActive (false);
		//FadeScript.Disable ();
		yield return new WaitForSeconds (3);

		StopCoroutine (GameOverSkip ());
		GameOver ();
	}
	private IEnumerator GameOverSkip(){
		yield return StartCoroutine (WaitForSubmitDown ());
		StopCoroutine (GameOverAnimation ());
		FadeScript.FadeRightToBlack ();
		yield return new WaitForSeconds (1);
		GameOver ();
	}

	public void GameOver(){
        //Time.timeScale = 0;
        //GameOverMenu.SetActive (true);
        Cursor.visible = true;
		//MusicForGame.PlayGameOver ();

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
        Cursor.visible = true;
        Earth.GetComponentInChildren<EarthManager>().Mute();

		GameEnd.SetActive (true);
        FinishedScoreText.text = ApplicationValues.Score.ToString();

        /*if (ApplicationValues.ScoreFile == null)
        {
            ApplicationValues.ScoreFile = HighScore.LoadData(ApplicationValues.FileName);
        }
		if (ApplicationValues.ScoreFile.isHighScore (ApplicationValues.Score)) {
			HighScoreMenu.SetActive (true);
		}*/
	}

	public void Quit(){
		AsteroidManager[] asters = GetComponentsInParent<AsteroidManager> ();
		for (int i = 0; i < asters.Length; i++){
			Destroy (asters [i].gameObject);
		}
		if (HighScoreMenu.activeInHierarchy) {
			char[] init = (Initials.text + "   ").ToCharArray();
			for (int i = 0; i < 3; i++) {
				if (!char.IsLetterOrDigit(init[i])){
					init [i] = ' ';
				}
			}
            ApplicationValues.ScoreFile.Add(init, ApplicationValues.Score);
            HighScore.SaveData (ApplicationValues.FileName, ApplicationValues.ScoreFile);
		}
		SceneManager.LoadSceneAsync ("MainMenu");
	}
}
