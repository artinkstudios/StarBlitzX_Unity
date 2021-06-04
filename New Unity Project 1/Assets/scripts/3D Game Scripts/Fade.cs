using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

	public GameObject BlackScreen;
	public GameObject[] DeScreen;
    private float fadeSpeed = 0;
	private static bool FadeIn = false;
	private static bool FadeOut = false;
	private static bool ToBlack = false;
    private static bool FlashOut = false;

	Func<bool> FOF = () => FadeOut == false;
	Func<bool> FIF = () => FadeIn == false;
	Func<bool> FTB = () => ToBlack == false;
    Func<bool> FL = () => FlashOut == false;


    void Start () {
		
	}
	

	void Update () {
		if (FadeIn) {
			FadeThisIn ();
		} else if (FadeOut) {
			FadeThisOut ();
		} else if (ToBlack) {
			FadeToBlack ();
		} else if (FlashOut)
        {
            Flashing();
        }
	}

	void FadeThisIn(){
		Color co = BlackScreen.GetComponent<Image> ().color;
		co.a = Mathf.Min (co.a + fadeSpeed, 1);
		BlackScreen.GetComponent<Image> ().color = co;

		if (co.a >= 1) {
			FadeIn = false;
		}
	}
	void FadeThisOut(){
		Color co = BlackScreen.GetComponent<Image> ().color;
		co.r = Mathf.Max (co.r - fadeSpeed, 0);
		BlackScreen.GetComponent<Image> ().color = co;

		if (co.r <= 0) {
			FadeOut = false;
		}
	}
	void FadeToBlack(){
		Color co = BlackScreen.GetComponent<Image> ().color;
		co.r = Mathf.Max (co.r - fadeSpeed, 0);
		co.a = Mathf.Min (co.a + fadeSpeed, 1);
		BlackScreen.GetComponent<Image> ().color = co;

		if (co.a >= 1) {
			ToBlack = false;
		}
	}
    void Flashing()
    {
        Color co = BlackScreen.GetComponent<Image>().color;
        co.a = Mathf.Max(co.a - fadeSpeed, 0);
        BlackScreen.GetComponent<Image>().color = co;

        if (co.a <= 0)
        {
            FadeOut = false;
        }
    }

	private IEnumerator FadeThis(GameObject activate, GameObject[] deactivate){
        fadeSpeed = 0.02f;
        FadeIn = true;
		yield return new WaitUntil (FIF);
		//activate.SetActive (true);
        for (int i=0; i<deactivate.Length; i++)
        {
            deactivate[i].SetActive(false);
        }
		FadeOut = true;
		yield return new WaitUntil (FOF);
	}
	private IEnumerator StartToBlack (GameObject activate, GameObject[] deactivate){
        fadeSpeed = 0.02f;
        FadeIn = false;
		FadeOut = false;
		ToBlack = true;
		yield return new WaitUntil (FTB);
        for (int i = 0; i < deactivate.Length; i++)
        {
            deactivate[i].SetActive(false);
        }
    }
    private IEnumerator Flash()
    {
        fadeSpeed = 0.1f;
        FlashOut = true;
        yield return new WaitUntil(FL);
        Disable();
    }


    public void StartFade(){
		Color co = BlackScreen.GetComponent<Image> ().color;
		co = Color.red;
		co.a = 0;
		BlackScreen.GetComponent<Image> ().color = co;
		BlackScreen.SetActive (true);
		StartCoroutine (FadeThis (null, DeScreen));
	}
	public void FadeRightToBlack(){
        
        StopCoroutine (FadeThis (null, DeScreen));
		StartCoroutine (StartToBlack (null, DeScreen));
	}
    public void FadeScreenToBlack()
    {
        Color co = BlackScreen.GetComponent<Image>().color;
        co = Color.black;
        co.a = 0;
        BlackScreen.GetComponent<Image>().color = co;
        BlackScreen.SetActive(true);
        StartCoroutine(StartToBlack(null, DeScreen));
    }
    public void StartFlash()
    {
        Color co = BlackScreen.GetComponent<Image>().color;
        co = Color.white;
        co.a = 1;
        BlackScreen.GetComponent<Image>().color = co;
        BlackScreen.SetActive(true);
        StartCoroutine(Flash());
    }

	public void Disable(){
		BlackScreen.SetActive (false);
	}
}
