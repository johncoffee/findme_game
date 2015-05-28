using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetConnector : MonoBehaviour {



	public enum PlayerColors
	{
		Blue,
		Red
	}

	public MainStuff mainStuff;

	
	private HTTPListener listener;
	private string enemyURL;
	private string gameId;
	private PlayerColors playerColor = PlayerColors.Blue;


	// Use this for initialization
	void Start () {
		listener = GetComponent<HTTPListener> ();
		listener.GotMessage += GotMessage;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	
	public void SendFoundOther() {
		SendWebMessage ("FoundOther");
	}
	
	public void SendYouLost() {
		SendWebMessage ("YouLost");
	}
	
	public void StartGame(ServerData serverData) {
		SetServer (serverData);
		Dictionary<string, string> arguments = new Dictionary<string, string> ();
		arguments.Add("url", "http://" + Network.player.ipAddress + ":9090");
		Debug.Log ("Sending start: " + ToString ());
		SendWebMessage ("StartGame");
	}
	
	public void GotMessage(string message, HttpContentParser formData) {
		Debug.Log ("Got Message!" + message);
		if (message == "FoundOther") {
			mainStuff.OthersHaveFoundYours();
		}
		if (message == "YouLost") {
			mainStuff.YouLost();
		}
		if (message == "StartGame") {
			if(formData != null) {
				enemyURL = formData.Parameters["url"];
			}
			mainStuff.StartGame();
		}
	}

	private void SendWebMessage(string message) {
		SendWebMessage (message, new Dictionary<string, string> ());
	}

	private void SendWebMessage(string message, Dictionary<string, string> arguments) {
		HTTPRequest req = HTTPRequest.CreateRequest ();
		req.Url = enemyURL+ "/message";
		req.Method = "POST";
		req.Arguments.AddField ("message", message);
		req.Arguments.AddField ("gameId", gameId);
		req.Arguments.AddField ("to", ((int)playerColor + 1) % 2);

		foreach (string argument in arguments.Keys) {
			req.Arguments.AddField (argument, arguments[argument]);
		}

		req.StartRequest ();
	}


	public void SetServer(ServerData serverData) {
		enemyURL = serverData.url;
		gameId = serverData.id;
		//StartGame ();
	}

	public string EnemyURL {
		get {
			return this.enemyURL;
		}
		set {
			enemyURL = value;
		}
	}

	public string GameId {
		get {
			return this.gameId;
		}
		set {
			gameId = value;
		}
	}

	public PlayerColors PlayerColor {
		get {
			return this.playerColor;
		}
		set {
			playerColor = value;
		}
	}


	public override string ToString ()
	{
		return string.Format ("[NetConnector: EnemyURL={0}, GameId={1}, PlayerId={2}]", EnemyURL, GameId, this.PlayerColor);
	}

}
