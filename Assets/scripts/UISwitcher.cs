using UnityEngine;
using System.Collections;

public class UISwitcher : MonoBehaviour {

	public GameObject seekUI;
	public RoomList roomsUI;
	public GameObject finishedUI;
	public GameObject connectUI;
	public GameObject lostUI;


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
		roomsUI.Show();
	}
	
	public void ShowFinishedUI() {
		seekUI.SetActive (false);
		roomsUI.Hide();
		finishedUI.SetActive (true);
	}

	public void ShowLostUI() {
		
		seekUI.SetActive (false);
		roomsUI.Hide();
		finishedUI.SetActive (false);
		lostUI.SetActive (true);
	}

	public void OnTimeup() {
		ShowRoomsUI ();
	}

	public void OnFound() {
		ShowRoomsUI ();
	}
	
	public void OnAllRoomsFinished() {
		ShowFinishedUI ();
	}


}
