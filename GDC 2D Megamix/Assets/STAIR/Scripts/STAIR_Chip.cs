using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_Chip : MonoBehaviour {

	public STAIR_Microchips activeChips;
	public STAIR_ChipMovement movementValues;
	public STAIR_ChipAnimation animationValues;
	public STAIR_ChipAction actionValues;
	private bool canMove;
	private int doorValue = -1;

	// Use this for initialization
	void Start () {
		movementValues.player = this.gameObject;
		movementValues.setPlayer();
		animationValues.player = this.gameObject;
		animationValues.setPlayer ();
		actionValues.player = this.gameObject;
		actionValues.setPlayer ();
		animationValues.canStandUp = true;
		movementValues.canStandUp = true;
		movementValues.canJumpButton = true;
		movementValues.moveHorizontalNext = 0f;
		setLaserPosition ("default");
		updateChips ();
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (canMove) {
			movementValues.checkInputs ();
		} else {
			movementValues.resetValues ();
		}
		if (activeChips.changed) {
			updateChips ();
			activeChips.changed = false;
		}
		animationValues.updateValues (movementValues.grounded, movementValues.jumping, movementValues.maxSpeed, movementValues.speedMultiplier);
		actionValues.updateActions (animationValues.anim.GetCurrentAnimatorStateInfo(0).IsName("Rolling"),
			(animationValues.anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping") || animationValues.anim.GetCurrentAnimatorStateInfo(0).IsName("Air Jump")),
			animationValues.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
		if (animationValues.checkRollNext) {
			animationValues.checkRollNext = false;
			StartCoroutine(animationValues.RollDelay (movementValues.maxSpeed));
		}
	}

	void FixedUpdate(){
		movementValues.canStandUp = animationValues.canStandUp;
		movementValues.checkGround ();
		movementValues.checkJump ();
		if (movementValues.jumpNext) {
			movementValues.jumpNext = false;
			if (animationValues.anim.GetBool ("IsRolling") == true) {
				movementValues.jump (-0.65f);
			} else {
				movementValues.jump (0f);
			}
		}
		if (movementValues.dropNext) {
			movementValues.drop ();
		}
		movementValues.moveHorizontal (movementValues.moveHorizontalNext);
	}

	void updateChips(){
		if (activeChips.tripleJump) {
			movementValues.maxJumps = 3;
		} else if (activeChips.doubleJump) {
			movementValues.maxJumps = 2;
		} else if (activeChips.jump) {
			movementValues.maxJumps = 1;
		} else {
			movementValues.maxJumps = 0;
		}
		movementValues.canRun = activeChips.run;
		movementValues.hasOS = activeChips.mainOS;
		animationValues.canRoll = activeChips.roll;
		actionValues.canShoot = activeChips.laser;
	}

	public void setLaserPosition(string value){
		actionValues.laserPositionHelper (value);
	}

	public void setCanMove(bool move){
		canMove = move;
	}

	public bool getCanMove(){
		return canMove;
	}

	public void setDoorValue(int value){
		doorValue = value;
	}

	public int getDoorValue(){
		return doorValue;
	}
}

//keeps track of which microchips have been acquired
[Serializable] public class STAIR_Microchips {
	public bool mainOS;
	public bool jump;
	public bool run;
	public bool roll;
	public bool doubleJump;
	public bool laser;
	public bool tripleJump;

	[HideInInspector]
	public bool changed;

	public void addChip(int chipValue){
		changed = true;
		switch (chipValue) {
		case 0:
			mainOS = true;
			break;
		case 1:
			jump = true;
			break;
		case 2:
			run = true;
			break;
		case 3:
			roll = true;
			break;
		case 4:
			doubleJump = true;
			break;
		case 5:
			laser = true;
			break;
		case 6:
			tripleJump = true;
			break;
		}
	}
}

[Serializable] public class STAIR_ChipMovement {

	public float maxSpeed;
	public float jumpSpeed;
	public float speedMultiplier;
	public float jumpSlowTime;
	public float moveTime;

	private int jumpCount;

	[HideInInspector]
	public bool canJumpButton;

	[HideInInspector]
	public bool jumping;

	[HideInInspector]
	public bool grounded;

	[HideInInspector]
	public bool canStandUp;

	[HideInInspector]
	public float moveHorizontalNext;

	[HideInInspector]
	public bool dropNext;

	[HideInInspector]
	public bool jumpNext;

	[HideInInspector]
	public int maxJumps;

	[HideInInspector]
	public bool canRun;

	[HideInInspector]
	public bool hasOS;

	[HideInInspector]
	public GameObject player;

	public GameObject jumpDustCloud;
	private GameObject jumpDustCloudCopy;

	public GameObject airJumpCloud;
	private GameObject airJumpCloudCopy;
	public float airCloudOffset;


	private Rigidbody2D rb;


	void Start(){
		canJumpButton = true;
		moveHorizontalNext = 0f;
	}

	public void setPlayer(){
		if (!rb) {
			rb = player.GetComponent<Rigidbody2D> ();
		}
	}

	public void checkGround(){
		if (player) {
			Collider2D[] colliderList = Physics2D.OverlapBoxAll (
				new Vector2 (player.transform.position.x, player.transform.position.y - 1.85f), new Vector2 (1.9f, 0.25f), 0f);
			foreach (Collider2D c in colliderList) {
				if (c.CompareTag ("Floor") && !jumping) {
					resetJumps ();
					grounded = true;
					return;
				}
			}
			grounded = false;
		}
	}

	public void checkInputs(){
		float horizontalInput = Input.GetAxis ("Horizontal");
		if (horizontalInput != 0) {
			if (Input.GetButton("Fire2") && canRun) {
				moveHorizontalNext = speedMultiplier * horizontalInput;
			} else if (!hasOS) {
				moveHorizontalNext = 0.2f * horizontalInput;
			} else {
				moveHorizontalNext = 1f * horizontalInput;
			}
		} else {
			moveHorizontalNext = 0f;
		}

		if (!grounded && jumpCount == maxJumps) {
			jumpCount--;
		}

		if (Input.GetButton ("Jump") && jumpCount > 0 && !jumping && canStandUp && canJumpButton) {
			jumpNext = true;
		}

		if (!Input.GetButton ("Jump")) {
			canJumpButton = true;
			if (jumping) {
				dropNext = true;
			}
		}
	}

	public void checkJump(){
		if (rb) {
			if (rb.velocity.y <= 0f) {
				jumping = false;
				dropNext = false;
			}
		}
	}

	public void jump(float rollOffset){
		if (grounded || maxJumps == 1) {
			if (Mathf.Abs (rb.velocity.x) < maxSpeed * 0.8) {
				jumpDustCloudCopy = GameObject.Instantiate (jumpDustCloud, player.transform.position + new Vector3 (-1f, -0.75f, 0f), Quaternion.identity);
				jumpDustCloudCopy.GetComponent<SpriteRenderer> ().flipX = true;
				jumpDustCloudCopy = GameObject.Instantiate (jumpDustCloud, player.transform.position + new Vector3 (1f, -0.75f, 0f), Quaternion.identity);
			} else if (rb.velocity.x > 0) {
				jumpDustCloudCopy = GameObject.Instantiate (jumpDustCloud, player.transform.position + new Vector3 (-1f, -0.75f, 0f), Quaternion.identity);
				jumpDustCloudCopy.GetComponent<SpriteRenderer> ().flipX = true;
			} else {
				jumpDustCloudCopy = GameObject.Instantiate (jumpDustCloud, player.transform.position + new Vector3 (1f, -0.75f, 0f), Quaternion.identity);
			}
		} else {
			airJumpCloudCopy = GameObject.Instantiate (airJumpCloud, player.transform.position + new Vector3 (airCloudOffset, airCloudOffset + rollOffset, 0f), Quaternion.identity);
			airJumpCloudCopy.transform.SetParent (player.transform);
			airJumpCloudCopy = GameObject.Instantiate (airJumpCloud, player.transform.position + new Vector3 (-airCloudOffset, airCloudOffset + rollOffset, 0f), Quaternion.Euler(0f, 0f, 90f));
			airJumpCloudCopy.transform.SetParent (player.transform);
			airJumpCloudCopy = GameObject.Instantiate (airJumpCloud, player.transform.position + new Vector3 (-airCloudOffset, -airCloudOffset + rollOffset, 0f), Quaternion.Euler(0f, 0f, 180f));
			airJumpCloudCopy.transform.SetParent (player.transform);
			airJumpCloudCopy = GameObject.Instantiate (airJumpCloud, player.transform.position + new Vector3 (airCloudOffset, -airCloudOffset + rollOffset, 0f), Quaternion.Euler(0f, 0f, 270f));
			airJumpCloudCopy.transform.SetParent (player.transform);
		}
		jumpCount--;
		canJumpButton = false;
		jumping = true;
		rb.velocity = new Vector2 (rb.velocity.x, jumpSpeed);
	}

	public void moveHorizontal(float speed){
		if (rb) {
			float currentHorizontal = 0f;
			rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, speed * maxSpeed, ref currentHorizontal, moveTime), rb.velocity.y);
		}
	}

	public void drop(){
		float currentVertical = 0f;
		rb.velocity = new Vector2(rb.velocity.x, Mathf.SmoothDamp(rb.velocity.y, 0f, ref currentVertical, jumpSlowTime));
	}

	private void resetJumps(){
		jumpCount = maxJumps;
	}

	public void resetValues(){
		moveHorizontalNext = 0f;
	}
}

