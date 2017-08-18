using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_TriggerMove : MonoBehaviour {

	public string Tag;
	public Vector2 newPosition;
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag (Tag)) {
			transform.position = newPosition;
		}
	}
}
