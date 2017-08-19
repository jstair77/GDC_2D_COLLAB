using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_GlobalValues : MonoBehaviour {

	public List<values> _globals;
	public static List<values> globals;
	private static bool created = false;

	// Use this for initialization
	void Awake () {
		if (created) {
			Destroy (this.gameObject);
		} else {
			DontDestroyOnLoad (this.gameObject);
			created = true;
			globals = _globals;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static int getGlobals(int index){
		try {
			return globals [index].value;
		} catch (Exception e){
			Debug.LogException (e);
			return 404;
		}
	}

	public static void setGlobals(int index, int value){
		try {
			globals [index].value = value;
		} catch (Exception e){
			Debug.LogException (e);
		}
	}
}

[Serializable] public class values {
	public string name;
	public int value;
}
