//
//  BonjourBrowser.h
//  NetworkTest
//
//  Created by Opposable Games on 15/05/2013.
//  Copyright (c) 2013 Opposable Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface BonjourBrowser : NSObject //<NSNetServiceBrowserDelegate>
{
    

}

// properties for the bonjour browser.

@property(strong) NSNetServiceBrowser* serviceBrowser;
@property(strong,atomic) NSMutableArray* availableServices;
@property bool currentlySearching;

// function to start a search.
// A function may need to be added to end a search,
-(void) StartSearch : (NSObject*) parent : (NSString*) passedType;
-(void) EndSearch;

@end
