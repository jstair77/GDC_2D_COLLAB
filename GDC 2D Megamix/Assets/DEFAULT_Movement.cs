using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFAULT_Movement : MonoBehaviour {

	public float groundSpeed;
	public float acceleration;
	public float maxSpeed;

	public float jumpSpeed;
	public int maxJumps;
	public int jumpCount;
	private bool jumping;
	public float jumpSlowTime;
	private float currentVertical;

	public KeyCode runKey;
	public float slowTime;
	public float runTime;
	private float currentHorizontal;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		checkGround();
		checkInput ();
	}

	void checkInput(){
		float horizontalInput = Input.GetAxisRaw ("Horizontal");
		if (horizontalInput > 0) {
			if (Input.GetKey (runKey)) {
				move (2f);
			} else {
				move (1f);
			}
		} else if (horizontalInput < 0) {
			if (Input.GetKey (runKey)) {
				move (-2f);
			} else {
				move (-1f);
			}
		} else {
			rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0f, ref currentHorizontal, slowTime), rb.velocity.y);
		}
		if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0 && !jumping) {
			jumping = true;
			rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			jumpCount--;
		}

		if (Input.GetKeyUp (KeyCode.Space) && jumping) {
			rb.velocity = new Vector2(rb.velocity.x, Mathf.SmoothDamp(rb.velocity.y, 0f, ref currentVertical, jumpSlowTime));
		}

		if (jumping && rb.velocity.y <= 0f) {
			jumping = false;
		}
	}

	void move(float speed){
		rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, speed * maxSpeed, ref currentHorizontal, slowTime), rb.velocity.y);
	}

	void resetJumps(){
		jumpCount = maxJumps;
	}

	void checkGround(){
		Collider2D[] colliderList = Physics2D.OverlapBoxAll(
			new Vector2 (transform.position.x, transform.position.y - 1f), new Vector2 (2f, 0.2f), 0f);
		foreach (Collider2D c in colliderList) {
			if (c.CompareTag ("Floor") && !jumping) {
				resetJumps ();
			}
		}
	}
}
