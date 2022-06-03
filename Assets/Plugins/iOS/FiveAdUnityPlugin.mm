#import <Foundation/NSObject.h>
#import <Foundation/NSString.h>
#import <stdio.h>
#import <FiveAd/FiveAd.h>

extern "C" {
  void _FADSettings_initialize(
      const char* appId,
      bool isTest,
      const char* gameObject,
      int unityScreenWidth,
      int unityScreenHeight,
      bool enableSoundByDefaultCalled,
      bool soundEnabled
  );
  void _FADSettings_enableSound(bool enabled);
  bool _FADSettings_isSoundEnabled();

  void _FAD_Interface_remove(const char* objId);
  void _FAD_Interface_enableSound(const char* objId, bool enabled);
  bool _FAD_Interface_isSoundEnaled(const char* objId);
  int  _FAD_Interface_getState(const char* objId);
  void _FAD_Interface_loadAd(const char* objId);

  void _FAD_Interstitial_put(const char* objId, const char* slotId);
  bool _FAD_Interstitial_show(const char* objId);
  void _FAD_Interstitial_loadAdAsync(const char *objId);

  void _FAD_CustomLayout_put(const char* objId, const char* slotId, int width);
  void _FAD_CustomLayout_add(const char *objId, int x, int y);
  int _FAD_CustomLayout_width(const char *objId);
  int _FAD_CustomLayout_height(const char *objId);
  void _FAD_CustomLayout_remove(const char *objId);
  void _FAD_CustomLayout_loadAdAsync(const char *objId);

  void _FAD_VideoReward_put(const char* objId, const char* slotId);
  bool _FAD_VideoReward_show(const char* objId);
  void _FAD_VideoReward_loadAdAsync(const char *objId);
}

@interface FiveAdUnityPluginListener : NSObject<FADLoadDelegate, FADAdViewEventListener>
@property(nonatomic, retain) NSString* objId;
- (id)initWithObjId:(NSString*)objId;
@end

static char* gameObject_ = NULL;
static CGFloat widthScale = 0.0, heightScale = 0.0;
static NSMutableDictionary* objIdMap = [NSMutableDictionary dictionary];
static NSMutableDictionary* objIdToDelegate = [NSMutableDictionary dictionary];

@implementation FiveAdUnityPluginListener
- (id)initWithObjId:(NSString *)objId {
  self = [super init];
  if (self) {
    self.objId = objId;
  }
  return self;
}

- (void)fiveAdDidLoad:(id<FADAdInterface>)ad {
  UnitySendMessage(gameObject_, "OnFiveAdLoad", [self.objId UTF8String]);
}

- (void)fiveAd:(id<FADAdInterface>)ad didFailedToShowAdWithError:(FADErrorCode)errorCode {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    NSString* message = [NSString stringWithFormat:@"%s:%d", [self.objId UTF8String], (int)errorCode];
    UnitySendMessage(gameObject_, "OnFiveAdLoadError", (char *) [message UTF8String]);
  }
}

- (void)fiveAd:(id<FADAdInterface>)ad didFailedToReceiveAdWithError:(FADErrorCode)errorCode {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    NSString* message = [NSString stringWithFormat:@"%s:%d", [self.objId UTF8String], (int)errorCode];
    UnitySendMessage(gameObject_, "OnFiveAdLoadError", (char *) [message UTF8String]);
  }
}

- (void)fiveAdDidClick:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdClick", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidClose:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdClose", [self.objId UTF8String]);
    _FAD_Interface_remove([self.objId UTF8String]);
  }
}

- (void)fiveAdDidStart:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdStart", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidPause:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdPause", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidResume:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdResume", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidViewThrough:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdViewThrough", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidReplay:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    UnitySendMessage(gameObject_, "OnFiveAdReplay", [self.objId UTF8String]);
  }
}

- (void)fiveAdDidImpression:(id<FADAdInterface>)ad {
  id<FADAdInterface> i = [objIdMap objectForKey:self.objId];
  if (i) {
    if (i.creativeType == kFADCreativeTypeImage) {
      UnitySendMessage(gameObject_, "OnFiveAdImpressionImage", [self.objId UTF8String]);
    } else {
      UnitySendMessage(gameObject_, "OnFiveAdImpressionVideo", [self.objId UTF8String]);
    }
  }
}

@end

void _FADSettings_initialize(
    const char* appId,
    bool isTest,
    const char* gameObject,
    int unityScreenWidth,
    int unityScreenHeight,
    bool enableSoundByDefaultCalled,
    bool soundEnabled
) {
  NSString* appIdNs = [NSString stringWithCString:appId encoding:NSUTF8StringEncoding];
  FADConfig* config = [[FADConfig alloc] initWithAppId:appIdNs];
  config.isTest = isTest;
  if (enableSoundByDefaultCalled) {
    [config enableSoundByDefault:soundEnabled];
  }
  [FADSettings registerConfig:config];

  if (gameObject_ == NULL) {
    gameObject_ = (char*) malloc(strlen(gameObject) + 1);
    strcpy(gameObject_, gameObject);
  } else {
    NSCAssert(strcmp(gameObject_, gameObject) == 0, @"gameObject should be same as previous one.");
  }

  UIScreen* screen = [UIScreen mainScreen];
  widthScale = screen.scale * unityScreenWidth / (screen.applicationFrame.size.width * screen.scale);
  heightScale = screen.scale * unityScreenHeight / (screen.applicationFrame.size.height * screen.scale);
}

void _FADSettings_enableSound(bool enabled) {
  [FADSettings enableSound:enabled];
}

