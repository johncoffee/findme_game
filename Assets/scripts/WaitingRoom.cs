using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaitingRoom : MonoBehaviour {

	public Text output;
	public NetConnector netConnector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		output.text = "Waiting...\n" + netConnector.GameId; 
	}
}
