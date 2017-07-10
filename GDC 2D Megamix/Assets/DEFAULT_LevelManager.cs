using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEFAULT_LevelManager : MonoBehaviour {

	public int lives;
	private int currentLives;
	private int currentScene = 0;

	public float respawnDelay;
	public float endDelay;
	public float gameOverDelay;
	public List<string> scenes;
	public string gameOverScene;

	void Start(){
		DontDestroyOnLoad (this.gameObject);
		currentLives = lives;
	}

	public void die(){
		Debug.Log ("die called, lives = " + currentLives);
		if (currentLives > 0) {
			currentLives--;
			StartCoroutine(loadDelay (respawnDelay, scenes[currentScene]));
		} else {
			currentLives = lives;
			StartCoroutine(loadDelay (gameOverDelay, gameOverScene));
		}
	}

	public void endLevel(){
		currentScene++;
		if (currentScene < scenes.Count - 1) {
			StartCoroutine (loadDelay (endDelay, scenes [currentScene]));
		}
	}

	private IEnumerator loadDelay(float delay, string scene){
		Debug.Log ("loadDelay called, delay = " + delay + ", scene = " + scene);
		yield return new WaitForSeconds (delay);
		SceneManager.LoadScene(scene);
		if (currentScene == 0 || currentScene == scenes.Count - 1) {
			Destroy (this.gameObject);
		}
	}
}
