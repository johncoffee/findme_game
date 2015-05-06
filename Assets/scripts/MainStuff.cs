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


	private enum ScannerState {
		None,
		Rooms,
		Treasure,
		Haunted,
		Finished
	}

	private ScannerState scannerState = ScannerState.None;
	// Use this for initialization
	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// Initialize EasyCodeScanner
		EasyCodeScanner.Initialize();
		timers = GetComponent<Timers> ();
		
		//Register on Actions
		EasyCodeScanner.OnScannerMessage += onScannerMessage;
		EasyCodeScanner.OnScannerEvent += onScannerEvent;
		EasyCodeScanner.OnDecoderMessage += onDecoderMessage;

		
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
			scannerState = ScannerState.Finished;
			ui.SendMessage ("OnTimeup");

		} else {
			ui.SendMessage ("OnFound");
		}
		timers.StopTimer ();
		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Found ());	


	}

	public void OpenScanner() {
		EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
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

		scannerState = ScannerState.Treasure;
		
		//EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);


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
			scannerState = ScannerState.Haunted;
			ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.Timeup ());			
			ui.SendMessage ("OnTimeup");
			isInRoom = false;
		}
	}

	public void Unlock() {
		/*isLocked = false;
		
		ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
		ui.ShowRoomsUI ();*/

		EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);

	}

	public void YouLost() {
		ui.ShowLostUI ();
	}


	public void CheckIn() {
		
		scannerState = ScannerState.Rooms;
		EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
	}

	
	//Callback when returns from the scanner
	void onScannerMessage(string data){
		Debug.Log("EasyCodeScannerExample - onScannerMessage data:"+data);		
		if (scannerState == ScannerState.Rooms) {
			if(data.Length < 5) {
				return;
			}
			string numberStr = data.Substring(4);
			int roomId = -1;
			int.TryParse(numberStr, out roomId);
			if(roomId > 0) {
				CheckinRoom(roomId);
			}
		}

		else if (scannerState == ScannerState.Treasure) {
			if(data == "server") {
				if(socketInterface.IsServer) {
					Found();
				} else {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.FoundEnemyPiece ());	
					socketInterface.SendFoundOther();
				}
			}
			
			else if(data == "client") {
				if(!socketInterface.IsServer) {
					Found();
				} else {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.FoundEnemyPiece ());	
					socketInterface.SendFoundOther();
				}
			} else if(data == "") {
				return;
			} 
		}

		else if (scannerState == ScannerState.Haunted) {
			if(data == "serverBase" && socketInterface.IsServer ) {
				
				isLocked = false;
				
				ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
				ui.ShowRoomsUI ();
			} else if(data == "clientBase" && !socketInterface.IsServer ) {
				
				isLocked = false;
				
				ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
				ui.ShowRoomsUI ();
			} else {
				EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
			}
		}

		
		
		else if (scannerState == ScannerState.Finished) {
			if(data == "serverBase" && socketInterface.IsServer ) {
				
				ui.SendMessage ("OnAllRoomsFinished");
				socketInterface.SendYouLost();

			} else if(data == "clientBase" && !socketInterface.IsServer ) {
				
				ui.SendMessage ("OnAllRoomsFinished");
				socketInterface.SendYouLost();

			} else {
				EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
			}
		}
	}
	
	//Callback which notifies an event
	//param : "EVENT_OPENED", "EVENT_CLOSED"
	void onScannerEvent(string eventStr){
		Debug.Log("EasyCodeScannerExample - onScannerEvent:"+eventStr);


	}
	
	//Callback when decodeImage has decoded the image/texture 
	void onDecoderMessage(string data){
		Debug.Log("EasyCodeScannerExample - onDecoderMessage data:"+data);

		//dataStr = data;


	}
	

	
	void OnDestroy() {
		
		//Unregister
		EasyCodeScanner.OnScannerMessage -= onScannerMessage;
		EasyCodeScanner.OnScannerEvent -= onScannerEvent;
		EasyCodeScanner.OnDecoderMessage -= onDecoderMessage;
	}

}
