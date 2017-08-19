using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_Positioner : MonoBehaviour {

	public int index;
	public List<Vector2> positions;

	void Update(){
		transform.position = positions [STAIR_GlobalValues.getGlobals(index)];
	}
}
