//
//  GoshaAuth.h
//  Unity-iPhone
//
//  Created by new on 4/23/18.
//

#import "AppDelegateListener.h"
#include "RegisterMonoModules.h"
#include "SafariView.h"

@interface GoshaAuth : NSObject <AppDelegateListener>
+ (GoshaAuth *)sharedInstance;
@end
