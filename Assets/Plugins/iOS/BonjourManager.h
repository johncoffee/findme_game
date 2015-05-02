//
//  BonjourManager.h
//  NetworkTest
//
//  Created by Opposable Games on 20/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "BonjourPublisher.h"
#import "BonjourBrowser.h"
#import <netinet/in.h>
#import <sys/socket.h>
#import <arpa/inet.h>

@interface BonjourManager : NSObject<NSNetServiceBrowserDelegate, NSNetServiceDelegate>
{
    
    // BonjourBrowser and Publisher.
    BonjourBrowser* browser;
    BonjourPublisher* publisher;
    NSNetService* storedService;
    struct sockaddr_in sin;
}


// functions to initialize search, service registration and to intialize the
// class and ensure a singleton is only used.
-(void) initializeSearch:(NSString*) serviceType;
-(void) publishServiceWithName:(NSString*) serviceName serviceType : (NSString*) serviceType portNumber: (int) port;
-(NSString*) createStringToSendFromService: (NSNetService*) netService;
-(void) deregisterService;
-(void) stopSearching;
+(BonjourManager*) getSingleton;
@property (strong)NSNetService* storedService;
-(void) applicationEnteredBackground;
-(void) applicationEnteredForeground;
-(void) willEnterForegroundNotification:(NSNotification *)notification;
-(void) willEnterBackgroundNotification:(NSNotification *)notification;





@end




