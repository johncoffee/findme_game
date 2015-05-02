package com.opposablegames.onetouchconnect;

import java.io.IOException;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.UnknownHostException;
import java.util.Collections;
import java.util.List;

import javax.jmdns.JmDNS;
import javax.jmdns.ServiceEvent;
import javax.jmdns.ServiceInfo;
import javax.jmdns.ServiceListener;

import org.apache.http.conn.util.InetAddressUtils;

import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.content.Context;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.text.format.Formatter;
import android.util.Log;

public class JMDNS_NetworkServiceDiscoveryHelperVersionTwo {

	private static JMDNS_NetworkServiceDiscoveryHelperVersionTwo sharedNetworkServiceDiscoveryHelper;
	
	boolean jmdnsActivated = false;
	Context mContext;
	
	android.net.wifi.WifiManager.MulticastLock lock;
	
	JmDNS jmdns;
	
	ServiceInfo serviceInfo;
	ServiceListener serviceListener;
	
	Thread searchThread;
	Thread registerThread;
	
	// Registered Service Info
	String mServiceType;
	String mServiceName;
	int mPort;
	
	boolean isSearching;
	boolean registeredService;

	private InetAddress _bindingAddress;

	public static JMDNS_NetworkServiceDiscoveryHelperVersionTwo getSingleton(Activity activity) 
	{
		
		
		if (sharedNetworkServiceDiscoveryHelper == null) {
			
			sharedNetworkServiceDiscoveryHelper = new JMDNS_NetworkServiceDiscoveryHelperVersionTwo(activity);
		}

		return sharedNetworkServiceDiscoveryHelper;
	}
	
	public JMDNS_NetworkServiceDiscoveryHelperVersionTwo( final Activity activity) {

		mContext = activity;
		
	}
	
