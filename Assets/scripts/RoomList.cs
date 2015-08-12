using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomList : MonoBehaviour {

	public MainStuff mainStuff;
	private VerticalLayoutGroup layoutGroup;
	public Button buttonPrefab;

	private List<Button> buttons = new List<Button>();
	public NetConnector netConnector;
	public Text teamOutput;

	private bool init = false;

	// Use this for initialization
	void Start () {
		Init ();
	}

	void Init() {
		if (init) {
			return;
		}
		init = true;
		
		layoutGroup = GetComponent<VerticalLayoutGroup> ();

	}
	// Update is called once per frame
	void Update () {
	
	}

	public void Show() {
		gameObject.SetActive (true);
		
		Init ();
		for (int i = 0; i < mainStuff.NumRooms; i++) {
			Button button = Instantiate(buttonPrefab) as Button;
			button.transform.SetParent(layoutGroup.transform, false);
			button.transform.GetChild(0).GetComponent<Text>().text = "Room" + (i + 1);
			buttons.Add(button);
		} 
		LayoutRebuilder.MarkLayoutForRebuild (layoutGroup.transform as RectTransform);

		for(int i = 0; i < buttons.Count; i++) {
			if(mainStuff.visitedRooms.Contains((i + 1))) {
				buttons[i].interactable = false;
			}
		}
		if (netConnector.PlayerColor == NetConnector.PlayerColors.Blue) {
			teamOutput.text = "You are team blue";
		} else {
			teamOutput.text = "You are team red";
		}
	}

	public void Hide() {
		foreach (Button button in buttons) {
			Destroy(button.gameObject);
		}
		buttons.Clear ();
		gameObject.SetActive (false);
	}
}
