using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EarthManager : MonoBehaviour {

	public bool enableGameOver;
	public Material[] HitMaterials;
	private Material DefaultMaterial;
	private int CurrentMaterial = 0;
	public AudioClip EarthHit;
	private AudioSource EarthSource;
	public GameObject SmallAsteroidExplosion;
	public GameObject BigAsteroidExplosion;
	public GameObject EarthCrack;
	// Use this for initialization
	void Start () {
		CurrentMaterial = 0;
		DefaultMaterial = GetComponent<Renderer> ().material;
		EarthSource = GetComponent<AudioSource> ();

		if (ApplicationValues.EarthHealth < 10) {
			CurrentMaterial = 9 - ApplicationValues.EarthHealth;
			GetComponent<Renderer> ().material = HitMaterials [CurrentMaterial];
			CurrentMaterial++;
		}
	}

	void Update () {
		
	}

	void OnCollisionEnter(Collision other) {
		ApplicationValues.EarthHealth--;
		EarthSource.PlayOneShot (EarthHit);
		GameObject aster = other.gameObject;
		bool big = aster.GetComponent<AsteroidManager> ().isBig;

		if (CurrentMaterial < 10) {
			GetComponent<Renderer> ().material = HitMaterials [CurrentMaterial];
			CurrentMaterial++;
		}

		if (aster.GetComponent<AsteroidManager> ().type.CompareTo ("phantom") == 0) {
			Destroy (other.transform.parent.gameObject);
			GameObject ex = Instantiate(BigAsteroidExplosion, other.transform.position, other.transform.rotation);
			ex.GetComponent<ParticleSystem> ().Play ();
			Destroy (ex, ex.GetComponent<ParticleSystem> ().main.duration);
		} else {
			if (big) {
				//big asteroid explosion
				GameObject ex = Instantiate(BigAsteroidExplosion, other.transform.position, other.transform.rotation);
				ex.GetComponent<ParticleSystem> ().Play ();
				Destroy (ex, ex.GetComponent<ParticleSystem> ().main.duration);
			} else {
				//small asteroid explosion
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
			
		if (enableGameOver && ApplicationValues.EarthHealth <= 0) {
			EarthSource.Play ();
			//earth explosion
			Camera.main.GetComponent<GameManager>().StartGameOver();
		}
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

	}
}
