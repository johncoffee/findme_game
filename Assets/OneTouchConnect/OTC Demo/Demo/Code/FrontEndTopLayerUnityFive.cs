using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrontEndTopLayerUnityFive : MonoBehaviour {

	// Use this for initialization
	[SerializeField]
	Text text;
	
	
		
	// Positions for UI Elements
	public string serviceName;
	public string serviceType = "_otc._tcp.";
	
	bool searching = false;
	
	bool playerConnected = false;
	
	void Start () {
			
		serviceName = SystemInfo.deviceName	;	
		
		if (string.IsNullOrEmpty(serviceName))
		{
			serviceName = "Unnamed Device";
		}
		
		text.text = ("Service Name = (" + serviceName + ")");
		
		
				
		OneTouchConnectEventHandler.ServiceFound += OnServiceFound;
		OneTouchConnectEventHandler.ServiceLost += OnServiceLost;
		OneTouchConnectEventHandler.BrowsingStarted += OnSearchingStarted;
		OneTouchConnectEventHandler.BrowsingEnded += OnSearchingStopped;	
		OneTouchConnectEventHandler.ServiceResolutionFailed += OnServiceFoundButResolutionFailed;
		OneTouchConnectEventHandler.ServiceRegistrationSucceeded += OnServiceRegistered;
		OneTouchConnectEventHandler.ServiceRegistrationFailed += OnServiceRegistrationFailed;

		OneTouchConnectInterface.Initialize();
	
	}

	void Update()
	{
		if (searching)
		{
			if (OneTouchConnectEventHandler.getServices.Count > 0)
			{
				string services = (OneTouchConnectEventHandler.getServices.Count + " services found: ");
				for (int i = 0; i < OneTouchConnectEventHandler.getServices.Count; i++) 
				{
					services += OneTouchConnectEventHandler.getServices[i].name + ", ";
					
				}
				services = services.Remove(services.Length-2);
				text.text = services;
			}
			else
			{
				text.text = "Searching...";
			}
		}
		else
		{
			//text.text = "";
		}
		
	}

	public void OnRegisterButtonClicked()
	{
		RegisterServiceAndCreateServer();
	}

	public void OnSearchButtonPressed()
	{
		OneTouchConnectInterface.BrowseForServices (serviceType);
	}
	/*
	void OnGUI ()
	{
		GUI.contentColor = Color.white;
		
		GUI.Box (headingPosition, "OneTouchConnect");
		
		// Publish button clicked. 
		if (GUI.Button (registerButtonPosition, "Broadcast")) {
			RegisterServiceAndCreateServer();
		}
		
		// Search button clicked.
		if (GUI.Button (searchButtonPosition, "Search")) {
			
			OneTouchConnectInterface.BrowseForServices (serviceType);
		}
		
		float startingPosX = searchButtonPosition.x;		
		// Make sure services listed start a reasonable distance away from the search button;
		float startingPosY = searchButtonPosition.y + searchButtonPosition.height * 2;
			
		Rect currentRect = new Rect (startingPosX, startingPosY, searchButtonPosition.width, searchButtonPosition.height);
		
		// All found services should be listed. 
		// Loop through all found services and make a button for each found service.
		for (int i = 0; i < OneTouchConnectEventHandler.getServices.Count; i++) 
		{
			if(GUI.Button (currentRect, OneTouchConnectEventHandler.getServices[i].name))
			{
				OnServiceButtonClicked(OneTouchConnectEventHandler.getServices[i]);
			}
			
			currentRect.y += searchButtonPosition.height;

			text.text = "Service Found";
			
			//GUI.Label (new Rect (400, 400, 40, 40), "Called");
		}
		
		if(playerConnected)
		{
			GUI.Label(new Rect (50,50, 150, 40), "Function called through RPC");
		}
		
	}
	*/
	void OnServiceFound(ServiceInfo info)
	{
		Debug.Log("OnServiceFound: " + info.name);
	}
	
	
	void OnServiceLost(ServiceInfo info)
	{
		Debug.Log("OnServiceLost: " + info.name);
	}
	
	void OnSearchingStarted(string message)
	{
		searching = true;
		Debug.Log("OnSearchingStarted " + message);
	}
	
	void OnSearchingStopped(string message)
	{
		
		Debug.Log("OnSearchingStopped " + message);
	}
	
	void OnServiceRegistered(string serviceName)
	{
		Debug.Log("ServiceRegistered: " + serviceName);
	}
	
	void OnServiceRegistrationFailed(string serviceName)
	{
		Debug.Log("ServiceRegistrationFailed: " + serviceName);
	}
	
	// ADVANCED
	void OnServiceFoundButResolutionFailed(ServiceInfo info)
	{
		Debug.Log("OnServiceFoundButResolutionFailed: " + info.name);
	}
	
	// GUI Button pressed
	void OnServiceButtonClicked(ServiceInfo info)
	{
		Debug.Log("OnServiceButtonClicked: "+ info.name);
		
		Debug.Log(info.ipAddress + " " + info.portNumber);
	}
	
	void RegisterServiceAndCreateServer()
	{		
		OneTouchConnectInterface.PublishAService (serviceName, serviceType, 10000);
	}
	
	
    void OnFailedToConnect(NetworkConnectionError error) {
		
        Debug.Log("Could not connect to server: " + error);
    }
	
	
	void OnApplicationQuit()
	{
		OneTouchConnectInterface.OnApplicationQuit();
		
		OneTouchConnectEventHandler.ServiceFound -= OnServiceFound;
		OneTouchConnectEventHandler.ServiceLost -= OnServiceLost;
		OneTouchConnectEventHandler.BrowsingStarted -= OnSearchingStarted;
		OneTouchConnectEventHandler.BrowsingEnded -= OnSearchingStopped;	
		OneTouchConnectEventHandler.ServiceResolutionFailed -= OnServiceFoundButResolutionFailed;
		OneTouchConnectEventHandler.ServiceRegistrationSucceeded -= OnServiceRegistered;
		OneTouchConnectEventHandler.ServiceRegistrationFailed -= OnServiceRegistrationFailed;
	
	}
}
