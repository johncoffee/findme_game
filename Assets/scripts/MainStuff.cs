using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainStuff : MonoBehaviour
{

	public Timers timers;
	bool bodyPartWasFound = false;
	public HashSet<int> visitedRooms = new HashSet<int> ();
	public GameObject ui;
	public int activeRoomID = -1;

	// Use this for initialization
	void Start ()
	{
		timers = GetComponent<Timers> ();


//		CheckinRoom(1);
	}
	

	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Found ()
	{
		bodyPartWasFound = true;
		visitedRooms.Add (activeRoomID);
		if (visitedRooms.Count >= 4) {
			ui.SendMessage ("OnAllRoomsFinished");
		} else {
			ui.SendMessage ("OnFound");
		}
		timers.StopTimer ();
		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Found ());	


	}

	public void CheckinRoom (int roomID)
	{
		bodyPartWasFound = false;

		activeRoomID = roomID;

		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.CheckedIn (roomID));	
		switch (roomID) {
		case 1:
			timers.CreateTimer (30f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
				}					
				
			});
			break;	
		case 2:
			timers.CreateTimer (45f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
				}					
				
			});
			break;	
		case 3:
			timers.CreateTimer (60f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
				}					
				
			});
			break;	
		case 4:
			timers.CreateTimer (90f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
				}					
				
			});
			break;	
		}
	}


	
}
