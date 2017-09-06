using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEMO : MonoBehaviour {

	public GameObject text;
	private GameObject player;

	void Start(){
		player = GameObject.Find ("Player");

		player.GetComponent<STAIR_Chip> ().setCanMove (false);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			Destroy (player);
			SceneManager.LoadScene ("Demo");
		}

		if (Input.GetKey(KeyCode.JoystickButton7)) {
			text.SetActive (false);
			player.GetComponent <STAIR_Chip>().setCanMove (true);
		}
	}
}
