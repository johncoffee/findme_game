using UnityEngine;
using System.Collections;

public class NFCSensor : MonoBehaviour {

	public delegate void GotNFCMessageCallback(string message);


	public GotNFCMessageCallback GotNFCMessage;

	AndroidJavaClass nfcSensor;
	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJNI.AttachCurrentThread ();
			nfcSensor = new AndroidJavaClass ("com.agentdroid.findme.UnityNFCActivity");
			nfcSensor.CallStatic ("SetObjectName", name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	
	void JavaMessage(string message) { 
		Debug.Log("message from java: " + message); 
		if (GotNFCMessage != null) {
			GotNFCMessage(message);
		}
	}
}