[Serializable] public class STAIR_ChipAnimation {

	public float walkSpeedEffector;
	public float runSpeedEffector;
	public float rollSpeedEffector;
	public float rollDelay;
	public GameObject rollDustCloud;
	private GameObject rollDustCloudCopy;
	public Vector2 rollDustOffset;

	[HideInInspector]
	public bool canRoll;

	[HideInInspector]
	public bool canStandUp;

	[HideInInspector]
	public bool checkRollNext;

	[HideInInspector]
	public bool grounded;

	private bool startedRollCheck;

	[HideInInspector]
	public GameObject player;

	[HideInInspector]
	public Animator anim;

	private BoxCollider2D hitbox;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	public void updateValues(bool tempGrounded, bool jumping, float maxSpeed, float speedMultiplier){
		grounded = tempGrounded;
		anim.SetBool ("Grounded", grounded);
		anim.SetBool ("Jumping", jumping);
		if (Mathf.Abs (rb.velocity.x) < maxSpeed * 0.1 && canStandUp) {
			anim.SetBool ("IsMoving", false);
			anim.SetBool ("IsRunning", false);
			anim.SetBool ("IsRolling", false);
			hitbox.offset = new Vector2 (0f, -0.3f);
			hitbox.size = new Vector2 (2f, 2.65f);
		} else if (Mathf.Abs (rb.velocity.x) < maxSpeed * 1.1 && canStandUp) {
			anim.SetBool ("IsMoving", true);
			anim.SetBool ("IsRunning", false);
			anim.SetBool ("IsRolling", false);
			hitbox.offset = new Vector2 (0f, -0.3f);
			hitbox.size = new Vector2 (2f, 2.65f);
			anim.SetFloat ("WalkSpeed", Mathf.Abs (rb.velocity.x) * walkSpeedEffector);
		} else if (Mathf.Abs (rb.velocity.x) <= maxSpeed * speedMultiplier && canStandUp) {
			anim.SetBool ("IsMoving", true);
			anim.SetBool ("IsRunning", true);
			hitbox.offset = new Vector2 (0f, -0.3f);
			hitbox.size = new Vector2 (2f, 2.65f);
			anim.SetFloat ("RunSpeed", Mathf.Abs (rb.velocity.x) * runSpeedEffector);
		}
		if (anim.GetBool ("IsRolling") == true) {
			startedRollCheck = false;
			anim.SetFloat ("RollSpeed", Mathf.Abs (rb.velocity.x) * rollSpeedEffector);
			hitbox.offset = new Vector2 (0f, -0.625f);
			hitbox.size = new Vector2 (2f, 2f);
			Collider2D[] rollCeiling = Physics2D.OverlapBoxAll (
				new Vector2 (player.transform.position.x, player.transform.position.y + 0.5f),
				new Vector2 (2f, 0.2f), 0f);
			canStandUp = true;
			foreach (Collider2D c in rollCeiling) {
				if (c.CompareTag ("Floor")) {
					canStandUp = false;
					break;
				}
				canStandUp = true;
			}
		}
		if (!startedRollCheck && Mathf.Abs (rb.velocity.x) > maxSpeed * speedMultiplier * 0.9 && anim.GetBool("IsRolling") == false && grounded && canRoll) {
			checkRollNext = true;
		}
		if (Mathf.Abs (rb.velocity.x) < maxSpeed * 3f * 0.9f && canStandUp) {
			anim.SetBool ("IsRolling", false);
		}
		if (rb.velocity.x > 0.1f * maxSpeed) {
			sr.flipX = false;
		} else if (rb.velocity.x < -0.1f * maxSpeed) {
			sr.flipX = true;
		}
	}

	public void setPlayer(){
		if (!anim) {
			anim = player.GetComponent<Animator> ();
		}
		if (!hitbox) {
			hitbox = player.GetComponent<BoxCollider2D> ();
		}
		if (!sr) {
			sr = player.GetComponent<SpriteRenderer> ();
		}
		if (!rb) {
			rb = player.GetComponent<Rigidbody2D> ();
		}
	}

	public IEnumerator RollDelay(float maxSpeed){
		startedRollCheck = true;
		float i = rollDelay;
		while (anim.GetBool ("IsRolling") == false && i > 0 && Mathf.Abs(rb.velocity.x) > maxSpeed * 3f * 0.9f && grounded) {
			i -= Time.deltaTime;
			yield return null;
		}
		if (i <= 0) {
			anim.SetBool ("IsRolling", true);
			if (rb.velocity.x > 0) {
				rollDustCloudCopy = GameObject.Instantiate (rollDustCloud, player.transform.position + new Vector3 (-rollDustOffset.x, rollDustOffset.y, 0f), Quaternion.identity);
				rollDustCloudCopy.GetComponent<SpriteRenderer> ().flipX = true;
			} else {
				rollDustCloudCopy = GameObject.Instantiate (rollDustCloud, player.transform.position + new Vector3 (rollDustOffset.x, rollDustOffset.y, 0f), Quaternion.identity);
			}
		} else {
			startedRollCheck = false;
		}
	}
}


