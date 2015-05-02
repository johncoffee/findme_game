//
//  BonjourBrowser.m
//  NetworkTest
//
//  Created by Opposable Games on 15/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import "BonjourBrowser.h"

@implementation BonjourBrowser

// Constructor/Initializer
-(id)init
{
    self = [super init];
    if(self)
    {
        self.availableServices = [NSMutableArray array];
        self.currentlySearching = false;
    }
    
    return self;
    
}

// Start a search. An object is required which indicates
// what should be the delegate of the service browser.
// Not sure is I should pass something less generic instead of an NSObject.
-(void)StartSearch : (id <NSNetServiceBrowserDelegate>) parent : (NSString*) passedType
{
    self.serviceBrowser = [[NSNetServiceBrowser alloc]init];
    
    [self.serviceBrowser setDelegate:parent];
    
    [self.serviceBrowser searchForServicesOfType:passedType inDomain:@""];
}

-(void)EndSearch
{
    [self.serviceBrowser stop];
    self.serviceBrowser = nil;
    self.currentlySearching = false;
}


@end