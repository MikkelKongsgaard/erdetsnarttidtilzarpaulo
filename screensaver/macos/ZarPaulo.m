#import <ScreenSaver/ScreenSaver.h>
#import <WebKit/WebKit.h>
@interface ZarPauloView : ScreenSaverView <WKNavigationDelegate>
@property(nonatomic,strong) WKWebView *web;
@end
@implementation ZarPauloView
- (instancetype)initWithFrame:(NSRect)frame isPreview:(BOOL)preview {
 if ((self=[super initWithFrame:frame isPreview:preview])) {
  self.animationTimeInterval=1.0;
  WKWebViewConfiguration *config=[WKWebViewConfiguration new];
  config.websiteDataStore=[WKWebsiteDataStore nonPersistentDataStore];
  _web=[[WKWebView alloc] initWithFrame:self.bounds configuration:config];
  _web.autoresizingMask=NSViewWidthSizable|NSViewHeightSizable;
  _web.navigationDelegate=self;
  [self addSubview:_web];
 }
 return self;
}
- (void)startAnimation {
 [super startAnimation];
 NSURL *root=[[NSBundle bundleForClass:self.class] resourceURL];
 [self.web loadFileURL:[root URLByAppendingPathComponent:@"index.html"] allowingReadAccessToURL:root];
}
- (void)stopAnimation { [self.web stopLoading]; [self.web loadHTMLString:@"" baseURL:nil]; [super stopAnimation]; }
- (void)animateOneFrame {}
- (NSView *)hitTest:(NSPoint)point { return self; }
- (void)webView:(WKWebView *)web decidePolicyForNavigationAction:(WKNavigationAction *)action decisionHandler:(void (^)(WKNavigationActionPolicy))handler {
 handler(action.request.URL.isFileURL ? WKNavigationActionPolicyAllow : WKNavigationActionPolicyCancel);
}
@end
