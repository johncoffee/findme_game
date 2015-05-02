using UnityEngine;
using System.Collections; 
using WebSocketSharp;
using WebSocketSharp.Server;

public class Server : MonoBehaviour
{
	
	WebSocketServer _server;
	FindMeService service;
	public SocketInterface.GotMessageCallback GotMessage;
	SocketInterface socketInterface;




	// Use this for initialization
	void Start ()
	{
		socketInterface = GetComponent<SocketInterface> ();
		_server = new WebSocketServer (12345); 

		_server.AddWebSocketService<FindMeService> ("/findme", BuildService); 
		_server.Start ();

	}

	public FindMeService BuildService ()
	{
		service = new FindMeService (this);
		return service;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnDestroy ()
	{
		//if (_server != null) _server.Stop ();
	}

	public class FindMeService : WebSocketService
	{


		private Server server;
		
		public FindMeService () : base()
		{

		}

		public FindMeService (Server server) : base()
		{
			this.server = server;
		}

		protected override void OnMessage (MessageEventArgs e)
		{
			Loom.QueueOnMainThread (() => {
				//Loom.DispatchToMainThread (() => {
				Debug.Log ("data:" + e.Data);
				server.ReceivedMessage (e.Data);
				//});
			});

		}

		public void SendData (string message)
		{
			Send (message);
		}

		protected override void OnOpen ()
		{
			base.OnOpen ();
			//Loom.DispatchToMainThread (() => {

			Loom.QueueOnMainThread (() => {
				server.Connected ();
			});
			//});
		}

		protected override void OnClose (CloseEventArgs e)
		{
			base.OnClose (e);
			
			Loom.QueueOnMainThread(()=> {
				Debug.Log ("Close:" + e.Reason + ";" + e.Code + ", wasClean:" + e.WasClean);

			});

		}

		protected override void OnError (ErrorEventArgs e)
		{
			base.OnError (e);
			
			Loom.QueueOnMainThread (() => {
				Debug.Log ("Error:" + e.Message);
			});
		}


	}


	public void SendWebMessage (string message)
	{
		service.SendData (message);

	}

	public void ReceivedMessage (string message)
	{
		if (GotMessage != null) {
			GotMessage (message);
		}
	}

	public void Connected ()
	{
			Debug.Log ("Cometdc");
			socketInterface.SetServer ();

	}

}
