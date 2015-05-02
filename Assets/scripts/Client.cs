using UnityEngine;
using System.Collections;
using WebSocketSharp;

public class Client : MonoBehaviour
{


	WebSocket socket;
	public SocketInterface.GotMessageCallback GotMessage;
	public UISwitcher ui;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Connect (string ip, string port)
	{
		string url = "ws://" + ip + ":" + "12345" + "/findme";
		string res = null;

		Debug.Log ("Connecting to: " + ip);
		socket = new WebSocket (url);
		var ver = Application.unityVersion;
		socket.OnOpen += (sender, e) => {
			//socket.Send ("Fuck");
				
			Loom.QueueOnMainThread (() => {
				ui.ShowRoomsUI ();
			});

		};
		socket.OnMessage += (sender, e) => {
				
			res = e.Data;
				
			Loom.QueueOnMainThread (() => {
				Debug.Log ("Message:" + e.Data);		
				if (GotMessage != null) {
					GotMessage (e.Data);
				}
			});

		};
		socket.OnError += (sender, e) => {
						
				
			Loom.QueueOnMainThread (() => {
				Debug.LogError (e.Message);
			});

		};
		socket.OnClose += (object sender, CloseEventArgs e) => {
				
			Loom.QueueOnMainThread (() => {

				Debug.Log ("Close:" + e.Reason + ";" + e.Code + ", wasClean:" + e.WasClean);
			});
		};
		socket.Connect ();

	}

	public void SendWebMessage (string message)
	{
		socket.Send (message);
	}
}
