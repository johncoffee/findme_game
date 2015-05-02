using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Threading;

public static class OneTouchConnectInterface
{
	
	private static string serviceName;
	private static string serviceType;
	private static int port;
	
	static Thread browsingThread;
	
#if UNITY_IOS 
	
	[DllImport ("__Internal")]
	private static extern void _OnSearchClicked (string serviceType);
	
	[DllImport ("__Internal")]
	private static extern void _OnRegisterClicked (string nameOfService, string serviceType, int port);
	
	[DllImport ("__Internal")]
	private static extern void _UnregisterAService ();
	
	[DllImport ("__Internal")]
	private static extern void _StopSearching ();
	
#elif UNITY_STANDALONE_OSX
	
	[DllImport ("OneTouchConnectBundle")]
	private static extern void _OnMacSearchClicked(string testString);
	
	
	[DllImport ("OneTouchConnectBundle")]
	private static extern void _OnMacRegisterClicked(string nameOfService, string serviceType, int port);
	
	[DllImport ("OneTouchConnectBundle")]
	private static extern void _UnregisterAServiceMac ();
	
	[DllImport ("OneTouchConnectBundle")]
	private static extern void _StopSearchingMac ();
		
	
#elif UNITY_ANDROID 
	
	static AndroidJavaClass javaClass;
	static AndroidJavaObject javaObject;
	static IntPtr obj_JavaClass;
	static IntPtr globalJavaClass;
	static private IntPtr obj_Activity;
	static private IntPtr searchMethod;
	static private IntPtr registerMethod;
	//static private IntPtr pauseMethod;
	static private IntPtr resumeMethod;
	static private IntPtr unregisterServiceMethod;
	static private IntPtr stopSearchingMethod;
	
	static bool serviceRegistered;
	static bool searchBegun;
	
	
#endif
	
	static NetworkServiceDiscoveryHelperMono networkServiceDiscoveryHelper = new NetworkServiceDiscoveryHelperMono();
	static bool hasBeenInitialized = false;
	
	public static void Initialize ()
	{
		//foundServices = new List<ServiceInfo> ();
			
		//Network.InitializeServer(1, 10000, true);
		
#if UNITY_ANDROID
		//JavaVM.AttachCurrentThread ();		
		
		char[] os = SystemInfo.operatingSystem.ToCharArray();
		
		Debug.Log(SystemInfo.operatingSystem);
		
		IntPtr cls_Activity = AndroidJNI.FindClass ("com/unity3d/player/UnityPlayer");
		IntPtr fid_Activity = AndroidJNI.GetStaticFieldID (cls_Activity, "currentActivity", "Landroid/app/Activity;");
		obj_Activity = AndroidJNI.GetStaticObjectField (cls_Activity, fid_Activity);
		Debug.Log ("obj_Activity = " + obj_Activity);
		
		jvalue[] networkDiscoveryHelperParameters = new jvalue[1];
		networkDiscoveryHelperParameters[0] = new jvalue();//obj_Activity;
		networkDiscoveryHelperParameters[0].l = obj_Activity;
		
		int versionNumber = 0;
		for(int i = 0; i < os.Length; i++)
		{			
			if(int.TryParse(os[i].ToString(), out versionNumber))
			{
				break;
			}	
		}
		
		SetUpAndroidPre4(networkDiscoveryHelperParameters);

		
		hasBeenInitialized = true;
		
#endif
		// Method one
	}
	
#if UNITY_ANDROID		
	
	private static void SetUpAndroidPre4(jvalue[] networkDiscoveryHelperParameters)
	{
			

		// create a JavaClass object...
		IntPtr cls_JavaClassV2 = AndroidJNI.FindClass ("com/opposablegames/onetouchconnect/JMDNS_NetworkServiceDiscoveryHelperVersionTwo");
		
		IntPtr mid_JavaClassV2 = AndroidJNI.GetStaticMethodID (cls_JavaClassV2, "getSingleton", "(Landroid/app/Activity;)Lcom/opposablegames/onetouchconnect/JMDNS_NetworkServiceDiscoveryHelperVersionTwo;");
		
		IntPtr obj_JavaClassV2 = AndroidJNI.CallStaticObjectMethod(cls_JavaClassV2, mid_JavaClassV2, networkDiscoveryHelperParameters);
		
		Debug.Log ("JavaClass object = " + obj_JavaClassV2);
		
		// create a global reference to the JavaClass object and fetch method id(s)..
		globalJavaClass = AndroidJNI.NewGlobalRef (obj_JavaClassV2);
		
		//IntPtr initializeMethod = AndroidJNI.GetMethodID (cls_JavaClass, "initializeNsd", "()V");
		//AndroidJNI.CallVoidMethod (globalJavaClass, initializeMethod, new jvalue[0]);

		IntPtr initializeMethod  = AndroidJNI.GetMethodID (cls_JavaClassV2, "initialize", "()V");
		AndroidJNI.CallVoidMethod(globalJavaClass, initializeMethod, new jvalue[0]);
		searchMethod = AndroidJNI.GetMethodID (cls_JavaClassV2, "discoverServicesThread", "(Ljava/lang/String;Z)V");
		registerMethod = AndroidJNI.GetMethodID (cls_JavaClassV2, "registerServiceThread", "(Ljava/lang/String;Ljava/lang/String;I)V");
		unregisterServiceMethod = AndroidJNI.GetMethodID(cls_JavaClassV2, "unregisterService", "()V");
		stopSearchingMethod = AndroidJNI.GetMethodID(cls_JavaClassV2, "stopDiscovery", "()V");
		
		// Need to add pause definition to JMDNS
		//pauseMethod = AndroidJNI.GetMethodID(cls_JavaClass, "pause", "()V");	
			
		Debug.Log ("JavaClass global ref = " + globalJavaClass);
			
		
	}
	
#endif	
	
