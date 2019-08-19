using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

	public int AverageFPS { get; private set; }
	public int frameRange = 60;

	int[] fpsBuffer;
	int fpsBufferIndex;

	public Text FPSText;


	// Use this for initialization
	void Start () {
		FPSText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (fpsBuffer == null || fpsBuffer.Length != frameRange) {
			InitializeBuffer();
		}
		UpdateBuffer();
		CalculateFPS();

		FPSText.text = Mathf.Clamp(AverageFPS, 0, 99).ToString();
	}

	void InitializeBuffer () {
		if (frameRange <= 0) {
			frameRange = 1;
		}
		fpsBuffer = new int[frameRange];
		fpsBufferIndex = 0;
	}

	void UpdateBuffer () {
		fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
		if (fpsBufferIndex >= frameRange) {
			fpsBufferIndex = 0;
		}
	}

	void CalculateFPS () {
		int sum = 0;
		for (int i = 0; i < frameRange; i++) {
			sum += fpsBuffer[i];
		}
		AverageFPS = sum / frameRange;
	}
}
