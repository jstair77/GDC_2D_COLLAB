using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_TriggerGlobal : MonoBehaviour {

	public string compareTag;
	public int index;
	public int value;

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag (compareTag)) {
			STAIR_GlobalValues.setGlobals (index, value);
		}
	}
}
