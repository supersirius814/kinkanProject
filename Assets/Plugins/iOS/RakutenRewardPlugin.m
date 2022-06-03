//
//  RakutenRewardPlugin.mm
//  NativePlugin
//
//  Created by Maeda, Kazuya | ASRHQ on 2/8/17.
//  Copyright © 2017 Rakuten Asia Pte. Ltd. All rights reserved.
//

#include <stdio.h>
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <RakutenRewardSDK/RakutenRewardSDK.h>
#import <RakutenRewardSDK/RakutenRewardSDK-Swift.h>

#define kRakutenReward_DefaultCallbackGameObjectName @"Main Camera"

@interface RakutenRewardPlugin: NSObject
@end

static NSString *RewardPackStrings(NSArray * strings);
static NSString *RewardPackJSONArray(NSArray *jsonArray);
static NSString *RewardAchievementDataToJSONString(MissionAchievementData *achievementData);
static NSString *RewardUserToJSONString(User *user);
RakutenRewardPlugin *__unityClientSharedInstance;

@interface RakutenRewardPlugin() <RakutenRewardDelegate>

@property(nonatomic, strong) NSString *callbackGameObjectName;

- (void)invokeUnityGameObjectMethod:(NSString *)methodName message:(NSString *)message;

@end

@implementation RakutenRewardPlugin

+ (RakutenRewardPlugin *)sharedInstance {
    if (!__unityClientSharedInstance) {
        __unityClientSharedInstance = [[RakutenRewardPlugin alloc] init];
    }
    
    return __unityClientSharedInstance;
}

# pragma mark - RakutenRewardDelegate

- (void)didUpdateUser:(User *)user {
    NSString *userJSON = RewardUserToJSONString(user);
    [self invokeUnityGameObjectMethod:@"_RakutenReward_didUpdateUser" message: userJSON];
}

- (void)didSDKStateChange:(enum RakutenRewardStatus)status {
    NSString *statusStr = [NSString stringWithFormat:@"%ld", status];
    [self invokeUnityGameObjectMethod:@"_RakutenReward_didSDKStateChange" message:statusStr];
}

- (void)didUpdateUnclaimedAchievement:(MissionAchievementData *)missionAchievement {
    NSString *json = RewardAchievementDataToJSONString(missionAchievement);
    [self invokeUnityGameObjectMethod:@"_RakutenReward_didUpdateUnclaimedAchievement" message:json];
}

# pragma mark - Private

- (void)invokeUnityGameObjectMethod:(NSString *)methodName message:(NSString *)message {
    NSString *gameObject = self.callbackGameObjectName == nil ?
    kRakutenReward_DefaultCallbackGameObjectName : self.callbackGameObjectName;
    
    const char *gameObjectStr = [gameObject cStringUsingEncoding:NSUTF8StringEncoding];
    const char *methodNameStr = [methodName cStringUsingEncoding:NSUTF8StringEncoding];
    const char *messageStr = [message cStringUsingEncoding:NSUTF8StringEncoding];
    
    UnitySendMessage(gameObjectStr, methodNameStr, messageStr);
}

@end


void RWStartSession(const char *appCode);
void RWOpenPortal();
void RWOpenSignin();
void RWLogAction(const char* actionCode);
void RWClaim(const char* actionCode, const char* achievedDate);
void RWSetUIEnabled(bool enabled);
void RWUpdateMissionList();
void RWSetCallbackGameObjectName(char *gameObjectName);
int RWGetStatus();
int RWGetUnclaimedCount();
int RWGetPoint();
bool RWIsSignin();
bool RWIsOptedOut();
bool RWIsUIEnabled();
const char* RWGetVersion();
void RWSetManualLoationPermissionRequest(bool enabled);


extern UIViewController* UnityGetGLViewController();
extern void UnitySendMessage(const char* obje, const char* method, const char* msg);

void RWSetCallbackGameObjectName(char *gameObjectName) {
    NSString *name = [NSString stringWithCString:gameObjectName encoding:NSUTF8StringEncoding];
    RakutenRewardPlugin *client = [RakutenRewardPlugin sharedInstance];
    client.callbackGameObjectName = name;
}

void RWStartSession(const char *appCode) {
    NSString *appCodeStr = [NSString stringWithCString:appCode encoding:NSUTF8StringEncoding];
    
    [[RakutenReward sharedInstance] startSessionWithAppCode: appCodeStr];
    [RakutenReward sharedInstance].delegate = [RakutenRewardPlugin sharedInstance];
}

void RWOpenPortal() {
    if (@available(iOS 11, *)) {
        [[RakutenReward sharedInstance] openPortal];
    } else {
        // Fallback on earlier versions
    }
}

void RWOpenSignin() {
    if (@available(iOS 11, *)) {
        [[RakutenReward sharedInstance] openSignin];
    } else {
        // Fallback on earlier versions
    }
}

