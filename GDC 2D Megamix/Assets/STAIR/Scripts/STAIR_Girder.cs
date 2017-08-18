using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_Girder : MonoBehaviour {

	public float moveTime;
	public float waitTime;
	public GameObject leftPulley;
	public GameObject rightPulley;
	public float pulleySpeedEffector;
	public Vector2 girderLineOffset;
	public Vector2 pulleyLineOffset;
	public List<Vector2> positionList;
	private Vector2 nextPosition;
	private int state;
	private LineRenderer leftLR;
	private LineRenderer rightLR;
	private Animator leftAnim;
	private Animator rightAnim;
	private float velocity;
	private Vector3 previous;
	private bool canStartCoroutine;
	private float lerp;

	// Use this for initialization
	void Start () {
		leftLR = leftPulley.GetComponent<LineRenderer> ();
		rightLR = rightPulley.GetComponent<LineRenderer> ();
		leftAnim = leftPulley.GetComponent<Animator> ();
		rightAnim = rightPulley.GetComponent<Animator> ();
		nextPosition = positionList [0];
		state = 0;
		canStartCoroutine = true;
		leftLR.SetPosition (0, leftPulley.transform.position + (Vector3) pulleyLineOffset);
		rightLR.SetPosition (0, rightPulley.transform.position + new Vector3 (-pulleyLineOffset.x, pulleyLineOffset.y, 0f));
		previous = transform.position;
		SpriteRenderer leftSR = leftPulley.GetComponent<SpriteRenderer> ();
		leftLR.sortingLayerName = leftSR.sortingLayerName;
		leftLR.sortingOrder = leftSR.sortingOrder - 1;
		SpriteRenderer rightSR = leftPulley.GetComponent<SpriteRenderer> ();
		rightLR.sortingLayerName = rightSR.sortingLayerName;
		rightLR.sortingOrder = rightSR.sortingOrder - 1;
	}
	
	// Update is called once per frame
	void Update () {
		rightLR.SetPosition (1, transform.position + (Vector3) girderLineOffset);
		leftLR.SetPosition (1, transform.position + new Vector3 (-girderLineOffset.x, girderLineOffset.y, 0f));
		leftAnim.SetFloat ("Direction", -velocity * pulleySpeedEffector);
		rightAnim.SetFloat ("Direction", velocity * pulleySpeedEffector);

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

		velocity = (transform.position.y - previous.y) / Time.deltaTime;
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
