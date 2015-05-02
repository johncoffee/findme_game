using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomList : MonoBehaviour {

	public MainStuff mainStuff;
	public List<Button> buttons = new List<Button>();
	public SocketInterface socketInterface;
	public Text teamOutput;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show() {
		gameObject.SetActive (true);
		for(int i = 0; i < buttons.Count; i++) {
			if(mainStuff.visitedRooms.Contains((i + 1))) {
				buttons[i].interactable = false;
			}
		}
		if (socketInterface.IsServer) {
			teamOutput.text = "You are team blue";
		} else {
			teamOutput.text = "You are team red";
		}
	}

	public void Hide() {
		
		gameObject.SetActive (false);
	}
}
