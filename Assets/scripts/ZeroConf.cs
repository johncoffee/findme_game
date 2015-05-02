using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeroConf : MonoBehaviour
{

	string serviceName;
	bool searching = false;
	private Dictionary<string, ServiceInfo> services = new Dictionary<string, ServiceInfo> ();
	public OneTouchConnectEventHandler.ServiceChangeEventHandler ServiceFound;
	// Use this for initialization


	void Start ()
	{

		
		OneTouchConnectEventHandler.ServiceFound += OnServiceFound;
		OneTouchConnectEventHandler.ServiceLost += OnServiceLost;
		OneTouchConnectEventHandler.BrowsingStarted += OnSearchingStarted;
		OneTouchConnectEventHandler.BrowsingEnded += OnSearchingStopped;	
		OneTouchConnectEventHandler.ServiceResolutionFailed += OnServiceFoundButResolutionFailed;
		OneTouchConnectEventHandler.ServiceRegistrationSucceeded += OnServiceRegistered;
		OneTouchConnectEventHandler.ServiceRegistrationFailed += OnServiceRegistrationFailed;
		
		OneTouchConnectInterface.Initialize ();

	}
	
	// Update is called once per frameserviceName
	void Update ()
	{
		
		if (searching) {
			foreach(ServiceInfo service in OneTouchConnectEventHandler.getServices) {

				if(!services.ContainsKey(service.name)) {
					services.Add(service.name, service);
					Debug.Log("Found one!");
					if(ServiceFound != null) {
						ServiceFound(service);
					}
				}
			}
		}

	}

	public void Search ()
	{
		OneTouchConnectInterface.BrowseForServices ("_findme._tcp.");
		Debug.Log ("We are now looking for Services!");
	}

	public void StartService() {
		serviceName = SystemInfo.deviceName;	
		
		if (string.IsNullOrEmpty (serviceName)) {
			serviceName = "Unnamed Device" + Random.Range(0, 122233);
		}
		Debug.Log ("myservice: " + serviceName);
		OneTouchConnectInterface.PublishAService (serviceName, "_findme._tcp.", 12455);

	}

	public void StartPublishAndSearch() {
		serviceName = SystemInfo.deviceName;	
		
		if (string.IsNullOrEmpty (serviceName)) {
			serviceName = "Unnamed Device" + Random.Range(0, 122233);
		}
		Debug.Log ("myservice: " + serviceName);
		OneTouchConnectInterface.PublishAServiceAndBrowse (serviceName, "_findme._tcp.", 12455);

	}

	public void StopSearching() {
		OneTouchConnectInterface.StopSearching ();
	}

	void FoundService (ServiceInfo info)
	{
		Debug.Log ("Found service:" + info.name);

	}
	
	void OnServiceFound (ServiceInfo info)
	{
		Debug.Log ("OnServiceFound: " + info.name);
	}
	
	void OnServiceLost (ServiceInfo info)
	{
		Debug.Log ("OnServiceLost: " + info.name);
	}
	
	void OnSearchingStarted (string message)
	{
		searching = true;
		Debug.Log ("OnSearchingStarted " + message);
	}
	
	void OnSearchingStopped (string message)
	{
		
		Debug.Log ("OnSearchingStopped " + message);
	}
	
	void OnServiceRegistered (string serviceName)
	{
		Debug.Log ("ServiceRegistered: " + serviceName);
	}
	
	void OnServiceRegistrationFailed (string serviceName)
	{
		Debug.Log ("ServiceRegistrationFailed: " + serviceName);
	}
	
	// ADVANCED
	void OnServiceFoundButResolutionFailed (ServiceInfo info)
	{
		Debug.Log ("OnServiceFoundButResolutionFailed: " + info.name);
	}
	
	void OnFailedToConnect (NetworkConnectionError error)
	{
		
		Debug.Log ("Could not connect to server: " + error);
	}
	
	void OnApplicationQuit ()
	{
		OneTouchConnectInterface.OnApplicationQuit ();
		
		OneTouchConnectEventHandler.ServiceFound -= OnServiceFound;
		OneTouchConnectEventHandler.ServiceLost -= OnServiceLost;
		OneTouchConnectEventHandler.BrowsingStarted -= OnSearchingStarted;
		OneTouchConnectEventHandler.BrowsingEnded -= OnSearchingStopped;	
		OneTouchConnectEventHandler.ServiceResolutionFailed -= OnServiceFoundButResolutionFailed;
		OneTouchConnectEventHandler.ServiceRegistrationSucceeded -= OnServiceRegistered;
		OneTouchConnectEventHandler.ServiceRegistrationFailed -= OnServiceRegistrationFailed;
		
	}
}
