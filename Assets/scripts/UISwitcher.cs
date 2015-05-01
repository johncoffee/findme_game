using UnityEngine;
using System.Collections;

public class UISwitcher : MonoBehaviour {

	public GameObject seekUI;
	public GameObject roomsUI;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowSeekUI() {
		seekUI.SetActive (true);
		roomsUI.SetActive (false);
	}

	public void ShowRoomsUI() {
		seekUI.SetActive (false);
		roomsUI.SetActive (true);
	}

	public void OnTimeup() {
		ShowRoomsUI ();
	}
}
