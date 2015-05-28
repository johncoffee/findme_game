using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Connector : MonoBehaviour {


	public VerticalLayoutGroup verticalLayout;

	public ServerButton serverButtonPrefab;
	public MainStuff mainStuff;


	private ZeroConf zeroConf;
	public NetConnector connector;


	// Use this for initialization
	void Start () {
		zeroConf = GetComponent<ZeroConf> ();

		
		zeroConf.ServiceFound += ServiceFound;
		zeroConf.StartPublishAndSearch ();
		/*
		ServerButton button = Instantiate (serverButtonPrefab) as ServerButton;
		ServiceInfo info = new ServiceInfo ();
		info.ipAddress = "192.168.1.76";
		info.portNumber = 234;
		info.name = "phone";

		button.ServiceInfo = info;
		button.transform.SetParent (verticalLayout.transform, false);
		Button buttonController = button.GetComponent<Button>();
		buttonController.onClick.AddListener(() => ServerButtonClicked(info));
		LayoutRebuilder.MarkLayoutForRebuild (verticalLayout.transform as RectTransform);


		ServerButton button2 = Instantiate (serverButtonPrefab) as ServerButton;
		ServiceInfo info2 = new ServiceInfo ();
		info2.ipAddress = "192.168.0.50";
		info2.portNumber = 234;
		info2.name = "phone";
		
		button2.ServiceInfo = info2;
		button2.transform.SetParent (verticalLayout.transform, false);
		Button buttonController2 = button2.GetComponent<Button>();
		buttonController2.onClick.AddListener(() => ServerButtonClicked(info2));
		LayoutRebuilder.MarkLayoutForRebuild (verticalLayout.transform as RectTransform);
*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	void ServiceFound(ServiceInfo service) {
		ServerButton button = Instantiate (serverButtonPrefab) as ServerButton;

		ServerData data = new ServerData (service);
		button.ServerData = data;
		button.transform.SetParent (verticalLayout.transform, false);
		Button buttonController = button.GetComponent<Button>();
		buttonController.onClick.AddListener(() => ServerButtonClicked(data));
		LayoutRebuilder.MarkLayoutForRebuild (verticalLayout.transform as RectTransform);
	}

	
	public void ServerButtonClicked(ServerData serverData) {
		//client.Connect (service.ipAddress, service.portNumber.ToString());
		mainStuff.ConnectToGame (serverData);
	}
	

}
