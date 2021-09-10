using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyDisplay : MonoBehaviour {

	[Tooltip("Individual Numbers (0-9) in required font")]
	public Sprite[] Numbers;
	public Image Digit1;
	public Image Digit2;
	public Image Digit3;
	public Image PercentSign;
	//public Text EnterText;

	private static bool FadeInNum = false;
	private static bool FadeOut = false;
	Func<bool> FOF = () => FadeOut == false;
	Func<bool> FIN = () => FadeInNum == false;

	void Start () {
		if (Numbers.Length != 10) {
			Debug.Log ("Accuracy Numbers not set");
		}
		Color co = PercentSign.color;
		co.a = 0;
		PercentSign.color = co;
		Digit1.color = co;
		Digit2.color = co;
		Digit3.color = co;
		//EnterText.color = co;

		/*Digit1.SetActive (false);
		Digit2.SetActive (false);
		Digit3.SetActive (false);
		PercentSign.SetActive (false);*/
	}
	
	// Update is called once per frame
	void Update () {
		if (FadeInNum) {
			FadeThisIn ();
		} else if (FadeOut) {
			FadeThisOut ();
		}
	}

	public void ShowPercent(double percent){
		/*if (percent < 1) {
			percent = percent * 100;
		}*/

		int percent1 = (int)(percent / 100);
		int percent2 = (int)(percent / 10);
		percent2 = percent2 % 10;
		int percent3 = (int)(percent % 10);

		Digit1.sprite = Numbers [percent1];
		Digit2.sprite = Numbers [percent2];
		Digit3.sprite = Numbers [percent3];

		/*if (percent1 == 1) {
			Digit1.SetActive (true);
		}
		Digit2.SetActive (true);
		Digit3.SetActive (true);
		PercentSign.SetActive (true);*/

		StartFade ();
	}

	void FadeThisIn(){
		Color co = PercentSign.color;
		co.a = Mathf.Min (co.a + 0.05f, 1);
		PercentSign.color = co;
		Digit2.color = co;
		Digit3.color = co;
		if (Digit1.sprite == Numbers [1]) {
			Digit1.color = co;
		}

		if (co.a >= 1) {
			FadeInNum = false;
		}
	}
	void FadeThisOut(){
		/*Color co = EnterText.color;
		co.a = Mathf.Min (co.a + 0.01f, 1);
		EnterText.GetComponent<Text> ().color = co;

		if (co.a >= 1) {
			FadeOut = false;
		}*/
	}

	private IEnumerator FadeThis(){
		yield return new WaitForSecondsRealtime (1);
		FadeInNum = true;
		yield return new WaitUntil (FIN);
		yield return new WaitForSecondsRealtime (3);
		FadeOut = true;
		yield return new WaitUntil (FOF);
	}

	public void StartFade(){
		Color co = PercentSign.color;
		co.a = 0;
		PercentSign.color = co;
		Digit1.color = co;
		Digit2.color = co;
		Digit3.color = co;
		StartCoroutine (FadeThis ());
	}
}
