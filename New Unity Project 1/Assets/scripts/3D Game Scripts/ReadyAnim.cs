using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyAnim : MonoBehaviour {

	public Animator animator;

	void Awake() {
		if(animator == null) animator = GetComponent<Animator>();
	}

	void OnEnable() {
		animator.Play("Ready_Set_Go");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
