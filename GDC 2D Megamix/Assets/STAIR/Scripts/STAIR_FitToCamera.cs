using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_FitToCamera : MonoBehaviour {

	private Camera cam;
	public float helper;

	// Use this for initialization
	void Start () {
		cam = transform.parent.GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (cam.pixelHeight / helper, cam.pixelHeight / helper, 1);
	}
}
