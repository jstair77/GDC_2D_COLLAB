using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndLevel : MonoBehaviour {

	public GameObject levelManager;
	private bool hasTouched;

	void Start(){
		levelManager = GameObject.Find ("LevelManager");
		hasTouched = false;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Player") && !hasTouched) {
			levelManager.GetComponent<DEFAULT_LevelManager> ().endLevel ();
			hasTouched = true;
		}
	}
}
