using UnityEngine;
using System.Collections;

public class ServerData {

	public ServerData() {

	}

	public ServerData(ServiceInfo info) {
		name = info.name;
		ip = info.ipAddress;
		port = port;
		id = info.name;
	}

	public string name;
	public string ip;
	public string id;
	public int port;
}
