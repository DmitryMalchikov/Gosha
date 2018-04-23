//
//  SafariView.h
//  Unity-iPhone
//
//  Created by new on 4/21/18.
//
#import <UIKit/UIKit.h>
#import <SafariServices/SafariServices.h>

@interface SafariView : UIViewController
@property(nonatomic, readonly, weak) SFSafariViewController *presentedSafariViewController;
+ (instancetype)sharedInstance;
@end