void RWLogAction(const char *actionCode) {
    NSString *actionCodeStr = [NSString stringWithCString:actionCode encoding:NSUTF8StringEncoding];
    if (@available(iOS 11, *)) {
        NSLog(@"logging action %s", actionCode);
        [[RakutenReward sharedInstance] logActionWithActionCode: actionCodeStr];
    } else {
        // Fallback on earlier versions
    }
}

void RWClaim(const char *actionCode, const char *achievedDate) {
    MissionAchievementData *missionAchievementData = [[MissionAchievementData alloc] init];
    [missionAchievementData setActionWithAction:[NSString stringWithCString:actionCode encoding:NSUTF8StringEncoding]];
    [missionAchievementData setAchievedDateStrWithAchievedDateStr:[NSString stringWithCString:achievedDate encoding:NSUTF8StringEncoding]];

    if (@available(iOS 11, *)) {
        [missionAchievementData claim];
    } else {
        // Fallback on earlier versions
    }
}

void RWSetUIEnabled(bool enabled) {
    [[RakutenReward sharedInstance] setUIEnabledWithEnabled:enabled];
}

int RWGetStatus() {
    return [[RakutenReward sharedInstance] getStatus];
}

int RWGetUnclaimedCount() {
    return [[[RakutenReward sharedInstance] getUser] getUnclaimed];
}

int RWGetPoint() {
    return [[[RakutenReward sharedInstance] getUser] getPoint];
}

bool RWIsSignin() {
    return [[[RakutenReward sharedInstance] getUser] isSignin];
}

bool RWIsOptedOut() {
    return [[RakutenReward sharedInstance] isOptedOut];
}

bool RWIsUIEnabled() {
    return [[RakutenReward sharedInstance] isUIEnabled];
}

const char* RWGetVersion() {
    NSString *verStr = [[RakutenReward sharedInstance] getVersion];
    const char *verChar = [verStr cStringUsingEncoding:NSUTF8StringEncoding];
    
    return verChar ? strdup(verChar) : NULL;
}

const char* getUserJSON() {
    return "User JSON test";
}

void RWUpdateMissionList() {
    [[RakutenReward sharedInstance] updateMissionList];
}

void RWSetManualLoationPermissionRequest(bool enabled) {
    [RewardConfiguration setEnableManualLocationPermissionRequest:enabled];
}

#pragma mark - Utility

static NSString *RewardPackStrings(NSArray * strings) {
    NSMutableString *packedString = [NSMutableString string];
    for (NSString *string in strings) {
        [packedString appendFormat:@"%ld:%@", (long)string.length, string];
    }
    
    return packedString;
}

static NSString *RewardPackJSONArray(NSArray *jsonArray) {
    return [jsonArray componentsJoinedByString:@"__"];
}

static NSString *RewardAchievementDataToJSONString(MissionAchievementData *achievementData) {
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    dateFormatter.dateFormat = @"yyyyMMdd";
    NSString *achievedDate = [dateFormatter stringFromDate:achievementData.getAchievedDate];

    NSDictionary *achievementDict = @{
                                      @"action": achievementData.getAction ? achievementData.getAction : @"",
                                      @"point": achievementData.getPoint ? achievementData.getPoint : 0,
                                      @"instruction": achievementData.getInstruction ? achievementData.getInstruction : @"",
                                      @"iconUrl": achievementData.getIconUrl ? achievementData.getIconUrl : @"",
                                      @"notificationType": achievementData.getNotificationType ? achievementData.getNotificationType : @"",
                                      @"name": achievementData.getName ? achievementData.getName : @"",
                                      @"isCustom": [[NSNumber alloc] initWithBool:achievementData.isCustom],
                                      @"achievedDate": achievedDate ? achievedDate : @"",
                                      };
    
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:achievementDict
                                                       options:0
                                                         error:&error];
    NSString *jsonString = nil;
    if (!error || jsonData) {
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    return jsonString;
}

static NSString *RewardUserToJSONString(User *user) {
    NSMutableArray *userAchievementsListJSONArray = [NSMutableArray array];
    
    for (MissionAchievementData *achievement in user.getAchievementsList) {
        [userAchievementsListJSONArray addObject:RewardAchievementDataToJSONString(achievement)];
    }
    
    NSString *userAchievementsListJSONString = RewardPackJSONArray(userAchievementsListJSONArray);
    
    NSDictionary *userDict = @{
                               @"isSignin": [NSNumber numberWithBool:user.isSignin],
                               @"point": [NSNumber numberWithUnsignedInteger:user.getPoint],
                               @"unclaimedCount": [NSNumber numberWithUnsignedInteger:user.getUnclaimed],
                               @"achievementsListJSON": userAchievementsListJSONString
                               };
    
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:userDict
                                                       options:0
                                                         error:&error];
    NSString *jsonString = nil;
    if (!error) {
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    return jsonString;
}
