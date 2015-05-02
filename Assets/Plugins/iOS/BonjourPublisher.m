//
//  BonjourPublisher.m
//  NetworkTest
//
//  Created by Opposable Games on 15/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import "BonjourPublisher.h"
#import "BonjourManager.h"

@implementation BonjourPublisher

// This is used to publish a service.
// An object needs to be passed for callbacks
-(bool)StartPublishingWithDelegate: (BonjourManager*) manager
                     serviceName: (NSString*) nameOfService
                     serviceType: (NSString*) nameOfServiceType
                      portNumber: (int)portNumber
{
    if(self.service == nil)
    {
        
        self.service = [[NSNetService alloc]initWithDomain: @"" type: nameOfServiceType name:nameOfService port:portNumber];
        
        
        [self.service setDelegate:manager];
    
        [self.service publish];
    }
    
    return true;
}

//Stop publishing function to stop a service being published. This needs further expansion. 
-(bool)StopPublishing
{
    return true;
}

@end