	// Method called when search button is clicked.
	public static void BrowseForServices (string typeOfService)
	{
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
		Debug.LogError("OneTouchConnect will not work in the editor - please compile");
		return;
#endif

		if(!hasBeenInitialized)
		{
			Initialize();
		}


		
		OneTouchConnectEventHandler.getServices.Clear();
#if UNITY_IPHONE
		
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//networkServiceDiscoveryHelper.BrowseForServices(typeOfService);
			_OnSearchClicked(typeOfService);
			
		} 	
		
#elif UNITY_STANDALONE_OSX

		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			_OnMacSearchClicked(typeOfService);
		}



		
		
		
#elif UNITY_ANDROID		
		jvalue[] arguments = new jvalue[2];
		
		arguments[0].l = AndroidJNI.NewStringUTF(typeOfService);
		arguments[1].z = true;
		
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJNI.CallVoidMethod (globalJavaClass, searchMethod, arguments);
		}
		
#else 
		
		if((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor)
			|| Application.platform == RuntimePlatform.OSXEditor) 
		{
		
			//SearchForServicesThread(typeOfService);
			networkServiceDiscoveryHelper.BrowseForServices(typeOfService);
		}
		
#endif
	}

	
	// Method called when register button is clicked.
	public static void PublishAService (string nameOfService, string typeOfService, int portNumber)
	{
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
		Debug.LogError("OneTouchConnect will not work in the editor - please compile");
		return;
#endif
		serviceName = nameOfService;
		serviceType = typeOfService;
		port = portNumber;

#if UNITY_STANDALONE_WIN
		// used because PC version starts a thread which can't communicate back to Unity thread
		// the Event handler instead polls booleans in the NetworkServiceDiscoveryhelper
		OneTouchConnectEventHandler.cachedServiceName = nameOfService;
#endif
		
		if(!hasBeenInitialized)
		{
			Initialize();
		}
		
#if UNITY_IPHONE
		if ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.OSXPlayer)) 
		{
			// Update method to allow a null port
			_OnRegisterClicked(serviceName, serviceType, portNumber);
			//networkServiceDiscoveryHelper.RegisterService(serviceName, serviceType, (short)port);
		}
#elif UNITY_STANDALONE_OSX 
			
		if(Application.platform == RuntimePlatform.OSXPlayer)
		{
			_OnMacRegisterClicked(serviceName, serviceType, port);
		}
			
#elif UNITY_ANDROID	
		if (Application.platform == RuntimePlatform.Android) {
			serviceRegistered = true;
			jvalue[] arguments = new jvalue[3]; 
			
			// Port Number
			arguments[0].l = AndroidJNI.NewStringUTF(serviceName);
			arguments[1].l = AndroidJNI.NewStringUTF(serviceType);
			arguments[2].i = port;
			
			
		
			AndroidJNI.CallVoidMethod (globalJavaClass, registerMethod, arguments);
		}
			
# else
		if((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor)
				|| Application.platform == RuntimePlatform.OSXEditor) 
		{
			networkServiceDiscoveryHelper.RegisterService(serviceName, serviceType, (short)port);
		}
			
			
#endif
	}
	
	public static void PublishAServiceAndBrowse(string serviceName, string serviceType, int port)
	{
		PublishAService(serviceName, serviceType, port);
		BrowseForServices(serviceType);
	}
	
	public static void OnApplicationPause(bool paused)
	{
#if UNITY_ANDROID
		/*if(paused)
		{
			jvalue[] arguments = new jvalue[1];
			arguments[0].l = AndroidJNI.NewStringUTF("");
			AndroidJNI.CallVoidMethod(globalJavaClass, pauseMethod, arguments);
		}
		else
		{
			if(serviceRegistered)
			{
				PublishAService(serviceName, serviceType, port);
			}
			
			if(searchBegun)
			{
				BrowseForServices(serviceType);
			}
		}*/
#endif
	}
	
	public static void UnregisterAService()
	{
#if UNITY_IPHONE
		
		_UnregisterAService();
		
#elif UNITY_ANDROID
		
		jvalue[] arguments = new jvalue[1];
		arguments[0].z = true;
		AndroidJNI.CallVoidMethod(globalJavaClass, unregisterServiceMethod, arguments);
		
#elif UNITY_STANDALONE_OSX
		
		_UnregisterAServiceMac();

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

		networkServiceDiscoveryHelper.stopPublishing();
		
#endif
		
			
	}
	
	public static void StopSearching()
	{
#if UNITY_IPHONE
		
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_StopSearching();
		}
		
		
#elif UNITY_ANDROID
		
		jvalue[] arguments = new jvalue[1];
		arguments[0].z = false;
		AndroidJNI.CallVoidMethod(globalJavaClass, stopSearchingMethod, arguments);
		
#elif UNITY_STANDALONE_OSX
		
		_StopSearchingMac();

#endif

	}
	
	public static void OnApplicationQuit()
	{

#if UNITY_STANDALONE_WIN 
		// uncommented by BT for development purposes
		networkServiceDiscoveryHelper.OnQuit();
		
		if (Application.isWebPlayer == false && Application.isEditor == false)
        {
            Application.CancelQuit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        } 
        
#endif

	}
}
