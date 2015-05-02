//
//  BonjourPublisher.h
//  NetworkTest
//
//  Created by Opposable Games on 15/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import <Foundation/Foundation.h>
@class BonjourManager;

@interface BonjourPublisher : NSObject //<NSNetServiceDelegate>
{
}

// Start publishing and stop publishing functions.
-(bool)StartPublishingWithDelegate: (BonjourManager*) manager
                     serviceName: (NSString*) nameOfService
                     serviceType: (NSString*) nameOfServiceType
                      portNumber: (int)portNumber;
-(bool)StopPublishing;

@property(strong, atomic) NSNetService* service; // The service that will be published.

@end
