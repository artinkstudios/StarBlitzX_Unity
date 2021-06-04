using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EarthManager : MonoBehaviour {

	public ScreenShake ShakeScript;
	public bool enableGameOver;
	public Material[] ComputerHitMaterials;
    public Material[] MobileHitMaterials;
    private Material[] HitMaterials;
    public Material ComputerDefaultMaterial;
    public Material MobileDefaultMaterial;
    private Material DefaultMaterial;
	private int CurrentMaterial = 0;
	public AudioClip EarthHit;
	private AudioSource EarthSource;
	public GameObject SmallAsteroidExplosion;
	public GameObject BigAsteroidExplosion;
	public GameObject EarthCrack;
	public float flashLength = 1;
	private float flashingTime = 0;
    public EarthHitAnimationScript HitAnimation;
    public GameObject Shield;
    public GameObject ShotShield;
    private float ShieldTime = 0;

    // Use this for initialization
    void Start () {
#if UNITY_STANDALONE
        DefaultMaterial = ComputerDefaultMaterial;
        HitMaterials = ComputerHitMaterials;
#endif
#if UNITY_ANDROID || UNITY_IOS
        DefaultMaterial = MobileDefaultMaterial;
        HitMaterials = MobileHitMaterials;
#endif
        CurrentMaterial = 0;
        GetComponent<Renderer>().material = DefaultMaterial;
		EarthSource = GetComponent<AudioSource> ();
        //Shield = transform.Find("Earth Deflector Shield").gameObject;

		if (ApplicationValues.EarthHealth < 10) {
			CurrentMaterial = 9 - ApplicationValues.EarthHealth;
			GetComponent<Renderer> ().material = HitMaterials [CurrentMaterial];
			CurrentMaterial++;
		}
	}

	void Update () {
		if (flashingTime < 0) {
			flashingTime = 0;
			GetComponent<Renderer> ().material.color = Color.white;
		} else if (flashingTime > 0) {
			flashingTime -= Time.deltaTime;
		}
        if (ShieldTime > 0)
        {
            ShieldTime -= Time.deltaTime;
            if (ShieldTime <= 0)
            {
                Shield.SetActive(false);
            }
        }
	}

	void OnCollisionEnter(Collision other) {
		ApplicationValues.EarthHealth--;
		EarthSource.PlayOneShot (EarthHit);
        HitAnimation.EarthHit();
		GameObject aster = other.gameObject;
		bool big = aster.GetComponent<AsteroidManager> ().isBig;

        if (enableGameOver && ApplicationValues.EarthHealth <= 0)
        {
            EarthSource.Play();
            //earth explosion
            Camera.main.GetComponent<GameManager>().StartGameOver();
        }

        if (CurrentMaterial < 10) {
			GetComponent<Renderer> ().material = HitMaterials [CurrentMaterial];
			CurrentMaterial++;
			GetComponent<Renderer> ().material.color = Color.red;
			flashingTime = flashLength;
		}

		if (aster.GetComponent<AsteroidManager> ().type.CompareTo ("phantom") == 0 || aster.GetComponent<AsteroidManager>().type.CompareTo("fire") == 0) {
			ShakeScript.shakeDuration = 1;
			Destroy (other.transform.parent.gameObject);
			GameObject ex = Instantiate(BigAsteroidExplosion, other.transform.position, other.transform.rotation);
			ex.GetComponent<ParticleSystem> ().Play ();
			Destroy (ex, ex.GetComponent<ParticleSystem> ().main.duration);
		} else {
			if (big) {
				//big asteroid explosion
				ShakeScript.shakeDuration = 0.8f;
				GameObject ex = Instantiate(BigAsteroidExplosion, other.transform.position, other.transform.rotation);
				ex.GetComponent<ParticleSystem> ().Play ();
				Destroy (ex, ex.GetComponent<ParticleSystem> ().main.duration);
			} else {
				//small asteroid explosion
				ShakeScript.shakeDuration = 0.3f;
				GameObject ex = Instantiate(SmallAsteroidExplosion, other.transform.position, other.transform.rotation);
				ex.GetComponent<ParticleSystem> ().Play ();
				Destroy (ex, ex.GetComponent<ParticleSystem> ().main.duration);
			}
			Destroy (aster);
		}

		ContactPoint contact = other.contacts [0];
		GameObject ec = Instantiate (EarthCrack, gameObject.transform);
		ec.transform.position = (new Vector3(contact.point.x, contact.point.y, contact.point.z));
		ec.transform.position = Vector3.MoveTowards (ec.transform.position, gameObject.transform.position, 26f);

		RaycastHit hit;
		if (!Physics.Raycast (new Vector3 (contact.point.x + (-2*contact.normal.x), contact.point.y + (-2*contact.normal.y), contact.point.z + (-2*contact.normal.z)), new Vector3 (contact.normal.x, contact.normal.y, contact.normal.z), out hit)) {
			Debug.Log ("missed ray");
		}
		//ec.transform.rotation = Quaternion.FromToRotation (contact.point, (new Vector3(0, hit.normal.y -80, 0)));
		ec.transform.rotation = Quaternion.LookRotation(contact.point - gameObject.transform.position);
		//ec.transform.rotation = Quaternion.FromToRotation (gameObject.transform.position, contact.point);
		ec.transform.eulerAngles = ec.transform.eulerAngles + (new Vector3 (1.2f, 5, 0));  //for 3D prefab
		if (!big) {
			//ec.transform.localScale += (new Vector3 (-0.3f, -0.3f, -0.3f));
			ec.transform.localScale.Scale (new Vector3 (0.8f, 0.8f, 0.8f));
		}
	}

    public void ShotHit(RaycastHit hit)
    {
        /*GameObject ec = Instantiate(ShotShield, hit.point, hit.transform.rotation);
        ec.GetComponentInChildren<ParticleSystem>().Play();*/
        /*if (ShieldTime > 0)
        {
            ShieldTime = 1;
        } else
        {
            ShieldTime = 1;
            Shield.SetActive(true);
        }*/
        //Shield.GetComponent<ShieldEffect>().Add(hit.point, 50, 0.1f, 2, 4);
        Shield.GetComponent<SpotShieldEffect>().Add(hit.point);
    }

	public void SetEarthHit(int health){
		ApplicationValues.EarthHealth = health;
		if (ApplicationValues.EarthHealth < 10) {
			CurrentMaterial = 9 - ApplicationValues.EarthHealth;
			GetComponent<Renderer> ().material = HitMaterials [CurrentMaterial];
			CurrentMaterial++;
		} else {
			CurrentMaterial = 0;
			GetComponent<Renderer> ().material = DefaultMaterial;
		}
        HitAnimation.EarthHit();
    }
    public void Mute()
    {
        HitAnimation.Mute();
    }
}
