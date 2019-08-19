using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour {

	Material mat;

	void Start () {
		mat = GetComponent<Image> ().material;
	}

	void Update () {
		mat.mainTextureOffset = new Vector2(mat.mainTextureOffset.x, mat.mainTextureOffset.y + (Time.deltaTime/5f));
		mat.SetTextureOffset ("_TextureB", new Vector2 (mat.GetTextureOffset ("_TextureB").x, mat.GetTextureOffset ("_TextureB").y + (Time.deltaTime / 10f)));

	}
}
