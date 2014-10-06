//
//  SFUnityJSInterface.h
//  Splitforce
//
//  Created by Stephen Price on 21/05/2014.
//  Copyright (c) 2014 Ikura Group Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SFUnityJSInterface : NSObject

- (void)evaluateJavascript:(NSString *)javascriptExpression call:(NSString *)methodName on:(NSString *)objectName;
- (void)resetJSEnvironmentAndCall:(NSString *)methodName on:(NSString *)objectName;

+ (instancetype)sharedInterface;

@end
