using UnityEngine;
using System.Collections;

public class ServerData {

	public ServerData() {

	}

	public ServerData(ServiceInfo info) {
		name = info.name;
		url = "http://" + info.ipAddress + ":" + info.portNumber;
		id = info.name;
	}

	public string name;
	public string url;
	public string id;
}
