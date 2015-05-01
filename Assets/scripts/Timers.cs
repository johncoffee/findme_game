using UnityEngine;
using System.Collections;

public class Timers : MonoBehaviour {

	float time;

	public delegate void OnTimer();

	private Coroutine activeTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StopTimer() {
		if (activeTimer != null) {
			StopCoroutine (activeTimer);
		}
	}

	public void CreateTimer (float seconds, OnTimer callback) {
		activeTimer = StartCoroutine(createTimer(seconds, callback));
	}

	IEnumerator createTimer(float seconds, OnTimer callback) {
		yield return new WaitForSeconds(seconds);
		if(callback != null) 
			callback();
	}


}