[Serializable] public class STAIR_ChipAction {

	public GameObject laserGraphic;
	public GameObject laserStart;
	public GameObject laserEnd;

	[HideInInspector]
	public bool canShoot;

	[HideInInspector]
	public GameObject player;

	[HideInInspector]
	public Vector2 laserDirection;

	[HideInInspector]
	public Vector3 laserOffset;

	private Animator laserAnim;
	private SpriteRenderer laserSr;
	private LineRenderer laserBeam;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	public void updateActions(bool rolling, bool jumping, bool idle){
		if (!rolling && !jumping && !idle) {
			laserPositionHelper ("default");
		}
		if (player) {
			if (Input.GetButton ("Fire3") && canShoot) {
				laserGraphic.SetActive (true);
				RaycastHit2D hit = Physics2D.Raycast (player.transform.position + laserOffset, laserDirection, 1000f, ~(1 << 8));
				laserBeam.SetPosition (0, player.transform.position + laserOffset);
				laserStart.transform.position = player.transform.position + laserOffset;
				laserBeam.sortingLayerName = "Just Behind";
				if (hit) {
					laserBeam.SetPosition (1, hit.point);
					laserEnd.SetActive (true);
					laserEnd.transform.position = hit.point;
				} else {
					laserBeam.SetPosition (1, laserBeam.GetPosition (0) + 1000f * new Vector3(laserDirection.x, laserDirection.y, 0));
					laserEnd.SetActive (false);
				}
				Debug.DrawLine (player.transform.position, hit.point);
			} else {
				laserGraphic.SetActive (false);
			}
		}
	}

	public void laserPositionHelper(string value){
		Vector2 direction = new Vector2(0, 0);
		if (value.Contains ("default")) {
			if (sr.flipX) {
				direction = Vector2.left;
			} else {
				direction = Vector2.right;
			}
		} else if (value.Contains ("DR")) {
			if (sr.flipX) {
				direction = Vector2.left + Vector2.down;
			} else {
				direction = Vector2.right + Vector2.down;
			}
		} else if (value.Contains ("DL")) {
			if (sr.flipX) {
				direction = Vector2.right + Vector2.down;
			} else {
				direction = Vector2.left + Vector2.down;
			}
		} else if (value.Contains ("UR")) {
			if (sr.flipX) {
				direction = Vector2.left + Vector2.up;
			} else {
				direction = Vector2.right + Vector2.up;
			}
		} else if (value.Contains ("UL")) {
			if (sr.flipX) {
				direction = Vector2.right + Vector2.up;
			} else {
				direction = Vector2.left + Vector2.up;
			}
		} else if (value.Contains ("R")) {
			if (sr.flipX) {
				direction = Vector2.left;
			} else {
				direction = Vector2.right;
			}
		} else if (value.Contains ("L")) {
			if (sr.flipX) {
				direction = Vector2.right;
			} else {
				direction = Vector2.left;
			}
		} else if (value.Contains ("D")) {
			direction = Vector2.down;
		} else if (value.Contains ("U")) {
			direction = Vector2.up;
		}
		direction.Normalize ();
		laserDirection = direction;
		if (value.Contains ("roll")) {
			laserOffset = direction - new Vector2 (0f, 0.65f);
		} else if (value.Contains ("idle")) {
			laserOffset = direction - new Vector2 (0f, 0.325f);
		} else {
			laserOffset = direction;
		}
	}

	public void setPlayer(){
		if (!laserAnim && laserGraphic) {
			laserAnim = laserGraphic.GetComponent<Animator> ();
		}
		if (!laserSr && laserGraphic) {
			laserSr = laserGraphic.GetComponent<SpriteRenderer> ();
		}
		if (!laserBeam && laserGraphic) {
			laserBeam = laserGraphic.GetComponent<LineRenderer> ();
		}
		if (!sr) {
			sr = player.GetComponent<SpriteRenderer> ();
		}
		if (!rb) {
			rb = player.GetComponent<Rigidbody2D> ();
		}
	}
}