using System;
using UnityEngine;
using Mono.Zeroconf;

public class NetworkServiceDiscoveryHelperMono
{
    private static string domain = "local";
	private static string serviceName;
	RegisterService service;
	ServiceBrowser browser;

	public static bool serviceRegistrationSuccess = false;
	public static bool serviceRegistrationFailure = false;
	//private static ServiceBrowser browser;

    public NetworkServiceDiscoveryHelperMono()
    {
       
    }

    public void BrowseForServices(string serviceType)
    {
        browser = new ServiceBrowser();
        browser.ServiceAdded += OnServiceAdded;
		// note ServiceRemoved is not working at all!
        browser.ServiceRemoved += OnServiceRemoved;
		
        browser.Browse(serviceType, domain);
    }

    public void RegisterService(string serviceName, string serviceType, short port)
    {
        service = new RegisterService();
        service.Name = serviceName;
        service.RegType = serviceType;
        service.ReplyDomain = "local";
        service.Port = port;
        service.TxtRecord = null;

		serviceRegistrationSuccess = false;
		serviceRegistrationFailure = false;

        service.Response += OnRegisterServiceResponse;
        service.Register();
    }

    private void OnServiceAdded(object o, ServiceBrowseEventArgs args)
    {
		
		// Uncomment
		args.Service.Resolved += OnServiceResolved;
        args.Service.Resolve();
		args.Service.Resolved -= OnServiceResolved;

		browser.ServiceRemoved += OnServiceRemoved;
    }

    private void OnServiceRemoved(object o, ServiceBrowseEventArgs args)
    {
		// This does not appear to be called!
		Debug.LogError ("Lost service");
		Debug.Log("*** Lost name = " + args.Service.Name + " type = " + args.Service.RegType +" domain = " + 
			args.Service.ReplyDomain);
    }

    private void OnServiceResolved(object o, ServiceResolvedEventArgs args)
    {
		//args.Service.Resolved -= OnServiceResolved;
        IResolvableService service = o as IResolvableService;
		
		if(serviceName != null)
		{	
			if(service.Name == serviceName)
			{
				return;
			}
			
		}
		
		//service.Resolved -= OnServiceResolved;
		
	    string fullString = service.Name + "|" + service.HostEntry.AddressList[0] + "|" + service.Port.ToString() + "|";
		
		//sDebug.Log(fullString);
		
		OneTouchConnectEventHandler.OnServiceFoundStatic(fullString);
		

    }

    private void OnRegisterServiceResponse(object o, RegisterServiceEventArgs args)
    {
		switch (args.ServiceError)
        {
            /*case ServiceErrorCode.NameConflict:
                Console.WriteLine("*** Name Collision! '{0}' is already registered",
                    args.Service.Name);
                break;*/
            case ServiceErrorCode.None:
                //Debug.Log("*** Registered name = "+ args.Service.Name);
				serviceName = args.Service.Name;
				// note cannot do the following as Unity is not Thread safe
				// and the library has created a new thread
				//OneTouchConnectEventHandler.OnServiceRegisteredStatic(serviceName);
				// instead set this identifier and poll
				serviceRegistrationSuccess = true;
				break;
            case ServiceErrorCode.Unknown:
                Debug.Log("*** Error registering name = " + args.Service.Name);
				
				// as above
				//OneTouchConnectEventHandler.OnServiceRegistrationFailedStatic(serviceName);
				serviceRegistrationFailure = true;
                break;
        }
    }

	public void stopPublishing ()
	{
		if(service != null)
		{
			//Debug.LogError ("Disposing of service on PC");
			service.Response -= OnRegisterServiceResponse;
			service.Dispose();


			OneTouchConnectEventHandler.OnServiceRegistrationStoppedStatic("");
			service = null;
		}
		else
		{
			//Debug.LogError ("Tried to dispose of service but did not exist");
		}
	}
	
	public void OnQuit()
	{
		if(service != null)
		{
			service.Dispose();
		}
		if(browser != null)
		{
			//sDebug.LogWarning("Browser Entered");
			browser.ServiceAdded -= OnServiceAdded;
			browser.ServiceRemoved -= OnServiceRemoved;
			browser.Dispose();
		}
		//service = null;
		//browser = null;
	}
}

