using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomList : MonoBehaviour {

	public MainStuff mainStuff;
	public List<Button> buttons = new List<Button>();
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
	}

	public void Hide() {
		
		gameObject.SetActive (false);
	}
}
