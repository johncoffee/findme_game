using UnityEngine;
using System.Collections;

public class UISwitcher : MonoBehaviour {

	public GameObject seekUI;
	public RoomList roomsUI;


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
		seekUI.SetActive (false);
		roomsUI.Show();
	}

	public void OnTimeup() {
		ShowRoomsUI ();
	}
}
