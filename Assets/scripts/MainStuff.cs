using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainStuff : MonoBehaviour {

	public Timers timers;
	bool bodyPartWasFound= false;

	public HashSet<int> visitedRooms = new HashSet<int>();

	public int activeRoomID = -1;

	// Use this for initialization
	void Start () {
		timers = GetComponent<Timers>();


		CheckinRoom(1);
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	public void Found () {
		bodyPartWasFound = true;
		visitedRooms.Add(activeRoomID);
	}


	public void CheckinRoom (int roomID) {
		bodyPartWasFound = false;

		activeRoomID = roomID;

		switch (roomID) {
			case 1:
				timers.CreateTimer (2f, () => {					
					if (!bodyPartWasFound) {
						ExecuteEvents.Execute<ITimeupEvent>(gameObject, null, (x,y) => x.Timeup() );			
					}
				});
				break;	

		}
	}


	
}
