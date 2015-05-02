package com.opposablegames.onetouchconnect;



import android.app.Activity;
import android.content.Context;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.text.format.Formatter;
import android.util.Log;
import com.unity3d.player.UnityPlayer;
import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.UnknownHostException;
import java.util.Timer;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

public class JMDNS_NetworkServiceDiscoveryHelper {
	Context mContext;
	JmDNS jmdnsNetworkDiscoveryManager;
	public WifiManager wifiManager;
	ServiceListener jmdnsServiceListener;
	final String TAG = "TestNetwork";
	ServiceInfo mServiceInfo;
	InetAddress _bindingAddress;
	String mServiceName;
	String mServiceType;
	Thread jmDNSSwitchOffThread;
	Thread jmDNSTurnOnThread;
	Thread jmDNSRegistrationThread;
	Thread jmDNSSearchThread;
	String registeredServiceName;
	String registeredServiceType;
	int registeredServicePort;
	int port;
	boolean jmDNSClosed;
	private static JMDNS_NetworkServiceDiscoveryHelper sharedNetworkServiceDiscoveryHelper;
	
	boolean jmdnsActivated = false;

	public static JMDNS_NetworkServiceDiscoveryHelper getSingleton(
			Activity activity) {
		
		if (sharedNetworkServiceDiscoveryHelper == null) {
			sharedNetworkServiceDiscoveryHelper = new JMDNS_NetworkServiceDiscoveryHelper(
					activity);
		}

		return sharedNetworkServiceDiscoveryHelper;
	}

