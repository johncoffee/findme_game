using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainStuff : MonoBehaviour
{

	public Timers timers;
	bool bodyPartWasFound = false;
	public HashSet<int> visitedRooms = new HashSet<int> ();
	public UISwitcher ui;
	public int activeRoomID = -1;

	public SocketInterface socketInterface;

	private bool isInRoom = false;

	private bool isLocked = false;


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
		isInRoom = false;
		bodyPartWasFound = true;
		visitedRooms.Add (activeRoomID);
		if (visitedRooms.Count >= 4) {
			ui.SendMessage ("OnAllRoomsFinished");
			socketInterface.SendYouLost();

		} else {
			ui.SendMessage ("OnFound");
		}
		timers.StopTimer ();
		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Found ());	


	}



	public void CheckinRoom (int roomID)
	{
		if (isLocked) {
			return;
		}

		ui.ShowSeekUI ();
		bodyPartWasFound = false;

		activeRoomID = roomID;

		
		isInRoom = true;
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.CheckedIn (roomID));	
		/*switch (roomID) {
		case 1:
			timers.CreateTimer (30f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
					
					isInRoom = false;
				}					
				
			});
			break;	
		case 2:
			timers.CreateTimer (45f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
					isInRoom = false;
				}					
				
			});
			break;	
		case 3:
			timers.CreateTimer (60f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
					isInRoom = false;
				}					
				
			});
			break;	
		case 4:
			timers.CreateTimer (90f, () => {					
				if (!bodyPartWasFound) {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
					
					ui.SendMessage ("OnTimeup");
					isInRoom = false;
				}					
				
			});
			break;	
		}*/
	}


	public void OthersHaveFoundYours() {
		if (isInRoom) {
			isLocked = true;
			ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
			ui.SendMessage ("OnTimeup");
			isInRoom = false;
		}
	}

	public void Unlock() {
		isLocked = false;
		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
		ui.ShowRoomsUI ();
	}

	public void YouLost() {
		ui.ShowLostUI ();
	}
	
}
