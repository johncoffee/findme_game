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
	public NetConnector netConnector;


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

		NFCSensor nfc = GetComponent<NFCSensor> ();
		nfc.GotNFCMessage += NFCTest;

		/*NFC.Subscribe( (string s) => { 
			Debug.Log("Got NFC:" + s);
			ui.SetDebugText(s);

		} );
*/
		//NFC.Subscribe( onScannerMessage );

		//		CheckinRoom(1);


	}

	void NFCTest(string message) {
		ui.SetDebugText (message);
	}
	

	// Update is called once per frame
	void Update ()
	{
	
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			onScannerMessage("room1");
		} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			onScannerMessage("room2");
		} else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			onScannerMessage("room3");
		} else if(Input.GetKeyDown(KeyCode.Alpha4)) {
			onScannerMessage("room4");
		} else if(Input.GetKeyDown(KeyCode.A)) {
			onScannerMessage("server");
		} else if(Input.GetKeyDown(KeyCode.S)) {
			onScannerMessage("serverBase");
		} else if(Input.GetKeyDown(KeyCode.K)) {
			onScannerMessage("client");
		} else if(Input.GetKeyDown(KeyCode.L)) {
			onScannerMessage("clientBase");
		} 


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
			scannerState = ScannerState.Rooms;
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
		EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
	}

	public void YouLost() {
		ui.ShowLostUI ();
	}



	public void WaitForOtherPlayer(ServerData serverData) {
		netConnector.PlayerColor = NetConnector.PlayerColors.Blue;
		ui.ShowWaitingRoom ();
		netConnector.SetServer (serverData);

	}


	public void StartGame() {
		Debug.Log ("Ready to play!" + netConnector.ToString());
		scannerState = ScannerState.Rooms;
		ui.ShowRoomsUI ();
	}

	public void ConnectToGame(ServerData serverData) {
		
		netConnector.PlayerColor = NetConnector.PlayerColors.Red;
		netConnector.StartGame (serverData);
		StartGame ();
	}


	public void CheckIn() {
		scannerState = ScannerState.Rooms;
		EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
	}

	
	//Callback when returns from the scanner
	void onScannerMessage(string data){
		
		ui.SetDebugText(data);
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
			if(data == "blue") {
				if(netConnector.PlayerColor == NetConnector.PlayerColors.Blue) {
					Found();
				} else {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.FoundEnemyPiece ());	
					netConnector.SendFoundOther();
				}
			}
			
			else if(data == "red") {
				if(netConnector.PlayerColor == NetConnector.PlayerColors.Red) {
					Found();
				} else {
					ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.FoundEnemyPiece ());	
					netConnector.SendFoundOther();
				}
			} else if(data == "") {
				return;
			} 
		}

		else if (scannerState == ScannerState.Haunted) {
			if(data == "blueBase" && netConnector.PlayerColor == NetConnector.PlayerColors.Blue ) {
				
				isLocked = false;
				
				ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
				ui.ShowRoomsUI ();
			} else if(data == "redBase" && netConnector.PlayerColor == NetConnector.PlayerColors.Red ) {
				
				isLocked = false;
				
				ExecuteEvents.Execute<ITimeupEvent> (gameObject, null, (x,y) => x.UnlockEvent ());	
				ui.ShowRoomsUI ();
			} else {
				//EasyCodeScanner.launchScanner( true, "Scanning...", -1, false);
			}
		}

		
		
		else if (scannerState == ScannerState.Finished) {
			if(data == "blueBase" && netConnector.PlayerColor == NetConnector.PlayerColors.Blue ) {
				
				ui.SendMessage ("OnAllRoomsFinished");
				netConnector.SendYouLost();

			} else if(data == "redBase" && netConnector.PlayerColor == NetConnector.PlayerColors.Red ) {
				
				ui.SendMessage ("OnAllRoomsFinished");
				netConnector.SendYouLost();

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
	}



	

	
	void OnDestroy() {
		
		//Unregister
		EasyCodeScanner.OnScannerMessage -= onScannerMessage;
		EasyCodeScanner.OnScannerEvent -= onScannerEvent;
		EasyCodeScanner.OnDecoderMessage -= onDecoderMessage;
	}
	

}
