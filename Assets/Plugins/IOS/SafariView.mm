#import "SafariView.h"

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);


@interface SafariView () <SFSafariViewControllerDelegate>
@property(nonatomic, readwrite, weak) SFSafariViewController *presentedSafariViewController;
@end

@implementation SafariView : UIViewController


+ (instancetype)sharedInstance {
    static SafariView *instance;
    static dispatch_once_t token;
    dispatch_once(&token, ^{
        instance = [[SafariView alloc] init];
    });
    return instance;
}

- (void)safariViewControllerDidFinish:(SFSafariViewController *)controller {
    [controller dismissViewControllerAnimated:true completion:nil];
}

-(void)LoadUrl : (const char *)url{
    NSString *urlStr = [NSString stringWithUTF8String:url];
    NSURL *nsurl = [NSURL URLWithString:urlStr];
     [SafariView sharedInstance] .presentedSafariViewController = [[SFSafariViewController alloc] initWithURL:nsurl];
    [SafariView sharedInstance].presentedSafariViewController.delegate = self;
    [UnityGetGLViewController() presentViewController:[SafariView sharedInstance] .presentedSafariViewController animated:YES completion:nil];
}
@end

extern "C" {
    void SafariView_LoadURL(const char *url);
}


void SafariView_LoadURL(const char *url)
{
    SafariView *picker = [SafariView sharedInstance];
    [picker LoadUrl:url];
}


