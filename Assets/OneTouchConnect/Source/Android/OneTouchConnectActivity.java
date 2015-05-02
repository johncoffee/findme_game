package com.opposablegames.onetouchconnect;


import android.os.Bundle;

public class OneTouchConnectActivity extends UnityPlayerNativeActivity {

	private JMDNS_NetworkServiceDiscoveryHelperVersionTwo jmDNS_NSDH;

	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

	}

	protected void onPause() {
		super.onPause();
		this.jmDNS_NSDH = JMDNS_NetworkServiceDiscoveryHelperVersionTwo
				.getSingleton(this);
		this.jmDNS_NSDH.pause();
	}

	protected void onResume() {
		super.onResume();
		this.jmDNS_NSDH = JMDNS_NetworkServiceDiscoveryHelperVersionTwo
				.getSingleton(this);
		this.jmDNS_NSDH.resume();
	}

	
}