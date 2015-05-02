//
//  BonjourManager.m
//  NetworkTest
//
//  Created by Opposable Games on 20/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import "BonjourManager.h"

@implementation BonjourManager

@synthesize storedService;

// a static BonjourManager to enable a singleton to be made.
static BonjourManager* sharedBonjourManager;

- (id)init
{
    self = [super init];
    if (self) {
        
        browser = [[BonjourBrowser alloc]init];
        publisher = [[BonjourPublisher alloc]init];
        [self initializeSocket];
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(willEnterForegroundNotification:) name:UIApplicationWillEnterForegroundNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(willEnterBackgroundNotification:) name:UIApplicationDidEnterBackgroundNotification object:nil];
        
    }
    return self;
}

// This function should be called to initialize.
// It ensures that the sharedBonjourManager is initialized.

+ (BonjourManager*) getSingleton
{
    if(sharedBonjourManager == nil)
    {
        sharedBonjourManager = [[BonjourManager alloc] init];
    }

    return sharedBonjourManager;
}

-(void)initializeSocket
{
    // Will move below to seperate method
    
    int ipv4_socket = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
    //int ipv6_socket = socket(PF_INET6, SOCK_STREAM, IPPROTO_TCP);
    
    memset(&sin, 0, sizeof(sin));
    sin.sin_len = sizeof(sin);
    sin.sin_family = AF_INET; // or AF_INET6 (address family)
    sin.sin_port = htons(0);
    sin.sin_addr.s_addr= INADDR_ANY;
    
    socklen_t len = sizeof(sin);
    
    if (bind(ipv4_socket, (struct sockaddr *)&sin, sizeof(sin)) < 0) {
        // Handle the error.
    }
    
    if (getsockname(ipv4_socket, (struct sockaddr *)&sin, &len) < 0) {
        // Handle error here
    }
}


-(void)initializeSearch: (NSString*) serviceType
{
    [browser StartSearch:self: serviceType];
}
    
-(void) publishServiceWithName:(NSString*) serviceName
                  serviceType : (NSString*) serviceType
                    portNumber: (int) port
{
    
    if(port == -1)
    {
        [publisher StartPublishingWithDelegate:self
                                 serviceName: serviceName
                                 serviceType: serviceType
                                  portNumber: sin.sin_port];
    }
    else
    {
        [publisher StartPublishingWithDelegate:self
                                 serviceName: serviceName
                                 serviceType: serviceType
                                  portNumber: port];
    }
}

// Implement delegates for search

// Called during start of search
-(void)netServiceBrowserWillSearch:(NSNetServiceBrowser *)aNetServiceBrowser
{
    UnitySendMessage( "OneTouchConnectManager", "OnBrowsingBegin", "Search Started" );
	browser.currentlySearching = true;
}


// Called when search is finished
-(void)netServiceBrowserDidStopSearch:(NSNetServiceBrowser *)aNetServiceBrowser
{
    UnitySendMessage( "OneTouchConnectManager", "OnBrowsingEnd", "Search Finished" );
    browser.currentlySearching = false;
}

// Callback for when a service is found.
-(void)netServiceBrowser:(NSNetServiceBrowser *)aNetServiceBrowser
          didFindService:(NSNetService *)aNetService
              moreComing:(BOOL)moreComing
{
    //UnitySendMessage( "OneTouchConnectManager", "OnServiceRegistrationSuccess", [stringToSend UTF8String] );
        
    if([aNetService.name isEqualToString:publisher.service.name])
    {
        return;
    }    
    // add the service to the browser's found services.
    [browser.availableServices addObject:aNetService.name];
    [aNetService setDelegate: self];
    [aNetService resolveWithTimeout: 0];
    self.storedService = aNetService;
    
    
}

// Callback if a service is removed.
-(void)netServiceBrowser:(NSNetServiceBrowser *)aNetServiceBrowser
        didRemoveService:(NSNetService *)aNetService
              moreComing:(BOOL)moreComing
{
    NSString* stringToSend;
    
    stringToSend = [self createStringToSendFromService:aNetService];
    UnitySendMessage( "OneTouchConnectManager", "OnServiceLost", [stringToSend UTF8String] );
}

// Callbacks below for services, need expanding.
-(void)netServiceDidStop:(NSNetService *)sender
{
     UnitySendMessage( "OneTouchConnectManager", "OnServiceRegistrationStopped", [@"" UTF8String]);
}

-(void)netServiceWillPublish: (NSNetService*)sender
{
    
}

