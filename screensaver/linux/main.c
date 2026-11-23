#include <gtk/gtk.h>
#include <gdk/gdkx.h>
#include <webkit2/webkit2.h>
#include <X11/Xlib.h>
#include <errno.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <limits.h>
static Window parent;
static GtkWidget *window;
static gboolean resize_parent(gpointer unused) {
 (void)unused;
 Display *d=GDK_DISPLAY_XDISPLAY(gdk_display_get_default());
 XWindowAttributes a;
 if (!XGetWindowAttributes(d,parent,&a)) { gtk_main_quit(); return G_SOURCE_REMOVE; }
 gtk_window_resize(GTK_WINDOW(window),a.width,a.height);
 return G_SOURCE_CONTINUE;
}
static gboolean navigation(WebKitWebView *view, WebKitPolicyDecision *decision, WebKitPolicyDecisionType type, gpointer data) {
 (void)view; (void)data;
 if(type==WEBKIT_POLICY_DECISION_TYPE_NAVIGATION_ACTION || type==WEBKIT_POLICY_DECISION_TYPE_NEW_WINDOW_ACTION) {
  const char *uri=webkit_uri_request_get_uri(webkit_navigation_action_get_request(webkit_navigation_policy_decision_get_navigation_action(WEBKIT_NAVIGATION_POLICY_DECISION(decision))));
  if(type==WEBKIT_POLICY_DECISION_TYPE_NEW_WINDOW_ACTION || !g_str_has_prefix(uri,"file://")) { webkit_policy_decision_ignore(decision); return TRUE; }
 }
 return FALSE;
}
static int xerror(Display *d,XErrorEvent *e) { (void)d; (void)e; gtk_main_quit(); return 0; }
int main(int argc,char **argv) {
 const char *id=getenv("XSCREENSAVER_WINDOW");
 for(int i=1;i<argc;i++) {
  if(!strcmp(argv[i],"-window-id") && i+1<argc) id=argv[++i];
  else if(!strcmp(argv[i],"-root")) { /* Use daemon's window, never paint the desktop root. */ }
  else { g_printerr("Usage: zar-paulo [-root] [-window-id ID]\n"); return 2; }
 }
 if(id) { char *end; errno=0; parent=strtoul(id,&end,0); if(errno || *end || !parent) return 2; }
 g_setenv("GDK_BACKEND","x11",TRUE);
 gtk_init(&argc,&argv);
 window=gtk_window_new(GTK_WINDOW_TOPLEVEL);
 gtk_window_set_decorated(GTK_WINDOW(window),FALSE);
 gtk_window_set_default_size(GTK_WINDOW(window),1000,700);
 GtkWidget *web=webkit_web_view_new();
 g_signal_connect(web,"decide-policy",G_CALLBACK(navigation),NULL);
 WebKitSettings *settings=webkit_web_view_get_settings(WEBKIT_WEB_VIEW(web));
 webkit_settings_set_enable_developer_extras(settings,FALSE);
 webkit_settings_set_enable_html5_local_storage(settings,FALSE);
 gtk_container_add(GTK_CONTAINER(window),web);
 g_signal_connect(window,"destroy",G_CALLBACK(gtk_main_quit),NULL);
 gtk_widget_realize(window);
 if(parent) {
  Display *d=GDK_DISPLAY_XDISPLAY(gdk_display_get_default());
  XSetErrorHandler(xerror);
  XReparentWindow(d,GDK_WINDOW_XID(gtk_widget_get_window(window)),parent,0,0);
  resize_parent(NULL); g_timeout_add(250,resize_parent,NULL);
 }
 char exe[PATH_MAX]; ssize_t n=readlink("/proc/self/exe",exe,sizeof(exe)-1);
 if(n<0) return 1;
 exe[n]=0; char *slash=strrchr(exe,'/'); if(!slash) return 1; *slash=0;
 char *file=g_build_filename(exe,"ui","index.html",NULL);
 char *uri=g_filename_to_uri(file,NULL,NULL);
 if(!uri) return 1;
 webkit_web_view_load_uri(WEBKIT_WEB_VIEW(web),uri);
 g_free(file); g_free(uri);
 gtk_widget_show_all(window); gtk_main(); return 0;
}
