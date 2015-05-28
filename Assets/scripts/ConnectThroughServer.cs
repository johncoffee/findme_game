using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ConnectThroughServer : MonoBehaviour {

	public InputField gameNameInput;
	public ServerButton serverButtonPrefab;
	public UISwitcher ui;
	public MainStuff mainStuff;
	public GoogleQueryServer googleQueryServer;

	private VerticalLayoutGroup verticalLayout;

	private List<ServerButton> buttons = new List<ServerButton>();
	// Use this for initialization
	void Start () {
		verticalLayout = GetComponent<VerticalLayoutGroup> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			BackPressed();
		}
	}

	public void GetAllGames() {
		StartCoroutine (GetAllGamesInternal ());
	}


	
	private IEnumerator GetAllGamesInternal() {
		var r = new HTTP.Request("GET", GoogleQueryServer.GOOGLE_URL + "/games");
		yield return r.Send();
		if(r.exception == null) {
			AddServerButtons(r.response.Text);
		} else {
			Debug.LogError(r.exception);
		}
	}

	public void StartServer() {
		if (string.IsNullOrEmpty (gameNameInput.text)) {
			return;
		}

		HTTPRequest request = HTTPRequest.CreateRequest ();
		request.Url = GoogleQueryServer.GOOGLE_URL + "/games";
		request.Method = "POST";
		request.Arguments.AddField ("url", "http://" + Network.player.ipAddress + ":9090");
		request.Arguments.AddField ("name", gameNameInput.text);
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

				ServerData serverData = new ServerData();
				serverData.id = json["gameId"].ToString();
				serverData.name = "";
				serverData.url = GoogleQueryServer.GOOGLE_URL;
			
				mainStuff.WaitForOtherPlayer(serverData);
				googleQueryServer.StartServer ();
			} else {
				Debug.Log("Response was null!");
			}
		};
		request.StartRequest ();

	}

	private void AddServerButtons(string jsonString) {
		JsonData json = JsonMapper.ToObject (jsonString);
		for (int i = 0; i < json.Count; i++) {
			JsonData jsonData = json[i];
			ServerData serverData = new ServerData();
			serverData.id = jsonData["id"].ToString();
			serverData.url = GoogleQueryServer.GOOGLE_URL;
			serverData.name = jsonData["name"].ToString();
			
			ServerButton button = Instantiate (serverButtonPrefab) as ServerButton;
			buttons.Add(button);
			button.ServerData = serverData;
			button.transform.SetParent (verticalLayout.transform, false);
			Button buttonController = button.GetComponent<Button>();
			buttonController.onClick.AddListener(() => ServerButtonClicked(serverData));
			LayoutRebuilder.MarkLayoutForRebuild (verticalLayout.transform as RectTransform);
		}
	}

	private void ServerButtonClicked(ServerData serverData) {


		
		HTTPRequest request = HTTPRequest.CreateRequest ();
		request.Url = GoogleQueryServer.GOOGLE_URL + "/addPlayer";
		request.Method = "POST";
		request.Arguments.AddField ("url", "http://" + Network.player.ipAddress + ":9090");
		request.Arguments.AddField ("id", serverData.id);
		request.RequestFinished += (HTTP.Response response, System.Exception exception) => {
			if(exception != null) {
				Debug.Log("Could not connect to server! " + exception.Message);
			}
			else if(response != null) {

				mainStuff.ConnectToGame (serverData);
				googleQueryServer.StartServer ();
			} else {
				Debug.Log("Response was null!");
			}
		};
		request.StartRequest ();

	}

	private void BackPressed() {
		ClearButtons ();
		ui.ShowConnect ();
	}

	private void ClearButtons() {
		foreach (ServerButton button in buttons) {
			Destroy(button.gameObject);
		}
		buttons.Clear ();
	}

}