	public void initialize()
	{
		
		Runnable runnable = new Runnable() {
			
			@Override
			public void run() {
			
				SetUp();
				
				try {
					//String ipAddress = getIPAddress(true);
					
					jmdns = JmDNS.create(_bindingAddress);
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
				jmdnsActivated = true;
				
				UnityPlayer.UnitySendMessage("OneTouchConnectManager",
						"AndroidOTCReady","");
				
				Log.i("OneTouchConnect", "jmDNS Set up");
				
			}
		};
		
		Thread thread = new Thread(runnable);
		thread.start();
	}
	
	private void SetUp() 
	{
		android.net.wifi.WifiManager wifi =
		           (android.net.wifi.WifiManager)
		             mContext.getSystemService(android.content.Context.WIFI_SERVICE);
			lock = wifi.createMulticastLock("HeeereDnssdLock");
		        lock.setReferenceCounted(true);
		        lock.acquire();
		        
		WifiManager wifiManager = ((WifiManager) mContext
						.getSystemService("wifi"));

		String ip = Formatter.formatIpAddress(wifiManager
						.getConnectionInfo().getIpAddress());
		
		try {
			_bindingAddress = InetAddress.getByName(ip);
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
				
	}
	
    private static String getIPAddress(boolean useIPv4) {
        try {
            List<NetworkInterface> interfaces = Collections.list(NetworkInterface.getNetworkInterfaces());
            for (NetworkInterface intf : interfaces) {
                List<InetAddress> addrs = Collections.list(intf.getInetAddresses());
                for (InetAddress addr : addrs) {
                    if (!addr.isLoopbackAddress()) {
                        String sAddr = addr.getHostAddress().toUpperCase();
                        boolean isIPv4 = InetAddressUtils.isIPv4Address(sAddr); 
                        if (useIPv4) {
                            if (isIPv4) 
                                return sAddr;
                        } else {
                            if (!isIPv4) {
                                int delim = sAddr.indexOf('%'); // drop ip6 port suffix
                                return delim<0 ? sAddr : sAddr.substring(0, delim);
                            }
                        }
                    }
                }
            }
        } catch (Exception ex) { } // for now eat exceptions
        return "";
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

	
	public void discoverServicesThread(String serviceType, boolean newSearch) {
	    
		if(searchThread != null)
		{
			if(searchThread.isAlive())
			{
				Log.i("OneTouchConnect", "Search Thread Already Running");
				return;
			}
		}

		
		mServiceType = serviceType + "local.";
	    
		SetUpListenerThread();

	}

	public void SetUpListenerThread()
	{
		Runnable runnable = new Runnable() {
			public void run() {
				
				while(!jmdnsActivated)
				{
					try
					{
						Thread.sleep(1000L);
					}
					catch (InterruptedException e) {
				        return;
				    }
				}
				
				Log.i("OneTouchConnectManager", "Thread: " + Thread.currentThread().getName());
				
				((Activity) mContext).runOnUiThread(new Runnable() {
				    public void run() {
				    	SetUpListener();
				    }
				});
		
			}
		};
		
		searchThread = new Thread(runnable);
		searchThread.start();
		
		Log.i("OneTouchConnectManager", "search started");
	}
	
	void SetUpListener()
	{
		if(serviceListener != null)
		{
			stopDiscovery();
		}
		
		Log.i("OneTouchConnectManager", "Thread: " + Thread.currentThread().getName());
		 serviceListener = new ServiceListener()  {
		    	
		    	@Override
		        public void serviceResolved(ServiceEvent ev) {
		    		
					ServiceInfo info = ev.getInfo();

					Log.i("OneTouchConnect", info.getName());
					
					if(mServiceName != null)
					{
						InetAddress[] inetAddresses = info.getInetAddresses();
						
						for(int i = 0; i < inetAddresses.length; i++)
						{
							Log.i("OneTouchConnect", inetAddresses[i].getHostAddress());
							
							if(getIPAddress(true).equalsIgnoreCase(inetAddresses[i].getHostAddress()))
							{
								Log.i("OneTouchConnect", "Service Found With Same Name");
								return;
							}
						}
					}

					UnityPlayer.UnitySendMessage("OneTouchConnectManager",
							"OnServiceFoundAndResolved",createStringToPass(info));
		        }
		    	
		    	@Override
		        public void serviceRemoved(ServiceEvent ev) {
					Log.i("OneTouchConnect", "service removed");

					ServiceInfo serviceInfo = ev.getInfo();

					UnityPlayer.UnitySendMessage("OneTouchConnectManager",
							"OnServiceLost",createStringToPass(serviceInfo));
		        }
		    	
		    	@Override
		        public void serviceAdded(ServiceEvent event) {
		            // Required to force serviceResolved to be called again
		            // (after the first search)
		    		
		            jmdns.requestServiceInfo(event.getType(), event.getName(), true);
		            
					Log.i("OneTouchConnectManager", "service added");
		        }
		    };
		    
		jmdns.addServiceListener(mServiceType, serviceListener);
		isSearching = true;
	}
	
	public void ResumeSearch()
	{
		if(isSearching)
		{
			SetUpListener();
		}
	}
	
	public void ResumeRegistration()
	{
		if(registeredService)
		{
			registerService();
		}
	}
	
	public void registerServiceThread(final String serviceName, final String serviceType,
			final int port) {
		
		
		Runnable runnable = new Runnable() {
			public void run() {
				while (!jmdnsActivated) {
					try {
						Thread.sleep(10L);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
				mServiceName = serviceName + Build.MODEL;
				mServiceType = serviceType + "local.";
				mPort = port;
				
				registerService();
			}
		};
		registerThread= new Thread(runnable);
		registerThread.start();
	}

	public void registerService() {
		Log.i("Debug", "Register Service called successfully");


		if (mPort == -1) {
			//port = initializeServerSocket();
		}

		serviceInfo = ServiceInfo.create(mServiceType, mServiceName, mPort,
				"Created by OneTouchConnect");

		//mPort = port;
		try {
			jmdns.registerService(serviceInfo);
			UnityPlayer.UnitySendMessage("OneTouchConnectManager",
					"OnServiceRegistrationSuccess", mServiceName);
			registeredService = true;
		} catch (IOException e) {
			e.printStackTrace();
			Log.i("TestNetwork", "Service Registration Error");
			UnityPlayer.UnitySendMessage("OneTouchConnectManager",
					"OnServiceRegistrationFailed", mServiceName);
		}

		
	}
	
	
	public void unregisterService() {
		Log.i("Unity", "Remove Service");
		
		jmdns.unregisterAllServices();
		registeredService = false;
	   // jmdns.close();
	}
	
	public void stopDiscovery() {

	    jmdns.removeServiceListener(mServiceType, serviceListener);
	    isSearching = false;
	    //jmdns.close();
		
	}
	
	void PauseDiscovery()
	{
		if(searchThread != null)
		{
			searchThread.interrupt();
		}
		
		
		if(isSearching)
		{
			jmdns.removeServiceListener(mServiceType, serviceListener);
		}
	}
	
	void PauseServiceRegistration()
	{
		if(registerThread != null)
		{
			registerThread.interrupt();
		}
		
		if(registeredService)
		{
			jmdns.unregisterAllServices();
		}
	}
	
	public void pause()
	{
		PauseDiscovery();
		PauseServiceRegistration();
		
	}
	
	public void resume()
	{
		ResumeSearch();
		ResumeRegistration();
		
	}
	
	protected void onDestroy() {
	   if (lock != null) lock.release();
	}
	
}
