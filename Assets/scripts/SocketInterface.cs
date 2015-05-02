using UnityEngine;
using System.Collections;

public class SocketInterface : MonoBehaviour {

	public delegate void GotMessageCallback(string message);

	public MainStuff mainStuff;

	public UISwitcher ui;



	bool isServer = false;
	Client client;
	Server server;

	// Use this for initialization
	void Start () {
		client = GetComponent<Client> ();
		server = GetComponent<Server> ();
		client.GotMessage += GotMessage;
		server.GotMessage += GotMessage;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetServer() {
		isServer = true;
		ui.ShowRoomsUI ();
	}

	public void SendFoundOther() {
		Debug.Log ("Send Message other");
		if (isServer) {
			server.SendWebMessage ("FoundOther");
		} else {
			client.SendWebMessage("FoundOther");
		}
	}
	
	public void SendYouLost() {
		Debug.Log ("Send Message other");
		if (isServer) {
			server.SendWebMessage ("YouLost");
		} else {
			client.SendWebMessage("YouLost");
		}
	}

	public void GotMessage(string message) {
		Debug.Log ("Got Message!" + message);
		if (message == "FoundOther") {
			mainStuff.OthersHaveFoundYours();
		}
		if (message == "YouLost") {
			mainStuff.YouLost();
		}
	}

	public bool IsServer {
		get { return isServer; } 
	}
}
