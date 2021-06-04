using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTShaderIm : MonoBehaviour {

	public Material material;
	// Use this for initialization
	void Start () {
		material = new Material(Shader.Find("Hidden/CRT"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_MainTex", source);
		Graphics.Blit(source, destination, material);
	}
}
