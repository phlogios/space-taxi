//
//  SFUnityJSInterface.m
//  Splitforce
//
//  Created by Stephen Price on 21/05/2014.
//  Copyright (c) 2014 Ikura Group Ltd. All rights reserved.
//

#import "SFUnityJSInterface.h"


extern void UnitySendMessage(const char *, const char *, const char *);


// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}


@implementation SFUnityJSInterface {
    UIWebView *webView;
    NSMutableArray *queue;
}

static SFUnityJSInterface *_sharedInterface = nil;

+ (instancetype)sharedInterface;
{
    if (_sharedInterface == nil)
    {
        static dispatch_once_t onceToken;
        dispatch_once(&onceToken, ^{
            _sharedInterface = [[SFUnityJSInterface alloc] init];
        });
    }
    return _sharedInterface;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
        webView = [[UIWebView alloc] init];
        queue = [NSMutableArray.array retain];
    }
    return self;
}

#pragma mark - Instance Methods

- (void)evaluateJavascript:(NSString *)javascriptExpression for:(NSString *)objectName method:(NSString *)methodName;
{
    [queue addObject:[@[javascriptExpression, objectName, methodName] retain]];
    [self releaseQueue];
}

- (void)resetJSEnvironmentFor:(NSString *)objectName method:(NSString *)methodName;
{
    [queue addObject:[@[objectName, methodName] retain]];
    [self releaseQueue];
}

- (void)releaseQueue
{
    dispatch_async(dispatch_get_main_queue(), ^{
        [self runQueue];
    });
}

- (void)runQueue
{
    if (queue.count == 0) return;
    NSArray *command = queue[0];
    [command autorelease];
    
    [queue removeObjectAtIndex:0];
    
    if (command.count == 2)
    {
        NSString *objectName = command[0];
        NSString *methodName = command[1];
        
        if (webView) [webView release];
        webView = [[UIWebView alloc] init];
        [self releaseQueue];
        return;
    }
    if (command.count != 3)
    {
        // Invalid command
        [self releaseQueue];
        return;
    }
    
    NSString *javascriptExpression = command[0];
    NSString *objectName = command[1];
    NSString *methodName = command[2];
    
    NSString *wrappedJs = [NSString stringWithFormat:@"var sfResult = %@;", javascriptExpression];
    [webView stringByEvaluatingJavaScriptFromString:wrappedJs];
    
    NSString *result = [webView stringByEvaluatingJavaScriptFromString:@"sfResult.toString();"];
    result = [NSString stringWithFormat:@"%@:%@", result, javascriptExpression];
    
    UnitySendMessage(MakeStringCopy(objectName.UTF8String), MakeStringCopy(methodName.UTF8String), MakeStringCopy(result.UTF8String));
    
    dispatch_async(dispatch_get_main_queue(), ^{
        [self releaseQueue];
    });
}

@end

extern "C" {
    void _evaluateJavascript (char* javascriptExpression, char* objectName, char* methodName)
    {
        [[SFUnityJSInterface sharedInterface] evaluateJavascript:CreateNSString(javascriptExpression) for:CreateNSString(objectName) method:CreateNSString(methodName)];
    }
    
    void _resetJSEnvironment (char* objectName, char* methodName)
    {
        [[SFUnityJSInterface sharedInterface] resetJSEnvironmentFor:CreateNSString(objectName) method:CreateNSString(methodName)];
    }
}
