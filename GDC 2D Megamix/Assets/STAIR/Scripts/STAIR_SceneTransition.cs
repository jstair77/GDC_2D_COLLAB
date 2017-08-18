using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class STAIR_SceneTransition : MonoBehaviour {

	public int doorValue;
	public float speed;
	public string scene;
	public bool horizontalDoor;
	public Image fade;
	private GameObject player;
	private Rigidbody2D rb;
	private Vector2 startPosition;
	public Vector2 endPosition;
	private bool started;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = player.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (started) {
			if (horizontalDoor) {
				rb.velocity = new Vector2 ((endPosition.x - startPosition.x) * speed, rb.velocity.y);
			} else {
				rb.velocity = new Vector2 (rb.velocity.x, (endPosition.y - startPosition.y) * speed);
			}
			fade.color = new Color(0f, 0f, 0f, Vector3.Distance(player.transform.position, startPosition) / Vector3.Distance(endPosition, startPosition));
			if (fade.color.a >= 1f) {
				SceneManager.LoadScene (scene);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Player") && !started) {
			STAIR_Chip chip = player.GetComponent<STAIR_Chip> ();
			if (chip.getCanMove()) {
				started = true;
				chip.setCanMove (false);
				chip.setDoorValue (doorValue);
				startPosition = player.transform.position;
			}
		}
	}
}