-(void)netServiceDidPublish:(NSNetService *)sender
{
    NSString* stringToSend;
    
    stringToSend = [sender name];
    UnitySendMessage( "OneTouchConnectManager", "OnServiceRegistrationSuccess", [stringToSend UTF8String] );
}


-(void)netService:(NSNetService *)sender didNotPublish:(NSDictionary *)errorDict
{
    NSString* stringToSend;
    
    stringToSend = [sender name];
    UnitySendMessage( "OneTouchConnectManager", "OnServiceRegistrationFailed", [stringToSend UTF8String] );
}

- (NSString*) createStringToSendFromService: (NSNetService*) netService
{
    NSString *name = @"";
    NSData *address = nil;
    struct sockaddr_in *socketAddress = nil;
    NSString *ipString = @"";
    //int port;
    name = [netService name];
    
    if ([[netService addresses] count] == 0)
    {
        // no addresses - this is most likely a service lost message
        ipString = @"0.0.0.0";
    }
    else
    {
        address = [[netService addresses] objectAtIndex: 0];
        socketAddress = (struct sockaddr_in *) [address bytes];
        ipString = [NSString stringWithFormat: @"%s",inet_ntoa(socketAddress->sin_addr)];
    }
    
    NSString* portString = [NSString stringWithFormat:@"%d", netService.port];
    
    NSMutableString* stringToSend = [[NSMutableString alloc] init];
    NSString* divisor = @"|";//[[NSString alloc] initWithString:@","];
    [stringToSend appendString:name];
    [stringToSend appendString:divisor];
    [stringToSend appendString:ipString];
    [stringToSend appendString:divisor];
    [stringToSend appendString:portString];
    [stringToSend appendString:divisor];
    
    return stringToSend;
    
}

- (void)netServiceDidResolveAddress:(NSNetService *)netService {
    
    NSString* stringToSend;
    
    stringToSend = [self createStringToSendFromService:netService];
    
    UnitySendMessage( "OneTouchConnectManager", "OnServiceFoundAndResolved", [stringToSend UTF8String] );
    
}

- (void)netService:(NSNetService *)sender didNotResolve:(NSDictionary *)errorDict {
    
    NSString* stringToSend;
    
    stringToSend = [self createStringToSendFromService:sender];
    
    UnitySendMessage( "OneTouchConnectManager", "OnServiceFoundAndResolutionFailed", [stringToSend UTF8String] );
}

- (void)applicationEnteredBackground
{
    NSNetService* service = [publisher service];
    
    [service stop];
    
}

-(void)applicationEnteredForeground
{
    NSNetService* service = [publisher service];
    [service publish];
}

- (void)willEnterForegroundNotification:(NSNotification *)notification {
    //NSLog(@\"Will Enter Foreground Notification!\");
          
    [self applicationEnteredForeground];
}

- (void)willEnterBackgroundNotification:(NSNotification *)notification {

    [self applicationEnteredBackground];
}
          
-(void) deregisterService
{
    if(publisher.service != nil)
    {
        [publisher.service stop];
        //[service stop];
        publisher.service = nil;
            
    }
}

-(void) stopSearching
{
    if(browser.currentlySearching)
    {
        [browser EndSearch];
    }
}
-(void) dealloc
{
    [browser release];
    [publisher release];
    [super dealloc];
}

@end

// For Unity
extern "C"
{
    void _OnSearchClicked(const char* serviceTypeToFind)
    {
	    // Ensure that the sharedBonjourManager is initialized and then search.
        NSString* serviceType = [NSString stringWithUTF8String:serviceTypeToFind];
        BonjourManager* bonjourManager = [BonjourManager getSingleton];
        [bonjourManager initializeSearch: serviceType];
    }
    
    void _OnRegisterClicked(const char* passedName, const char* passedType, int portNum)
    {
        NSString* serviceName = [NSString stringWithUTF8String:passedName];
        NSString* serviceType = [NSString stringWithUTF8String:passedType];
	    //Ensure that the sharedBonjourManager is initialized and then intialize a service.
        BonjourManager* bonjourManager = [BonjourManager getSingleton];
        [bonjourManager publishServiceWithName:serviceName serviceType:serviceType portNumber:portNum];
    }
    
    void _UnregisterAService()
    {
        BonjourManager* bonjourManager = [BonjourManager getSingleton];
        [bonjourManager deregisterService];
    }

    void _StopSearching()
    {
        BonjourManager* bonjourManager = [BonjourManager getSingleton];
        [bonjourManager stopSearching];
    }

}
