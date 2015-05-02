using UnityEngine;
using System.Collections;

public class OneTouchConnectWidget : MonoBehaviour {
	
	public Texture backgroundTexture;
	public Texture clientTexture;
	public Texture serverTexture;
	public Texture oneTouchTextureBig;
	public Texture oneTouchTextureSmall;
	public Rect landScapePosition = new Rect(372, 199, 280, 220);
	public Rect horizontalPosition = new Rect(372, 199, 280, 220);
	public GameObject connectedGameObject;
	private Rect position;
	private int fontSize;

	public string serviceType = "_onetouchconnect._tcp.";
	public string serviceName = "";
	public int portNumber = 10000;
	
	Matrix4x4 guiMatrix;
	Matrix4x4 previousMatrix;
	
	int maxNumberOfDots = 3;
	int currentNumberOfDots;
	
	private Vector2 scrollPosition;
	
	private float timer = 0;
	private const float TIME_TO_CHANGE_DOT = 1f;
	DeviceOrientation lastFrameOrientation;
	
	bool connected = false;
	
	// Use this for initialization
	void Start () {
		
		SetupScreen();
		
		RegisterServerAndBrowse();
	
	}
	
	void SetupScreen()
	{
		Vector3 scale;
		
		scale.y = Screen.height/768f; // calculate vert scale
		
		scale.x = scale.y; // this will keep your ratio base on Vertical scale
		
		scale.z = 1;
		
		float scaleX = Screen.width/1024f; // store this for translate
		
			
			
		//Matrix4x4 svMat = GUI.matrix; // save current matrix // substitute matrix - only scale is altered from standard
		//if(scale.y > 1)
		//{
		guiMatrix = Matrix4x4.TRS(new Vector3( (scaleX - scale.y) / 2 * 1024f, 0, 0), Quaternion.identity, scale);
		
		if(Screen.width > Screen.height)
		{
			position = landScapePosition;
			fontSize = 32;
		}
		else
		{
			position = horizontalPosition;
			fontSize = 24;
		}
		
		lastFrameOrientation = Input.deviceOrientation;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.deviceOrientation != lastFrameOrientation)
		{
			SetupScreen();
		}
		
		lastFrameOrientation =  Input.deviceOrientation;
	
	}
	
	void OnGUI() {
		
		timer += Time.deltaTime;
		
		if(timer > TIME_TO_CHANGE_DOT)
		{
			timer = 0;
			currentNumberOfDots++;
			
			if(currentNumberOfDots > maxNumberOfDots)
			{
				currentNumberOfDots = 0;
			}
		}
		
		string dotsToAppend = "";
		
		for(int i = 0; i < currentNumberOfDots; i++)
		{
			dotsToAppend += ".";
		}
		
		previousMatrix = GUI.matrix;
		
		GUI.matrix = guiMatrix;
		
		
		//GUI.Label(position, "Hello");
		GUI.DrawTexture(position, backgroundTexture);
		
		if(Screen.height > 640) 
		{
			GUI.DrawTexture(new Rect(position.x + 10, position.y + 10, position.width - 10, position.height / 1.5f), oneTouchTextureBig);
		}
		else
		{
			GUI.DrawTexture(new Rect(position.x + 10, position.y + 10, position.width - 10, position.height / 1.5f), oneTouchTextureSmall);
		}
		
		
		GUI.skin.label.fontSize = fontSize;
		
		GUI.Label(new Rect(position.x + position.width / 2 - (2f * fontSize), position.y + position.height - position.height / 4 - 10, position.width - 10, 50), "Searching" + dotsToAppend);
		
		ServiceInfo[] services = OneTouchConnectEventHandler.getServices.ToArray();
		
		if(services.Length > 0)
		{
			GUI.skin.button.fontSize = 24;
			
			GUI.DrawTexture(new Rect (position.x, position.y + position.height, position.width, 150), backgroundTexture);
			
			scrollPosition = GUI.BeginScrollView(new Rect(position.x, position.y + position.height, position.width, 150), scrollPosition, 
				new Rect(0, 0, position.width - 20, position.height), false, true);
			
			Rect startingRectangle = new Rect(0, 10, position.width - 20, 60);
			
			for(int i = 0; i < services.Length; i++)
			{
				if(GUI.Button(startingRectangle, services[i].name))
				{
					AttemptConnection(services[i]);
				}
				startingRectangle.y += startingRectangle.height + 15;
			}
			
			GUI.EndScrollView();
		}
		
		if(connected)
		{
			GUI.Label(new Rect(position.x + position.width / 2 - (2f * fontSize), position.y + position.height - position.height / 4 - 10, position.width - 10, 50), "Connected");
		}
		
		GUI.matrix = previousMatrix;
	
	}
	
	void RegisterServerAndBrowse()
	{
		Network.InitializeServer(2, portNumber, false);
		OneTouchConnectInterface.PublishAServiceAndBrowse(serviceName, serviceType, 10000);
		//OneTouchConnectInterface.PublishAService(serviceName, serviceType, portNumber);
	}
	
	void AttemptConnection(ServiceInfo service)
	{
		NetworkConnectionError error = Network.Connect(service.ipAddress, portNumber);
		
		if(error == NetworkConnectionError.NoError)
		{
			gameObject.SetActive(false);
			OneTouchConnectInterface.UnregisterAService();
			OneTouchConnectInterface.StopSearching();
		}
	}
	
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		Debug.Log("Player Connected");
		gameObject.SetActive(false);
		OneTouchConnectInterface.UnregisterAService();
		OneTouchConnectInterface.StopSearching();
		connected = true;
		connectedGameObject.SetActive(true);
	}
			
	void OnApplicationQuit()
	{
		OneTouchConnectInterface.OnApplicationQuit();
	}
}
