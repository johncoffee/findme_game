﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainStuff : MonoBehaviour {

	public Timers timers;

	// Use this for initialization
	void Start () {
		timers = GetComponent<Timers>();


		CheckinRoom(1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CheckinRoom (int roomID) {

		switch (roomID) {
			case 1:
				timers.CreateTimer (2f, () => {
					Debug.Log("Button 1 has timeout");
					ExecuteEvents.Execute<ITimeupEvent>(gameObject, null, (x,y) => x.Timeup() );			
				});
				break;	

		}
	}


	
}
