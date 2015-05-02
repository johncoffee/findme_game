using UnityEngine;
using System.Collections;

public class Connected : MonoBehaviour {

	private int fontSize;
	private Rect position;
	
	public Rect landScapePosition = new Rect(372, 199, 280, 220);
	public Rect horizontalPosition = new Rect(372, 199, 280, 220);

	Matrix4x4 guiMatrix;
	Matrix4x4 previousMatrix;
	
	DeviceOrientation lastFrameOrientation;
	
	// Use this for initialization
	void Start () {
	
		SetupScreen();
	}
	
	void SetupScreen()
	{
		Vector3 scale;
		
		scale.y = Screen.height/768f; // calculate vert scale
		
		scale.x = scale.y; // this will keep your ratio base on Vertical scale
		
		scale.z = 1;
		
		float scaleX = Screen.width/1024f; // store this for translate
		
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
	
	void OnGUI()
	{
		previousMatrix = GUI.matrix;
		
		GUI.matrix = guiMatrix;
		
		GUI.skin.label.fontSize = fontSize;
		
		GUI.Label(new Rect(position.x + position.width / 2 - (2f * fontSize), position.y + position.height - position.height / 4 - 10, position.width - 10, 50), "Connected");
		
		GUI.matrix = previousMatrix;
	}
	
	void OnApplicationQuit()
	{
		OneTouchConnectInterface.OnApplicationQuit();
	}
}
