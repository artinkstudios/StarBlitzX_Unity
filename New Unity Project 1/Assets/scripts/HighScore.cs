using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

[Serializable]
public class HighScore {
	private char[,] Initials;
	private float[] Score;

	public HighScore(){
		Initials = new char[10,3];
		Score = new float[10];

		for (int i = 0; i < 10; i++) {
			Score [i] = 0;
			Initials [i, 0] = ' ';
			Initials [i, 1] = ' ';
			Initials [i, 2] = ' ';
		}
	}

	public float[] scores{
		get { return Score; }
		set { Score = value; }
	}
	public float score(int index){
		return Score [index];
	}
	public void SetScore(int index, float s){
		Score [index] = s;
	}
	public char[] GetInitials(int index){
		char[] init = new char[3];
		init [0] = Initials [index, 0];
		init [1] = Initials [index, 1];
		init [2] = Initials [index, 2];
		return init;
	}
	public void SetInitials(int index, char[] init){
		Initials [index, 0] = init [0];
		Initials [index, 1] = init [1];
		Initials [index, 2] = init [2];
	}

	public bool isHighScore(float s){
		bool In = false;
		for (int i = 0; i < 10 && !In; i++) {
			if (s > Score [i]) {
				In = true;
			}
		}
		return In;
	}

	public void Add (char[] name, float s){
		int index = -1;
		for (int i = 0; i < 10 && index<0; i++) {
			if (s > Score [i]) {
				index = i;
			}
		}
		for (int i = 8; index >= 0 && i >= index; i--) {
			Score [i + 1] = Score [i];
			Initials [i + 1, 0] = Initials [i, 0];
			Initials [i + 1, 1] = Initials [i, 1];
			Initials [i + 1, 2] = Initials [i, 2];
		}
		if (index >= 0) {
			Score [index] = s;
			Initials [index, 0] = name [0];
			Initials [index, 1] = name [1];
			Initials [index, 2] = name [2];
		}
	}

	public static void SaveData(string FileName, HighScore s){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/" + FileName, FileMode.OpenOrCreate);

		bf.Serialize (file, s);
		file.Close ();
	}
	public static IEnumerator GetFromWebpage(string WebPage, HighScore h){
		WWW w = new WWW ("http://server.com/file.ext");
		//wait for result
		yield return w;

		if (w.error != null) {
			//error happened
			Debug.LogError(w.error);
		} else {
			//retrieved text
			if (h == null) {
				h = new HighScore();
			}
			//assume 7 figure score
			for (int i = 0; i < 10; i++) {
				h.SetInitials (i, new char[]{ w.text [i*10], w.text [i*10 + 1], w.text [i*10 + 2] });
				h.SetScore (i, float.Parse (w.text.Substring (i*10 + 3, 7)));
			}
		}
	}
	public static IEnumerator PostToWebPage(string WebPage, HighScore h){
		byte[] send;
		char[] temp = new char[100];

		for (int i = 0; i < 10; i++) {
			char[] init = h.GetInitials (i);
			temp [i * 10] = init [0];
			temp [i * 10 + 1] = init [1];
			temp [i * 10 + 2] = init [2];

			string s = h.score (i).ToString ();
			for (int j = s.Length-1; j >= 0; j--) {
				temp [i * 10 + 3 + 7 - s.Length + j] = s [j];
			}
			for (int j = 7 - s.Length; j > 0; j--) {
				temp [i * 10 + 3 + j - 1] = '0';
			}
		}

		send = ASCIIEncoding.UTF8.GetBytes (temp);
		WWW w = new WWW (WebPage, send);
		yield return w;
	}

	public static HighScore LoadData(string FileName){
		if (File.Exists (Application.persistentDataPath + "/" + FileName)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + FileName, FileMode.Open);
			HighScore score = (HighScore)bf.Deserialize (file);
			file.Close ();

			return score;
		} else {
			Debug.LogError ("Unable to retrieve File.  File may not exist or unable to access.");
			return new HighScore ();
		}
	}
}
