using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerButton : MonoBehaviour {

	public Text label;

	private ServerData serverData;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ServerData ServerData {
		get {
			return serverData;
		}
		set {
			serverData = value;
			label.text = serverData.name;
		}
	}

}
