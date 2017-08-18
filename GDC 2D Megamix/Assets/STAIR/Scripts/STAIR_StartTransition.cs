using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class STAIR_StartTransition : MonoBehaviour {

	public int doorValue;
	public float speed;
	public Image fade;
	private GameObject player;
	private Rigidbody2D rb;
	public Vector2 startPosition;
	public Vector2 endPosition;

	// Use this for initialization
	void Start () {
		GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		if (allPlayers.Length > 1) {
			for (int i = 0; i < allPlayers.Length; i++) {
				if (allPlayers [i].GetComponent<STAIR_Chip> ().getDoorValue () < 0) {
					Destroy (allPlayers [i]);
				}
			}
		}
		player = GameObject.FindGameObjectWithTag ("Player");
		if (doorValue != player.GetComponent<STAIR_Chip> ().getDoorValue ()) {
			Destroy (this.gameObject);
		} else {
			rb = player.GetComponent<Rigidbody2D> ();
			player.transform.position = startPosition;
			fade.color = new Color (0f, 0f, 0f, 0f);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		rb.velocity = new Vector2((endPosition.x - startPosition.x) * speed, (endPosition.y - startPosition.y) * speed);
		fade.color = new Color(0f, 0f, 0f, 1f - Vector3.Distance(player.transform.position, startPosition) / Vector3.Distance(endPosition, startPosition));
		if (fade.color.a <= 0f) {
			player.GetComponent<STAIR_Chip> ().setCanMove (true);
			Destroy (this.gameObject);
		}
	}
}