	private void SetUp() 
	{
		Log.i("TestNetwork", "called Setup");
		wifiManager = ((WifiManager) mContext
				.getSystemService("wifi"));

		String ip = Formatter.formatIpAddress(wifiManager
				.getConnectionInfo().getIpAddress());
		
		
		try {
			_bindingAddress = InetAddress.getByName(ip);
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
	}

	public JMDNS_NetworkServiceDiscoveryHelper( final Activity activity) {
		mContext = activity;
		
		// weirdly enough, onResume may have done all of this before we need to
		if (jmdnsActivated)
		{
			return;
		}
		SetUp();
		
		Runnable runnable = new Runnable() {
			
			@Override
			public void run() {

				//JMDNS_NetworkServiceDiscoveryHelper.this.SetUp(activity);
				try {
	
					Log.i("TestNetwork", "Instantiation method trying to set up jmdns with binding address " + _bindingAddress.toString());
					jmdnsNetworkDiscoveryManager = JmDNS.create(_bindingAddress);
					Log.i("TestNetwork", "Instantiation method created JmDNS");
					jmdnsActivated = true;
				} catch (IOException e) {
					e.printStackTrace();
					Log.i("TestNetwork", "jmDNS did not intialise correctly");
					return;
				}
				
			}
		};
		
		Thread thread = new Thread(runnable);
		thread.start();

		Log.i("Debug", "constructor Called Successfully");
	}

	private void initialiseServiceListener() {
		jmdnsServiceListener = new ServiceListener() {
			public void serviceResolved(ServiceEvent serviceEvent) {
				Log.i("TestNetwork", "resolving service");

				ServiceInfo info = serviceEvent.getInfo();

				Log.i("TestNetwork", info.getName());

				if (mServiceInfo != null) {
					Log.i("TestNetwork",
							mServiceInfo.getName());
					if (info.getName()
							.equalsIgnoreCase(mServiceInfo.getName())) 
					{
						return;
					}
				}

				UnityPlayer.UnitySendMessage("OneTouchConnectManager",
						"OnServiceFoundAndResolved",createStringToPass(info));
			}

			public void serviceRemoved(ServiceEvent serviceEvent) {
				Log.i("TestNetwork", "service removed");

				ServiceInfo serviceInfo = serviceEvent.getInfo();

				UnityPlayer.UnitySendMessage("OneTouchConnectManager",
						"OnServiceLost",createStringToPass(serviceInfo));
			}

			public void serviceAdded(ServiceEvent serviceEvent) {
				jmdnsNetworkDiscoveryManager.requestServiceInfo(mServiceType, serviceEvent.getName());
				Log.i("TestNetwork", "service added");
			}
		};
	}

	public void registerServiceThread(final String serviceName, final String serviceType,
			final int port) {
		
		
		Runnable runnable = new Runnable() {
			public void run() {
				while (jmDNSClosed) {
					try {
						Thread.sleep(10L);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
				registerService(serviceName, serviceType, port);
			}
		};
		jmDNSRegistrationThread = new Thread(runnable);
		jmDNSRegistrationThread.start();
	}

	public void registerService(String serviceName, String serviceType, int port) {
		Log.i("Debug", "Register Service called successfully");

		registeredServiceName = serviceName;
		registeredServiceType = serviceType;

		registeredServicePort = port;

		serviceName = serviceName + Build.MODEL;
		serviceType = serviceType + "local.";

		if (port == -1) {
			port = initializeServerSocket();
		}

		mServiceInfo = ServiceInfo.create(serviceType, serviceName, port,
				"Created by OneTouchConnect");

		this.port = port;
		try {
			jmdnsNetworkDiscoveryManager
					.registerService(mServiceInfo);
			UnityPlayer.UnitySendMessage("OneTouchConnectManager",
					"OnServiceRegistrationSuccess", serviceName);
		} catch (IOException e) {
			e.printStackTrace();
			Log.i("TestNetwork", "Service Registration Error");
			UnityPlayer.UnitySendMessage("OneTouchConnectManager",
					"OnServiceRegistrationFailed", serviceName);
		}

		
	}

	public void discoverServicesThread(final String serviceType, final boolean newSearch) {

		Runnable runnable = new Runnable() {
			public void run() {
				while (jmDNSClosed) {
					try {
						Thread.sleep(10L);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}

				discoverServices(serviceType, newSearch);
			}
		};
		jmDNSSearchThread = new Thread(runnable);
		jmDNSSearchThread.start();
	}

	private void discoverServices(String serviceType, boolean newSearch) {
		if (newSearch) {
			serviceType = serviceType + "local.";
		}
		if (serviceType == null)
		{
			Log.i("TestNetwork", "trying to discover services with Null serviceType");
		}
		mServiceType = serviceType;

		initialiseServiceListener();
		int tryCount = 0;
		Timer timer = new Timer();
		while (!jmdnsActivated)
		{
			
			/*if (tryCount == 10)
			{
				tryCount = 0;
				Log.i("TestNetwork", "discover services attempting to create jmndns");
				// avoid infinite loop - this probably means JMDNS has not been activated properly, lets try again
				try {
					SetUp();
					Log.i("TestNetwork", "Discover services trying to set up jmdns with binding address " + _bindingAddress.toString());
					jmdnsNetworkDiscoveryManager = JmDNS.create(_bindingAddress);
					Log.i("TestNetwork", "discover services created jmdns");
					jmdnsActivated = true;
					jmDNSClosed = false;
				} catch (IOException e) {
					e.printStackTrace();
					Log.i("TestNetwork",
							"Discover services jmDNS did not intialise correctly");
				}
			}*/
			try {
				
				Thread.sleep(100L);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
		}
		jmdnsNetworkDiscoveryManager.addServiceListener(serviceType, jmdnsServiceListener);
		Log.i("TestNetwork", "JMDNS started listening");
        UnityPlayer.UnitySendMessage("OneTouchConnectManager",
                                     "OnBrowsingBegin", serviceType);
	}

	public int initializeServerSocket() {
		Log.i("Debug", "initializeServerSocket Called Successfully");

		ServerSocket mServerSocket = null;
		try {
			mServerSocket = new ServerSocket(0);
		} catch (IOException e) {
			return 0;
		}

		int port = mServerSocket.getLocalPort();

		return port;
	}

	private String createStringToPass(ServiceInfo serviceInfo) {
		String divisor = "|";
		String finalString = serviceInfo.getName();
		finalString = finalString + divisor;
		finalString = finalString
				+ serviceInfo.getHostAddresses()[0].toString();
		finalString = finalString + divisor;
		finalString = finalString + serviceInfo.getPort();

		Log.d("TestNetwork", finalString);

		return finalString;
	}

	public void stopDiscovery(boolean pause) {
		if (mServiceType != null && jmdnsNetworkDiscoveryManager != null) {
			jmdnsNetworkDiscoveryManager.removeServiceListener(
					mServiceType, jmdnsServiceListener);
		}

		if (!pause) {
			Log.i("Unity", "Search being stopped");
			mServiceType = null;
		}
	}

	public void unregisterService(boolean deleteServiceInfo) {
		Log.i("Unity", "Remove Service");

		if (mServiceInfo != null && jmdnsNetworkDiscoveryManager != null) {
			jmdnsNetworkDiscoveryManager
					.unregisterService(mServiceInfo);
			if (deleteServiceInfo) {
				Log.i("Unity", "Deleting service info");
				mServiceInfo = null;
			}
		}
		if (jmdnsNetworkDiscoveryManager != null)
		{
			jmdnsNetworkDiscoveryManager.unregisterAllServices();
		}
	}
/*
	private void reregisterService() {
		Log.i("TestNetwork", "reregister function called");
		new Thread(new Runnable() {
			public void run() {
				try {
					jmdnsNetworkDiscoveryManager = JmDNS.create(_bindingAddress);
					Log.i("TestNetwork", "Reregister service created the jmdns again");
					jmdnsActivated = true;
				} catch (IOException e1) {
					e1.printStackTrace();
				}

				Log.i("TestNetwork", "reregister thread called");

				Log.i("TestNetwork",registeredServiceName);
				Log.i("TestNetwork",registeredServiceType);
				Log.i("TestNetwork", Integer.toString(registeredServicePort));

				mServiceInfo = ServiceInfo.create(registeredServiceName, registeredServiceType, registeredServicePort,
								"Created by OneTouchConnect");
				try {
					jmdnsNetworkDiscoveryManager.registerService(mServiceInfo);
				} catch (IOException e) {
					e.printStackTrace();
					Log.i("TestNetwork", "Service Registration Error");
				}

				Log.i("TestNetwork", "Service Reregistered");
			}
		}).start();
	}
*/
	public void pause() {
		if (jmdnsServiceListener != null && jmdnsActivated) {
			stopDiscovery(true);
			unregisterService(false);
			Runnable runnable = new Runnable() {
				public void run() {
					try {
						jmdnsNetworkDiscoveryManager.close();
						jmdnsActivated = false;
						jmDNSClosed = true;
                        UnityPlayer.UnitySendMessage("OneTouchConnectManager",
                                                     "OnBrowsingEnd", "GamePaused");
						
					} catch (IOException e) {
						e.printStackTrace();
					}
				}
			};
			jmDNSSwitchOffThread = new Thread(runnable);
			jmDNSSwitchOffThread.start();
		}
	}

	public void resume() {

		if (jmDNSClosed && !jmdnsActivated) {
			Runnable runnable = new Runnable() {
				public void run() {
					while (jmDNSSwitchOffThread.getState() != Thread.State.TERMINATED) {
						try {
							Thread.sleep(10L);
						} catch (InterruptedException e) {
							e.printStackTrace();
						}
					}

					try {
						SetUp();
						jmdnsNetworkDiscoveryManager = JmDNS.create(_bindingAddress);
						Log.i("TestNetwork", "Resume created the jmdns again");
                        UnityPlayer.UnitySendMessage("OneTouchConnectManager",
                                                     "OnBrowsingBegin", "GamePaused");
						jmdnsActivated = true;
						jmDNSClosed = false;
					} catch (IOException e) {
						e.printStackTrace();
						Log.i("TestNetwork",
								"jmDNS did not intialise correctly");
						return;
					}

					if (mServiceInfo != null) {
						registerServiceThread(registeredServiceName,registeredServiceType,registeredServicePort);
					}

					if (mServiceType != null) 
					{
						discoverServicesThread(mServiceType,false);
					}
				}
			};
			jmDNSTurnOnThread = new Thread(runnable);
			jmDNSTurnOnThread.start();
		}
	}
}