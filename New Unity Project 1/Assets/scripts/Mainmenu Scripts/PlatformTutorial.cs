using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformTutorial : MonoBehaviour {

	public Sprite keyboard;
	public Sprite Android;
	public Sprite IOS;
    public GameObject[] DisableForMobile;

	// Use this for initialization
	void Start () {
		#if UNITY_STANDALONE
		GetComponent<Image>().sprite = keyboard;
		#endif

		#if UNITY_IOS
		GetComponent<Image>().sprite = IOS;
		#endif

		#if UNITY_ANDROID
		GetComponent<Image>().sprite = Android;
#endif

#if UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i<DisableForMobile.Length; i++)
        {
            DisableForMobile[i].SetActive(false);
        }
#endif
    }
	

}
