using UnityEngine;
using System.Collections;

public class UISwitcher : MonoBehaviour {

	public GameObject seekUI;
	public RoomList roomsUI;
	public GameObject finishedUI;
	public GameObject connectUI;
	public GameObject lostUI;
	public GameObject hauntedUI;
	public ConnectThroughServer serverConnect;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowSeekUI() {
		seekUI.SetActive (true);
		roomsUI.Hide ();
	}

	public void ShowRoomsUI() {
		connectUI.SetActive (false);
		seekUI.SetActive (false);
		hauntedUI.SetActive (false);
		roomsUI.Show();
	}
	
	public void ShowFinishedUI() {
		seekUI.SetActive (false);
		roomsUI.Hide();
		hauntedUI.SetActive (false);
		finishedUI.SetActive (true);
	}

	public void ShowLostUI() {
		
		seekUI.SetActive (false);
		roomsUI.Hide();
		finishedUI.SetActive (false);
		hauntedUI.SetActive (false);
		lostUI.SetActive (true);
	}
	
	public void ShowHauntedUI() {
		
		seekUI.SetActive (false);
		roomsUI.Hide();
		finishedUI.SetActive (false);
		hauntedUI.SetActive (true);
	}

	public void ShowServerConnect() {
		connectUI.SetActive (false);
		serverConnect.gameObject.SetActive (true);
		serverConnect.GetAllGames ();
	}
	
	public void ShowConnect() {
		connectUI.SetActive (true);
		serverConnect.gameObject.SetActive (false);
	}

	public void OnTimeup() {
		ShowHauntedUI ();
	}

	public void OnFound() {
		ShowRoomsUI ();
	}
	
	public void OnAllRoomsFinished() {
		ShowFinishedUI ();
	}


}
