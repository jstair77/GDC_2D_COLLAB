using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFAULT_CameraFollow : MonoBehaviour {

	public Vector2 setMin;
	public Vector2 setMax;
	public float smoothing;
	private GameObject player;
	Vector3 refVector;
	private Camera cam;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.SmoothDamp (transform.position, player.transform.position, ref refVector, smoothing);
		transform.position = new Vector3 (transform.position.x, transform.position.y, -15f);

		Vector3 camWorldMin = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3 camWorldMax = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 0f));
		if (camWorldMin.x < setMin.x) {
			transform.position = new Vector3 (setMin.x + (camWorldMax.x - camWorldMin.x) / 2, transform.position.y, transform.position.z);
		} else if (camWorldMax.x > setMax.x) {
			transform.position = new Vector3 (setMax.x - (camWorldMax.x - camWorldMin.x) / 2, transform.position.y, transform.position.z);
		}
		if (camWorldMin.y < setMin.y) {
			transform.position = new Vector3 (transform.position.x, setMin.y + (camWorldMax.y - camWorldMin.y) / 2, transform.position.z);
		} else if (camWorldMax.y > setMax.y) {
			transform.position = new Vector3 (transform.position.x, setMax.y - (camWorldMax.y - camWorldMin.y) / 2, transform.position.z);
		}
	}
}
