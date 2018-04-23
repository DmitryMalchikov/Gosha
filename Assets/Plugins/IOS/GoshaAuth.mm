//
//  GoshaAuth.m
//  Unity-iPhone
//
//  Created by new on 4/23/18.
//

#import "GoshaAuth.h"

extern "C" void UnitySendMessage(const char *, const char *, const char *);

//static const char *GameObjectName;
static GoshaAuth *_instance = [GoshaAuth sharedInstance];

@interface GoshaAuth()
{
    NSString *gameObjectName;
}
@end

@implementation GoshaAuth
+ (GoshaAuth *)sharedInstance
{
    return _instance;
}

+ (void)initialize {
    if(!_instance) {
        _instance = [[GoshaAuth alloc] init];
    }
}

- (id)init
{
    if(_instance != nil) {
        return _instance;
    }
    
    if ((self = [super init])) {
        _instance = self;
        UnityRegisterAppDelegateListener(self);
    }
    return self;
}

- (void)onOpenURL:(NSNotification *)notification
{
    NSURL *url = notification.userInfo[@"url"];
    NSLog(@"%@", gameObjectName);
    if ([[url scheme] isEqualToString:@"goshaapp"]){
        UnitySendMessage([gameObjectName UTF8String], "OnGoshaAuthorization", [[url absoluteString] UTF8String] );
        [[[SafariView sharedInstance] presentedSafariViewController] dismissViewControllerAnimated:true completion:nil];
    }
    
}

- (void)initAuth : (const char*) gameObject
{
    gameObjectName = [NSString stringWithUTF8String:gameObject];
}
@end

extern "C" {
    
    void GoshaInit(const char * gameObject)
    {
        [_instance initAuth:gameObject];
    }
}
