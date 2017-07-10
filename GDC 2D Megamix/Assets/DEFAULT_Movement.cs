using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFAULT_Movement : MonoBehaviour {

	public float groundSpeed;
	public float acceleration;
	public float maxSpeed;
	public float jumpSpeed;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float horizontalInput = Input.GetAxis ("Horizontal");
		if (Mathf.Abs (rb.velocity.x) < maxSpeed) {
			rb.AddForce (new Vector2 (horizontalInput * groundSpeed * acceleration, 0f));
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			rb.AddForce (new Vector2 (0f, jumpSpeed), ForceMode2D.Impulse);
		}
	}
}
