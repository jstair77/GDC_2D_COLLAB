using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAIR_Microchip : MonoBehaviour {

	private Animator anim;
	private GameObject player;
	private bool collected;

	public int chipValue;
	public float delayTime;
	public float disappearTime;

	// Use this for initialization
	void Start () {
		if (STAIR_GlobalValues.getGlobals(chipValue) == 1) {
			Destroy (this.gameObject);
		}
		anim = GetComponent<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Awake(){

	}

	void OnTriggerEnter2D (Collider2D other){
		if (other.CompareTag ("Player") && !collected) {
			STAIR_GlobalValues.globals [chipValue].value = 1;
			anim.SetInteger ("ChipValue", chipValue);
			anim.SetTrigger ("Flip");
			player.GetComponent<STAIR_Chip> ().activeChips.addChip (chipValue);
			collected = true;
			StartCoroutine (disappear (disappearTime, transform.localScale));
		}
	}

	IEnumerator disappear(float tempTime, Vector3 size){
		yield return new WaitForSeconds (delayTime);
		while (transform.localScale.x > 0f) {
			tempTime -= Time.deltaTime;
			transform.localScale = new Vector3 (size.x * tempTime / disappearTime, size.y * tempTime / disappearTime);
			yield return null;
		}
		Destroy (this.gameObject);
	}
}
