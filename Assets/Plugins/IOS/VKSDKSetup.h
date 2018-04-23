//
//  VKSDKSetup.h
//  Unity-iPhone
//
//  Created by new on 4/21/18.
//
#import "VKSdk.h"
#import "AppDelegateListener.h"
#include "RegisterMonoModules.h"

@interface VKSDKSetup : NSObject <AppDelegateListener>
+ (VKSDKSetup *)sharedInstance;
@end
