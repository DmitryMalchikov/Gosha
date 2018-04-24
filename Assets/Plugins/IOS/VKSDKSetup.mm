//
//  VKSDKSetup.m
//  Unity-iPhone
//
//  Created by new on 4/21/18.
//

#import "VKSDKSetup.h"
#import "AppDelegateListener.h"
#include "RegisterMonoModules.h"

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

static VKSDKSetup *_instance = [VKSDKSetup sharedInstance];
static VKSdk *sdkInstance = nil;
static const char *LoginSuccessCallback = "OnLoginSuccess";
static const char *LoginFailedCallback = "OnLoginError";
static const char *ShareSuccessCallback = "OnShareSuccess";
static const char *ShareFailedCallback = "OnShareError";
static NSArray *SCOPE = @[VK_PER_WALL, VK_PER_EMAIL];

@interface VKSDKSetup() <VKSdkDelegate, VKSdkUIDelegate>
{
    NSString *gameObjectName;
    NSString *lastAction;
    BOOL authResult;
}
@end
@implementation VKSDKSetup
+ (VKSDKSetup *)sharedInstance
{
    return _instance;
}

+ (void)initialize {
    if(!_instance) {
        _instance = [[VKSDKSetup alloc] init];
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
    NSString *sourceApplication = notification.userInfo[@"sourceApplication"];
    [VKSdk processOpenURL:url fromApplication:sourceApplication];
}

-(void)login
{
    lastAction = @"Auth";
    [VKSdk authorize:SCOPE];
}

-(void)logout
{
    [VKSdk forceLogout];
}

-(void)WallPost : (const char *)link
      linkTitle : (const char *)linkTitle
            text:(const char *)text
       imageLink: (const char *)imageLink
{
    lastAction = @"Share";
    VKShareDialogController * shareDialog = [VKShareDialogController new];
    shareDialog.text = [NSString stringWithUTF8String:text];
    shareDialog.vkImages = @[[NSString stringWithUTF8String: imageLink]]; //3
    shareDialog.shareLink = [[VKShareLink alloc] initWithTitle:[NSString stringWithUTF8String:linkTitle]  link:[NSURL URLWithString:[NSString stringWithUTF8String:link]]]; //4
    [shareDialog setCompletionHandler:^(VKShareDialogController *dialog, VKShareDialogControllerResult result) {
        if (result == VKShareDialogControllerResultDone){
            UnitySendMessage([gameObjectName UTF8String], ShareSuccessCallback, "");
        }
        else{
               UnitySendMessage([gameObjectName UTF8String], ShareFailedCallback, "Failed");
        }
        [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
    }]; //5
    [UnityGetGLViewController() presentViewController:shareDialog animated:YES completion:nil]; //6
}
- (void)vkSdkAccessAuthorizationFinishedWithResult:(VKAuthorizationResult *)result {
    if ([result token]) {
        authResult = YES;
    }
    else{
        authResult = NO;
    }
}

- (void)vkSdkUserAuthorizationFailed {
    UnitySendMessage([gameObjectName UTF8String], LoginFailedCallback, "");
}

- (void)vkSdkNeedCaptchaEnter:(VKError *)captchaError {
    
}

- (void)vkSdkShouldPresentViewController:(UIViewController *)controller {
    [UnityGetGLViewController() presentViewController:controller animated:YES completion:nil];
}

-(void)vkSdkDidDismissViewController:(UIViewController *)controller{
    if ([lastAction isEqualToString:@"Auth"]){
        if (authResult == YES){
            UnitySendMessage([gameObjectName UTF8String], LoginSuccessCallback, "");
        }
        else{         
            UnitySendMessage([gameObjectName UTF8String], LoginFailedCallback, "");
        }
    }
    
    lastAction = @"";
}

- (void)initSDK : (const char*) _appId
     gameObject : (const char*) gameObject
{
    gameObjectName = [NSString stringWithUTF8String:gameObject];
    
    sdkInstance = [VKSdk initializeWithAppId:[NSString stringWithUTF8String:_appId]];
    [sdkInstance registerDelegate:self];
    [sdkInstance setUiDelegate:self];    
    
    [VKSdk wakeUpSession:SCOPE completeBlock:^(VKAuthorizationState state, NSError *error) {
        if (state == VKAuthorizationAuthorized) {
            // Authorized and ready to go
        } else if (error) {
            // Some error happend, but you may try later
        }
    }];
}

@end

extern "C" {
    
    void VKInit(const char *_appId, const char * gameObject)
    {
        [[VKSDKSetup sharedInstance] initSDK:_appId gameObject:gameObject];
    }
    
    BOOL Initialized()
    {
        return sdkInstance != nil;
    }
    
    void VKLogin()
    {
        [[VKSDKSetup sharedInstance] login];
    }
    
    void VKLogout()
    {
        [[VKSDKSetup sharedInstance] logout];
    }
    
    bool VKLoggedIn(){
        return [VKSdk isLoggedIn];
    }
    
    void WallPost(const char *link, const char *linkTitle, const char *text, const char *imageLink)
    {
        [[VKSDKSetup sharedInstance] WallPost:link linkTitle:linkTitle text:text imageLink:imageLink];
    }
}
