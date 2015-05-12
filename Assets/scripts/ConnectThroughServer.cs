using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ConnectThroughServer : MonoBehaviour {


	public ServerButton serverButtonPrefab;
	public UISwitcher ui;
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
		var r = new HTTP.Request("GET", "http://findmeqr.appspot.com/games");
		yield return r.Send();
		if(r.exception == null) {
			AddServerButtons(r.response.Text);

		} else {
			Debug.LogError(r.exception);
		}
	}

	private void AddServerButtons(string jsonString) {
		JsonData json = JsonMapper.ToObject (jsonString);
		for (int i = 0; i < json.Count; i++) {
			JsonData jsonData = json[i];
			ServerData serverData = new ServerData();
			serverData.id = jsonData["id"].ToString();
			serverData.ip = jsonData["IPPlayer1"].ToString();
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
