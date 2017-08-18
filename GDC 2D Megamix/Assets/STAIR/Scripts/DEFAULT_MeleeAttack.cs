using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFAULT_MeleeAttack : MonoBehaviour {

	public GameObject hitBox;
	private GameObject hitBoxCopy;
	public List<Attack> attackList;

	private bool canAttack;
	private bool canQueue;
	private int currentCombo;

	void Start(){
		canAttack = true;
		canQueue = false;
	}

	// Update is called once per frame
	void Update () {
		foreach (Attack a in attackList) {
			if ((canAttack || canQueue) && Input.GetKeyDown(a.control)
				&& Mathf.Abs(Input.GetAxis("Horizontal")) >= a.directional.x
				&& Input.GetAxis("Vertical") >= a.directional.y) {
				StartCoroutine (attackDelay (a));
			}
		}
	}

	private IEnumerator attackDelay(Attack a){
		while (!canAttack) {
			yield return null;
		}
		canAttack = false;
		for (int i = 0; i < a.hitList.Count; i++) {
			Hit h = a.hitList [i];
			if (i < a.hitList.Count - 1) {
					
				hitBoxCopy = Instantiate (hitBox, transform.position, Quaternion.identity, transform);
				hitBoxCopy.transform.localPosition = h.offset;
				hitBoxCopy.transform.localScale = new Vector3 (h.size.x, h.size.y, 1);
				//hitBoxCopy.getComponent<DEFAULT_Hitbox>().setValues(h.damage, h.knockback);

				yield return new WaitForSeconds (h.length);
				Destroy (hitBoxCopy);
			} else {
				if (a.hitList [i].length < 0.2f) {

					hitBoxCopy = Instantiate (hitBox, transform.position, Quaternion.identity, transform);
					hitBoxCopy.transform.localPosition = h.offset;
					hitBoxCopy.transform.localScale = new Vector3 (h.size.x, h.size.y, 1);
					//hitBoxCopy.getComponent<DEFAULT_Hitbox>().setValues(h.damage, h.knockback);

					canQueue = true;
					yield return new WaitForSeconds (h.length);
					Destroy (hitBoxCopy);
				} else {
						
					hitBoxCopy = Instantiate (hitBox, transform.position, Quaternion.identity, transform);
					hitBoxCopy.transform.localPosition = h.offset;
					hitBoxCopy.transform.localScale = new Vector3 (h.size.x, h.size.y, 1);
					//hitBoxCopy.getComponent<DEFAULT_Hitbox>().setValues(h.damage, h.knockback);

					yield return new WaitForSeconds (h.length - 0.2f);
					canQueue = true;
					yield return new WaitForSeconds (0.2f);
					Destroy (hitBoxCopy);
				}
				canQueue = false;
			}
		}
		canAttack = true;
	}
}

[Serializable] public class Attack {
	public int combo;
	public KeyCode control;
	public Vector2 directional;
	public List<Hit> hitList;
}

[Serializable] public class Hit {
	public float damage;
	public float length;
	public Vector2 knockback;
	public Vector2 size;
	public Vector2 offset;
}
