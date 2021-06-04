using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ArrowKeyWait : MonoBehaviour {
	private bool isSelected = false;
    private bool decreasing = true;
	public string audioname;
	public int currentValue;
	public Image Parent;
	public Sprite ParentUnselect;
	public Sprite ParentSelect;
	public Sprite[] unselected;
	public Sprite[] selected;
	public AudioClip LaserSample;
	public AudioSource sample;

	// Use this for initialization
	void Start () {
#if UNITY_IOS || UNITY_ANDROID
        UnityEngine.Events.UnityAction myAction = MobileChange;
        GetComponent<Button>().onClick.AddListener(myAction);
#endif

        currentValue = 9;
		float temp;
		ApplicationValues.GameMixer.GetFloat(audioname, out temp);
        //currentValue = (int)((temp+50) / (50.0f / 9));
        //Mathf.Log10(currentValue / 9.0f) * 20

        currentValue = ApplicationValues.GetValuefromVolume(temp);
        
		GetComponent<Image> ().sprite = unselected [currentValue];
    }

    // Update is called once per frame
    void Update () {
#if UNITY_STANDALONE
        if (isSelected && Input.GetButtonDown ("Horizontal")) {
			//Debug.Log ( Input.GetAxisRaw ("Horizontal"));
			//Debug.Log("moving sound");
			currentValue += (int) Input.GetAxisRaw ("Horizontal");

			if (currentValue < 0) {
				currentValue = 0;
			} else if (currentValue > 9) {
				currentValue = 9;
			}
			ChangeValue();
		}
#endif
	}

    void MobileChange()
    {
        if ((currentValue == 0 && decreasing) || (currentValue == 9 && !decreasing))
        {
            decreasing = !decreasing;
        }
        if (decreasing)
        {
            currentValue--;
        } else
        {
            currentValue++;
        }
        ChangeValue();
    }

    public void DecreaseValue()
    {
        currentValue--;
        if (currentValue < 0)
        {
            currentValue = 0;
        }
        ChangeValue();
    }
    public void IncreaseValue()
    {
        currentValue++;
        if (currentValue > 9)
        {
            currentValue = 9;
        }
        ChangeValue();
    }

    private void ChangeValue()
    {
        GetComponent<Image>().sprite = unselected[currentValue];
        
        float volume = ApplicationValues.GetVolumefromValue(currentValue);
        ApplicationValues.GameMixer.SetFloat(audioname, volume);

        if (audioname.CompareTo("SFXVolume") == 0)
        {
            //Camera.main.GetComponent<ButtonScript> ().LaserSamplePlay ();
            sample.PlayOneShot(LaserSample);
        }
    }

	public void Change(int value){
		currentValue = value;
		GetComponent<Image> ().sprite = unselected [currentValue];
	}

	public void Selected(){
		isSelected = true;
		//GetComponent<Image> ().sprite = selected [currentValue];
		Parent.sprite = ParentSelect;
		//Parent.rectTransform.localScale = new Vector3 (1.05f, 1.05f, 1.05f);
	}
	public void notSelected(){
		isSelected = false;
		//GetComponent<Image> ().sprite = unselected [currentValue];
		Parent.sprite = ParentUnselect;
		//Parent.rectTransform.localScale = new Vector3 (1, 1, 1);
	}
}
