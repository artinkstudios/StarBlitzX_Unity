using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour {

	public bool isActive = false;
	public bool isBig;
	public float speed = 2;
	private Vector3 Rot;
	private Vector3 force = new Vector3 (0,0,0);
    public Vector3 GreyRotatePos;
	public string type;
	public float MaxRotation = 3;
	public Vector3 Earth;
	public int health = 3;
    private MeshRenderer Shield;
	//public bool SpecialEffective = false;
	private bool exiting = false;
	private Vector3 defaultSize;

	// Use this for initialization
	void Start () {
		exiting = false;
        //SpecialEffective = false;
        if (!type.Contains("towerroc"))
        {
            if (type.Contains("2"))
            {
                defaultSize = Vector3.one * 3.8f;
            }
            else
            {
                defaultSize = Vector3.one;
            }
            //defaultSize = transform.localScale;
            if (type.CompareTo("fire") != 0 && !type.Equals("brownrag"))
            {
                Shield = transform.Find("Plasma Shield").GetComponent<MeshRenderer>();
            }


            if (!isBig)
            {
                speed = speed * 2;
                transform.localScale = (defaultSize * 0.6f);
                transform.localScale += (Random.insideUnitSphere / 8);
                //transform.localScale += (Vector3.one * 0.6f);
                if (GreyRotatePos != Vector3.zero)
                {
                    Earth = new Vector3(transform.position.x, transform.position.y, Earth.z);
                }
                if (Mathf.Abs(transform.position.x) < 100 && (type.CompareTo("fire") != 0 && !type.Equals("brownrag")))
                {
                    Shield.enabled = false;
                }
            }
            else
            {
                transform.localScale += (Random.insideUnitSphere / 4);
            }
            
            Rot = Random.insideUnitSphere * MaxRotation;

            
        } else if (type.Equals("towerrocchild"))
        {
            Rot = Random.insideUnitSphere * MaxRotation;
        }
        if (ApplicationValues.isHard)
        {
            speed = speed * 2;
        }
        speed = speed + ((10 - ApplicationValues.EarthHealth) / 2);
        isActive = true;
	}
	

	void FixedUpdate () {
		//on spawn, increase until normal size
		if (isBig && (transform.localScale.x < defaultSize.x && transform.localScale.y < defaultSize.y && transform.localScale.z < defaultSize.z)) {
			transform.localScale += (Vector3.one * 0.01f);
		}
        
		transform.Rotate (Rot.x, Rot.y, Rot.z);

		if (!Camera.main.GetComponent<GameManager>().WaitBetween && (type.Contains ("phantom") || type.Contains("fire"))) {
			float step = speed * Time.deltaTime;
			transform.parent.position = Vector3.MoveTowards (transform.parent.position, Earth, step);
			//GetComponentInParent<Transform> ().position = Vector3.MoveTowards (GetComponentInParent<Transform> ().position, Earth.position, step);
		} else if (!Camera.main.GetComponent<GameManager>().WaitBetween) {
			/*Vector3 direction = (Earth.position - transform.position).normalized;
			GetComponent<Rigidbody>().MovePosition(transform.position + direction * speed * Time.deltaTime);*/
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, Earth, step);
            
            if (type.Equals("zigzag"))
            {
                transform.position = new Vector3(transform.position.x + (Mathf.Sin(Time.time * 1.5f) * 0.3f), transform.position.y, transform.position.z);
            }
		}

        if (!isBig && type.Contains("grey"))
        {
            GreyRotatePos = Vector3.MoveTowards(GreyRotatePos, Earth, speed * Time.deltaTime);
            transform.RotateAround(GreyRotatePos, Vector3.forward, 40 * Time.deltaTime);
            
        } else if (!isBig && type.Contains("brown") && GreyRotatePos != Vector3.zero)
        {
            GreyRotatePos = Vector3.MoveTowards(GreyRotatePos, Earth, speed * Time.deltaTime);
            transform.RotateAround(GreyRotatePos, Vector3.forward, -40 * Time.deltaTime);
        }

		if (force != Vector3.zero) {
			//Debug.Log ("Force Changed");
			gameObject.GetComponent<Rigidbody> ().AddForce (force, ForceMode.Impulse);
		}
        if (type.Equals("iron") && transform.position.z > 100)
        {
            Destroy(gameObject);
        }
	}

    public void setRotation(Vector3 rotation)
    {
        Rot = rotation;
    }

	public void AddForce(Vector3 newforce){
		//Debug.Log ("Got new Force");
		Debug.Log (newforce);
		force += newforce;
	}

    public void ReleaseTowerrocChildren(float force, Vector3 point, float radius)
    {
        Destroy(transform.Find("Plasma Shield").gameObject);
        for (int i = 0; i<4; i++)
        {
            Transform tran = transform.GetChild(i);
            if (i%2 == 0)
            {
                tran.position = new Vector3(tran.position.x, tran.position.y, tran.position.z-4);
            } else
            {
                tran.position = new Vector3(tran.position.x, tran.position.y, tran.position.z+4);
            }
            tran.gameObject.AddComponent<Rigidbody>();
            tran.GetComponent<Rigidbody>().useGravity = false;
            tran.GetComponent<SphereCollider>().enabled = true;
            tran.GetComponent<AsteroidManager>().enabled = true;
            tran.GetComponent<AsteroidManager>().Earth = Earth;
            tran.GetComponent<Rigidbody>().AddExplosionForce(force, point, radius);
            tran.GetComponent<Rigidbody>().drag = 0.4f;
            Camera.main.GetComponent<GameManager>().AddAsteroid(tran.gameObject);
        }
        transform.DetachChildren();
    }

	void OnDestroy(){
		/*if (!exiting && Camera.main.GetComponent<GameManager> ().level == Camera.main.GetComponent<GameManager> ().LastLevel) {
			Camera.main.GetComponent<GameManager> ().RemoveAsteroidInPlay ();
		} else if (!exiting) {
			Camera.main.GetComponent<GameManager> ().RemoveAsteroid (gameObject);
		}*/
        if (type.CompareTo("fire") == 0 || type.CompareTo("phantom") == 0)
        {
            Camera.main.GetComponent<GameManager>().RemoveAsteroid(transform.parent.gameObject);
        } else
        {
            Camera.main.GetComponent<GameManager>().RemoveAsteroid(gameObject);
        }
        
    }

	void OnApplicationQuit(){
		exiting = true;
	}
}
