using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_MovingPlatform : MonoBehaviour {

	public float moveTime;
	public float waitTime;
	public float moveSpeedAffector;
	public List<Vector2> positionList;
	private Vector2 nextPosition;
	private int state;
	private Animator anim;
	private float velocity;
	private Vector3 previous;
	private bool canStartCoroutine;
	private float lerp;

	// Use this for initialization
	void Start () {
		try {
			anim = GetComponent<Animator> ();
		} catch (Exception e){
			Debug.LogException (e);
		}
		nextPosition = positionList [0];
		state = 0;
		canStartCoroutine = true;
		previous = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (anim) {
			anim.SetFloat ("Speed", -velocity * moveSpeedAffector);
		}
		if (state % 2 == 0) {
			if (canStartCoroutine) {
				canStartCoroutine = false;
				nextPosition = positionList [state / 2];
				StartCoroutine (delay (waitTime));
			}
		} else {
			lerp += moveTime * Time.deltaTime;
			transform.position = Vector3.Lerp (transform.position, nextPosition, lerp);
			if (Mathf.Abs(transform.position.x - nextPosition.x) <= 0.1f && Mathf.Abs(transform.position.y - nextPosition.y) <= 0.1f) {
				if (state >= positionList.Count * 2 - 1) {
					state = 0;
				} else {
					state++;
				}
				lerp = 0;
			}
		}

		velocity = Vector3.Distance(transform.position, previous) / Time.deltaTime;
		if (transform.position.y - previous.y < 0) {
			velocity = -velocity;
		}
		previous = transform.position;
	}

	IEnumerator delay(float t){
		yield return new WaitForSeconds (t);
		canStartCoroutine = true;
		if (state >= positionList.Count * 2 - 1) {
			state = 0;
		} else {
			state++;
		}
	}
}