bool _FADSettings_isSoundEnabled() {
  return [FADSettings isSoundEnabled] == YES;
}

void _FAD_Interface_remove(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  [objIdMap removeObjectForKey:objIdNs];
  [objIdToDelegate removeObjectForKey:objIdNs];
}
void _FAD_Interface_enableSound(const char* objId, bool enabled) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil) { [i enableSound:enabled]; }
}
bool _FAD_Interface_isSoundEnaled(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil) { return [i isSoundEnabled] == YES; }
  return false;
}
int  _FAD_Interface_getState(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil) { return (int)[i state]; }
  return kFADStateError;
}
void _FAD_Interface_loadAd(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil) { [i loadAd]; }
}

void _FAD_Interstitial_put(const char* objId, const char* slotId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  NSString* slotIdNs = [NSString stringWithCString:slotId encoding:NSUTF8StringEncoding];
  FADInterstitial* i = [[FADInterstitial alloc] initWithSlotId:slotIdNs];
  FiveAdUnityPluginListener* d = [[FiveAdUnityPluginListener alloc] initWithObjId:objIdNs];
  [i setLoadDelegate:d];
  [i setAdViewEventListener:d];
  [objIdToDelegate setObject:d forKey:objIdNs];
  [objIdMap setObject:i forKey:objIdNs];
}
bool _FAD_Interstitial_show(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[FADInterstitial class]]) {
    FADInterstitial* interstitial = (FADInterstitial*) i;
    return [interstitial show];
  }
  return false;
}
void _FAD_Interstitial_loadAdAsync(const char *objId) {
    NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
    id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
    if (i != nil && [i isKindOfClass:[FADInterstitial class]]) {
      FADInterstitial* interstitial = (FADInterstitial*) i;
      [interstitial loadAdAsync];
    }
}


void _FAD_View_add(const char *objId, int unityX, int unityY) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[UIView class]]) {
    UIView* ad = (UIView*) i;
    CGFloat x = unityX / widthScale, y = unityY / heightScale;
    ad.frame = CGRectMake(x, y, ad.frame.size.width, ad.frame.size.height);
    UIViewController* unityViewController = UnityGetGLViewController();
    [unityViewController.view addSubview:ad];
  }
}
int _FAD_View_width(const char *objId) { // in Unity scale.
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[UIView class]]) {
    UIView* ad = (UIView*) i;
    return (int)(ad.frame.size.width * widthScale);
  }
  return -1;
}
int _FAD_View_height(const char *objId) { // in Unity scale.
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[UIView class]]) {
    UIView* ad = (UIView*) i;
    return (int)(ad.frame.size.height * heightScale);
  }
  return -1;
}
void _FAD_View_remove(const char *objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[UIView class]]) {
    UIView* ad = (UIView*) i;
    [ad removeFromSuperview];
  }
}

void _FAD_CustomLayout_put(const char* objId, const char* slotId, int width) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  NSString* slotIdNs = [NSString stringWithCString:slotId encoding:NSUTF8StringEncoding];
  FADAdViewCustomLayout* i = [[FADAdViewCustomLayout alloc] initWithSlotId:slotIdNs width:(width / widthScale)];
  FiveAdUnityPluginListener* d = [[FiveAdUnityPluginListener alloc] initWithObjId:objIdNs];
  [i setLoadDelegate:d];
  [i setAdViewEventListener:d];
  [objIdToDelegate setObject:d forKey:objIdNs];
  [objIdMap setObject:i forKey:objIdNs];
}
void _FAD_CustomLayout_add(const char *objId, int x, int y) { _FAD_View_add(objId, x, y); }
int _FAD_CustomLayout_width(const char *objId) { return _FAD_View_width(objId); }
int _FAD_CustomLayout_height(const char *objId) { return _FAD_View_height(objId); }
void _FAD_CustomLayout_remove(const char *objId) { _FAD_View_remove(objId); }
void _FAD_CustomLayout_loadAdAsync(const char *objId) {
    NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
    id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
    if (i != nil && [i isKindOfClass:[FADAdViewCustomLayout class]]) {
      FADAdViewCustomLayout* customLayout = (FADAdViewCustomLayout*) i;
      [customLayout loadAdAsync];
    }
}

void _FAD_VideoReward_put(const char* objId, const char* slotId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  NSString* slotIdNs = [NSString stringWithCString:slotId encoding:NSUTF8StringEncoding];
  FADVideoReward* i = [[FADVideoReward alloc] initWithSlotId:slotIdNs];
  FiveAdUnityPluginListener* d = [[FiveAdUnityPluginListener alloc] initWithObjId:objIdNs];
  [i setLoadDelegate:d];
  [i setAdViewEventListener:d];
  [objIdToDelegate setObject:d forKey:objIdNs];
  [objIdMap setObject:i forKey:objIdNs];
}
bool _FAD_VideoReward_show(const char* objId) {
  NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
  id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
  if (i != nil && [i isKindOfClass:[FADVideoReward class]]) {
    FADVideoReward* videoReward = (FADVideoReward*) i;
    return [videoReward show];
  }
  return false;
}
void _FAD_VideoReward_loadAdAsync(const char *objId) {
    NSString* objIdNs = [NSString stringWithCString:objId encoding:NSUTF8StringEncoding];
    id<FADAdInterface> i = [objIdMap objectForKey:objIdNs];
    if (i != nil && [i isKindOfClass:[FADVideoReward class]]) {
      FADVideoReward* videoReward = (FADVideoReward*) i;
      [videoReward loadAdAsync];
    }
}
