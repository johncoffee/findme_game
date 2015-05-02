using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerButton : MonoBehaviour {

	public Text label;

	private ServiceInfo service;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ServiceInfo ServiceInfo {
		get {
			return service;
		}
		set {
			service = value;
			label.text = service.ipAddress;
		}
	}

}
