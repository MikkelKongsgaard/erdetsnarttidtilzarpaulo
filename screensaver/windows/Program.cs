using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

internal static class Program {
 [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr child, IntPtr parent);
 [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr window, int index, int value);
 [DllImport("user32.dll")] static extern bool GetClientRect(IntPtr window, out RECT rect);
 [DllImport("user32.dll")] static extern bool IsWindow(IntPtr window);
 struct RECT { public int Left, Top, Right, Bottom; }
 [STAThread] static void Main(string[] args) {
  ApplicationConfiguration.Initialize();
  string command = args.Length == 0 ? "/c" : args[0].ToLowerInvariant();
  string[] parts = command.Split(':', 2);
  string mode = parts[0].Replace('-', '/');
  if (mode == "/c") { MessageBox.Show("Offline countdown to 19 November 2027, 20:00 Copenhagen. No settings. Requires Microsoft Edge WebView2 Runtime.", "Zar Paulo"); return; }
  IntPtr parent = IntPtr.Zero;
  if (mode == "/p") {
   string value = parts.Length == 2 ? parts[1] : args.Length > 1 ? args[1] : "";
   if (!long.TryParse(value, out long handle) || !IsWindow(new IntPtr(handle))) return;
   parent = new IntPtr(handle);
  } else if (mode != "/s") return;
  bool preview = parent != IntPtr.Zero;
  var forms = new List<Form>();
  Point origin = Cursor.Position;
  var timer = new System.Windows.Forms.Timer { Interval = 100 };
  DateTime started = DateTime.UtcNow;
  foreach (var screen in preview ? new[] { Screen.PrimaryScreen! } : Screen.AllScreens) {
   var form = new Form { FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.Manual, Bounds = screen.Bounds, TopMost = !preview, ShowInTaskbar = false };
   forms.Add(form);
   var web = new WebView2 { Dock = DockStyle.Fill, DefaultBackgroundColor = System.Drawing.Color.FromArgb(244,247,255) };
   form.Controls.Add(web);
   form.Load += async (_, _) => {
    if (preview) { SetWindowLong(form.Handle, -16, 0x40000000 | 0x10000000); SetParent(form.Handle, parent); GetClientRect(parent, out var r); form.SetBounds(0,0,r.Right,r.Bottom); }
    try {
     var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZarPaulo", "WebView2"));
     if (form.IsDisposed) return;
     await web.EnsureCoreWebView2Async(env);
     web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
     web.CoreWebView2.Settings.AreDevToolsEnabled = false;
     web.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
     web.CoreWebView2.SetVirtualHostNameToFolderMapping("countdown.invalid", Path.Combine(AppContext.BaseDirectory,"ui"), CoreWebView2HostResourceAccessKind.DenyCors);
     web.CoreWebView2.NavigationStarting += (_, e) => { if (e.Uri != "https://countdown.invalid/index.html") e.Cancel = true; };
     web.CoreWebView2.NewWindowRequested += (_, e) => e.Handled = true;
     web.Source = new Uri("https://countdown.invalid/index.html");
    } catch (Exception ex) { MessageBox.Show("Unable to start countdown. Install Microsoft Edge WebView2 Runtime.\n" + ex.Message, "Zar Paulo"); Application.Exit(); }
   };
   form.Show();
  }
  timer.Tick += (_, _) => {
   if (preview) {
    if (!IsWindow(parent)) { Application.Exit(); return; }
    GetClientRect(parent,out var r); forms[0].SetBounds(0,0,r.Right,r.Bottom);
   } else if ((DateTime.UtcNow-started).TotalSeconds > 1 && (Math.Abs(Cursor.Position.X-origin.X)>4 || Math.Abs(Cursor.Position.Y-origin.Y)>4 || AnyKey())) Application.Exit();
  };
  timer.Start();
  if (!preview) Cursor.Hide();
  try { Application.Run(); } finally { if (!preview) Cursor.Show(); timer.Dispose(); foreach (var f in forms) f.Dispose(); }
 }
 [DllImport("user32.dll")] static extern short GetAsyncKeyState(int key);
 static bool AnyKey() { for(int i=1;i<255;i++) if((GetAsyncKeyState(i)&0x8000)!=0) return true; return false; }
}
