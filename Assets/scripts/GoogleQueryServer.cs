using UnityEngine;
using System.Collections;
using LitJson;

public class GoogleQueryServer : MonoBehaviour {

	public static string GOOGLE_URL = "http://findmeqr.appspot.com";

	
	//public static string GOOGLE_URL = "http://192.168.0.11:10090";
	//public static string GOOGLE_URL = "http://localhost:10090";


	public float updateTime = 1f;

	private NetConnector netConnector;

	private bool isRunning = false; 

	private Coroutine runningCoroutine;
	// Use this for initialization
	void Start () {
		netConnector = GetComponent<NetConnector> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartServer() {
		isRunning = true;
		runningCoroutine = StartCoroutine (RunServer ());
	}

	public void StopServer() {
		StopCoroutine (runningCoroutine);
		isRunning = false;
	}


	private IEnumerator RunServer() {
		while (isRunning) {
			FetchMessages();
			yield return new WaitForSeconds(updateTime);
		}
	}

	private void FetchMessages() {
		
		HTTPRequest request = HTTPRequest.CreateRequest ();
		request.Url = GOOGLE_URL + "/message?gameId=" + netConnector.GameId + "&playerId=" + (int)netConnector.PlayerColor;
		request.Method = "GET";

		request.RequestFinished += (HTTP.Response reponse, System.Exception exception) => {
			if(exception != null) {
				Debug.Log("Could not connect to server! " + exception.Message);
			}
			else if(reponse != null) {
				JsonData json = JsonMapper.ToObject (reponse.Text);
				if(json == null) {
					Debug.Log("Response is no JSON");
					return;
				}
				for(int i = 0; i < json.Count; i++) {
					
					string message = json[i]["message"].ToString();
					netConnector.GotMessage(message, null);
				}
			} else {
				Debug.Log("Response was null!");
			}
		};
		request.StartRequest ();
	}
}
