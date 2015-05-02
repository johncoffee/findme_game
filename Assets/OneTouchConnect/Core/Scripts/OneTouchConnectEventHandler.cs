using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OneTouchConnectEventHandler : MonoBehaviour {

	public delegate void ServiceChangeEventHandler( ServiceInfo info );
	
	public static event ServiceChangeEventHandler ServiceFound;
	public static event ServiceChangeEventHandler ServiceLost;
	public static event ServiceChangeEventHandler ServiceResolutionFailed;
	
	public delegate void BrowsingChangeEventHandler(string message);
	
	public static event BrowsingChangeEventHandler BrowsingStarted;
	public static event BrowsingChangeEventHandler BrowsingEnded;
	
	public delegate void ServiceRegistrationEventHandler(string serviceName);
	
	public static event ServiceRegistrationEventHandler ServiceRegistrationSucceeded;
	public static event ServiceRegistrationEventHandler ServiceRegistrationFailed;
	public static event ServiceRegistrationEventHandler ServiceStopped;

	public delegate void AndroidEvent();
	public static AndroidEvent AndroidReady;
	
	private static List<ServiceInfo> foundServices = new List<ServiceInfo>();	

	public static List<ServiceInfo> getServices
	{
		get
		{
			return foundServices;
		}
		set
		{
			foundServices = value;
		}
	}

	public void AndroidOTCReady(string empty)
	{
		Debug.Log("AndroidOTCReady called");

		if(AndroidReady != null)
		{
			AndroidReady();
		}
	}
	
	public void OnServiceFoundAndResolved (string serviceString)
	{	
		string[] splitStrings = SplitStringsForServices (serviceString);
		
		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);
		
		if(!serviceInfo.ipAddress.Contains("0.0.0.0"))
		{
			foundServices.Add (serviceInfo);
			if(ServiceFound != null)
			{
				ServiceFound(serviceInfo);
			}
		}
	}


	
	private static ServiceInfo CreateServiceInfoFromStrings(string[] splitStrings)
	{
		ServiceInfo serviceInfo = new ServiceInfo ();

		for (int i = 0; i < splitStrings.Length; i++) 
		{
			switch (i) 
			{
			case 0:
				{
					
					serviceInfo.name = splitStrings [i];
					break;
				}
			case 1:
				{
					serviceInfo.ipAddress = splitStrings [i];
					break;
				}
			case 2:
				{
					if (!int.TryParse (splitStrings [i], out serviceInfo.portNumber)) 
				{
						Debug.LogError ("Number wasn't parsed");
				}
					break;
				}
			}			
			//Debug.Log(splitStrings[i]);
		}
		
		return serviceInfo;
	}
	
	private static string[] SplitStringsForServices (string serviceString)
	{
		char[] divisors = new char[]
		{		
			'|',		
		};
		return serviceString.Split (divisors);
	}
	
	public void OnServiceLost(string serviceString)
	{	
		string[] splitStrings = SplitStringsForServices (serviceString);
		
		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);
		
		for(int i = 0; i < foundServices.Count; i++)
		{
			if(serviceInfo.name == foundServices[i].name)
			{
				foundServices.RemoveAt(i);
				break;
			}
		}
		
		if(ServiceLost != null)
		{
			ServiceLost(serviceInfo);
		}
	}
	
	public void OnBrowsingBegin(string conformation)
	{
		if(BrowsingStarted != null)
		{
			BrowsingStarted(conformation);
		}
	}
	
	public void OnBrowsingEnd(string finished)
	{
		if(BrowsingEnded != null)
		{
			BrowsingEnded(finished);
			
		}
	}

	public void OnServiceRegistrationSuccess(string serviceName)
	{
		if (ServiceRegistrationSucceeded != null)
		{
			ServiceRegistrationSucceeded(serviceName);
		}
	}
	
	public void OnServiceRegistrationFailed(string serviceName)
	{
		
		if (ServiceRegistrationFailed != null)
		{
			ServiceRegistrationFailed(serviceName);
		}
	}

	public void OnServiceRegistrationStopped(string emptyMessage)
	{
		if (ServiceStopped != null)
		{
			ServiceStopped(emptyMessage);
		}
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		previousServiceRegistrationSuccess = false;
		previousServiceRegistrationFailure = false;
#endif
	}
	
	public void OnServiceFoundAndResolutionFailed(string serviceString)
	{
		string[] splitStrings = SplitStringsForServices(serviceString);
		
		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);
		
		if(ServiceResolutionFailed != null)
		{
			ServiceResolutionFailed(serviceInfo);
		}
	}
	
	
	public static void OnServiceRegisteredStatic(string serviceString)
	{
		if(ServiceRegistrationSucceeded != null)
		{
			ServiceRegistrationSucceeded(serviceString);
		}
	}

	public static void OnServiceRegistrationStoppedStatic(string emptyMessage)
	{
		if (ServiceStopped != null)
		{
			ServiceStopped(emptyMessage);
		}
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		previousServiceRegistrationSuccess = false;
		previousServiceRegistrationFailure = false;
		#endif
	}
	
	public static void OnServiceFoundStatic(string serviceString)
	{

		string[] splitStrings = SplitStringsForServices (serviceString);
		
		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);
		
		if(!serviceInfo.ipAddress.Contains("0.0.0.0"))
		{
			foundServices.Add (serviceInfo);
			if(ServiceFound != null)
			{
				ServiceFound(serviceInfo);
			}
		}
		
		
	}
	
	public static void OnServiceFoundAndResolutionFailedStatic(string serviceString)
	{
		string[] splitStrings = SplitStringsForServices(serviceString);
		
		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);
		
		if(ServiceResolutionFailed != null)
		{
			ServiceResolutionFailed(serviceInfo);
		}
	}
	
	public static void OnServiceRegistrationFailedStatic(string serviceName)
	{	
		if(ServiceRegistrationFailed != null)
		{
			ServiceRegistrationFailed(serviceName);
		}
	}
	
	public static void OnBrowsingBeginStatic(string conformation)
	{
		if(BrowsingStarted != null)
		{
			BrowsingStarted(conformation);
		}
	}
	
	public static void OnBrowsingEndStatic(string finished)
	{
		if(BrowsingEnded != null)
		{
			BrowsingEnded(finished);
			
		}
	}
	
	public static void OnServiceLostStatic(string serviceString)
	{
		string[] splitStrings = SplitStringsForServices (serviceString);

		ServiceInfo serviceInfo = CreateServiceInfoFromStrings(splitStrings);

		for(int i = 0; i < foundServices.Count; i++)
		{
			if(serviceInfo.name == foundServices[i].name)
			{
				foundServices.RemoveAt(i);
				break;
			}
		}
		
		if(ServiceLost != null)
		{
			ServiceLost(serviceInfo);
		}


	}
	
	
	
	void OnApplicationPause(bool paused)
	{
		OneTouchConnectInterface.OnApplicationPause(paused);
	}

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

	public static bool previousServiceRegistrationSuccess = false;
	public static bool previousServiceRegistrationFailure = false;

	public static string cachedServiceName = "";
	void Update ()
	{
		if (NetworkServiceDiscoveryHelperMono.serviceRegistrationSuccess &&
												!previousServiceRegistrationSuccess) 
		{
			//Debug.Log ("Created service");
			previousServiceRegistrationSuccess = NetworkServiceDiscoveryHelperMono.serviceRegistrationSuccess;
			NetworkServiceDiscoveryHelperMono.serviceRegistrationSuccess = false;
			OnServiceRegistrationSuccess(cachedServiceName);
		}

		if (NetworkServiceDiscoveryHelperMono.serviceRegistrationFailure &&
		    !previousServiceRegistrationFailure) 
		{
			//Debug.Log ("Service Creation Failed");
			previousServiceRegistrationFailure = NetworkServiceDiscoveryHelperMono.serviceRegistrationFailure;
			NetworkServiceDiscoveryHelperMono.serviceRegistrationFailure = false;
			OnServiceRegistrationFailed(cachedServiceName);
		}
		
	}
	#endif

}


public class ServiceInfo
{
	public string name;
	public string ipAddress;
	public int portNumber;
}	
