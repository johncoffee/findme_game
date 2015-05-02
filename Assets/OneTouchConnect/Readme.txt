OneTouchConnect

The multi-platform connection framework from Opposable Games

OneTouchConnect allows cross platform devices to recognise 
each other and start transferring information. 

The usual scenario is getting iOS and Android devices to 
talk to each other seamlessly, however the technology works 
on many other platforms. 

Developers are able to get up and running in seconds, 
with drag and drop prefabs handling most of the complexity, 
but the flexibility underneath to recreate your own 
connection behaviour and architecture.

Requirements:

OneTouchConnect uses the zeroconf protocol over wifi channels.
For the framework to operate, all devices need to be connected 
to the same wifi network.

The software uses custom libraries on Android, iOS and MacOS.  
Windows requires Bonjour Print Services to be installed (http://support.apple.com/kb/DL999).

OneTouchConnect works on Unity Free and Unity Pro.  

Usage:

There are three ways to get OneTouchConnect up and running:

1. Using the demo scene (provided for fast testing purposes)

2. Using the OneTouchConnect Widget (for demos and jams)

3. Using the OTC event handler and publish / browse functions (for seamless embedding in your games)


Demo Scene (pre Unity 4.6):

- Open OneTouchConnect/OTC Demo/Demo/OTC Demo
- Build on two seperate devices 
- Attach both devices to the same network
- Open the app on both devices
- Both apps will autopublish a sample service and begin browsing for other services
- Once an app has connected or been connected to, browsing / publishing will stop

Demo Scene (Unity 4.6 and after):

- Open OneTouchConnect/OTC Demo/Demo/OTC Demo 4.6
- Build on two seperate devices 
- Attach both devices to the same network
- Open the app on both devices
- Both apps will autopublish a sample service and begin browsing for other services
- Once an app has connected or been connected to, browsing / publishing will stop


Widget:

Widget can be used in your own games to provide a simple User Interface to allow players to find 
other devices on the same network.  

Code can be added to the OneTouchConnectWidget script to perform custom behavior when players connect to each other.

- Open OneTouchConnect/Widget/Widget Scene
- Build on two seperate devices 
- Attach both devices to the same network
- Open the app on both devices
- Both apps will autopublish a sample service and begin browsing for other services
- Add code to the OneTouchConnectWidget script, OnPlayerConnected method to trigger your own game behavior


OTC Events, Publishing and Browsing:


- Within a game scene, add a OneTouchConnect/Core/Prefab/OneTouchConnectManager prefab,
  or alternately add a OneTouchConnect/Scripts/OneTouchConnectEventHandler script to an existing, active GameObject
  
- Create custom methods and add these to the relevant static events in the OneTouchConnectEventHandler script.
  For examples see OneTouchConnect/Demo/Code/FrontEndTopLayer
  
- To publish and browse on the same device, call the static method OneTouchConnectInterface.PublishAServiceAndBrowse with
	- A service name - usually the device name
	- A service type - usually product name in service format e.g. "_yourproductname._tcp." or "_myproductname._udp.". 
					   The inclusion of underscores ("_") and periods (".") are important.
	- A port - a free port number
	
- To just publish, call the static method OneTouchConnectInterface.PublishAService with above parameters

- To just browse, call the static method OneTouchConnectInterface.BrowseForServices with the service type (as above)

- To stop publishing, call the static method OneTouchConnectInterface.UnregisterAService

- To stop browsing, call the static method OneTouchConnectInterface.StopSearching

Integration with other plugins for Android

If other plugins are being used with your application, there may be a little bit of work required to integrate it correctly. The activity class derives off Unity’s base activity class. Other plugins may choose to do this too. Should this be the case, you’ll need to edit the Activity to derive off the other plugin’s activity. 

For example, if a plugin has it’s own activity called APluginActivity, you would need to edit the OneTouchConnect Activity file to derive off APluginActivity. You would then just use the Android manifest file that comes with this package as usual. You shouldn’t need to edit either the manifest nor the activity classes.

We are  interested in your feedback, please let us know how you find the framework at onetouchconnect@opposablegames.com.