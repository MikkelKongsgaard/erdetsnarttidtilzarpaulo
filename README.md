# Er det snart tid til Zar Paulo?

Unofficial countdown to **19 November 2027, 20:00 Europe/Copenhagen** at Djurslands Bank Arena, Aarhus. The target is 19:00 UTC (CET); it does not depend on the computer's timezone. At and after the target the countdown stays at zero.

The existing static website remains at the repository root. `countdown.js` supplies the countdown for both the website and the screensavers. `screensaver/shared/index.html` is the shared blue/white retro UI, bundled locally with that script. It uses system fonts, has no remote assets or links, and needs no network after installation. Windows needs WebView2 Runtime installed first.

The favicon is a blue React-style atom on a black background, drawn in SVG at the user’s request. The screensaver uses only text and CSS, with no band artwork. This is not an official band product.

## Downloads and build status

[GitHub Releases](https://github.com/MikkelKongsgaard/erdetsnarttidtilzarpaulo/releases) will contain published packages. PR and branch builds are available as workflow artifacts to signed-in GitHub users; they are not releases. A `screensaver-v*` tag builds all platforms and creates a **draft** release only after all jobs pass. A human must install, preview and validate each package before publishing that draft. This change does not publish a release.

Baseline repository inspection found only `index.html` and `CNAME`; there were no pre-existing `.scr`, `.saver`, or Linux implementations to repair.

## Windows 10/11 x64

1. Install Microsoft's [Evergreen WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).
2. Extract the **entire** ZIP to a permanent folder, for example `%LOCALAPPDATA%\ZarPaulo`. Keep `ui/` and all DLLs beside `ZarPaulo.scr`. This is a folder distribution, not a standalone SCR file.
3. Right-click `ZarPaulo.scr` and choose **Install** (on Windows 11, use Show more options), or run `rundll32.exe desk.cpl,InstallScreenSaver "C:\full\path\ZarPaulo.scr"`.
4. Select ZarPaulo in Screen Saver Settings. Use Preview, and enable **On resume, display logon screen** if wanted. The OS handles locking; the screensaver does not implement a password prompt.

`/s` shows a borderless window on every monitor. Keyboard/mouse input exits after a one-second startup grace period. `/c`, `/c:HWND`, and no arguments show an informational settings dialog (no configurable options). `/p HWND` and `/p:HWND` embed in the supplied preview parent, resize with it, and do not exit on input. Invalid modes/preview handles exit without opening full screen. Preview uses Win32 child-window parenting. Full-screen input is polled because WebView2 uses child windows and can consume normal form input events.

The package includes the .NET runtime; it does not include the Edge runtime. WebView2 stores its local cache in `%LOCALAPPDATA%\ZarPaulo\WebView2`. Uninstall by choosing another screensaver and deleting the extracted folder/cache. Builds are **not Authenticode signed**; SmartScreen may flag them. Verify provenance and hashes. No Windows ARM64 or x86 build is supplied.

## macOS 11+ (Intel and Apple Silicon)

Extract the ZIP and double-click `ZarPaulo.saver` inside `macos/`. Install for your user, then select it under System Settings → Screen Saver (System Preferences on older versions). Alternatively copy it to `~/Library/Screen Savers/`. Remove that bundle to uninstall.

The bundle contains both `arm64` and `x86_64` slices. It subclasses `ScreenSaverView` and loads local resources with WKWebView; the system controls preview, exit, and multi-display instantiation. Web content is stopped when the saver stops. Compatibility must be checked in the actual system screensaver host, including current macOS versions; a successful compiler run is not a host integration test.

Builds have an **ad-hoc signature**, not a Developer ID signature, and are **not notarized**. Gatekeeper may prevent installation. Follow your Mac's supported Privacy & Security approval flow only if you trust the package. Do not disable Gatekeeper globally. Public distribution with fewer warnings requires an Apple Developer membership, appropriate Developer ID signing, notarization with `notarytool`, and validation of the final distributed archive. No signing credentials are included in CI.

## Linux: X11 / XScreenSaver, x86_64

The CI binary targets Ubuntu 24.04 x86_64. Other distributions may require rebuilding against their own libraries. Install runtime packages `libgtk-3-0t64`, `libwebkit2gtk-4.1-0`, `libx11-6`, and `xscreensaver` on Ubuntu 24.04. Extract the archive and move its `linux` directory to a permanent location, e.g. `~/.local/lib/zar-paulo`. Keep the `ui` directory next to `zar-paulo`.

Test the standalone preview with `~/.local/lib/zar-paulo/zar-paulo` (close its window to quit). In `xscreensaver-demo`, add this entry to the existing Programs list, preserving the other entries:

```
"Zar Paulo"  /home/YOUR_USER/.local/lib/zar-paulo/zar-paulo -root
```

Use the real absolute path. XScreenSaver supplies `XSCREENSAVER_WINDOW`; the saver reparents its X11 GTK window into that daemon-owned window. `-window-id ID` explicitly selects a decimal or hexadecimal parent for testing. `-root` never paints directly over the desktop root. Without a supplied parent it opens a standalone preview window. XScreenSaver owns input dismissal, locking, and display selection. Test both the small preview and actual activation in `xscreensaver-demo` before relying on it.

This is **not a native Wayland, GNOME lock screen, KDE lock screen, or systemd integration**. Use an X11 session with XScreenSaver; XWayland alone does not provide lock-screen integration. Do not run the saver as root or disable the WebKit sandbox. To uninstall, remove the program entry and extracted directory.

## Local builds and checks

```
node --check countdown.js
node tests/countdown.cjs
sh screensaver/macos/build.sh
# Windows with .NET 8 SDK:
dotnet publish screensaver/windows/ZarPaulo.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o build/windows
# Rename ZarPaulo.exe to ZarPaulo.scr and retain all other publish files.
# Ubuntu build dependencies:
sudo apt-get install build-essential pkg-config libgtk-3-dev libwebkit2gtk-4.1-dev libx11-dev
sh screensaver/linux/build.sh
```

The macOS script requires Apple's command-line tools and verifies the universal slices and ad-hoc signature. CI packages each platform and runs countdown tests plus an Xvfb Linux launch smoke check. These checks do not establish appearance, Windows preview parenting, input behavior, system locking, macOS host compatibility, or Linux daemon integration. Before releasing, manually check those behaviors, offline operation, timezones, sleep/resume, display scaling, multiple monitors, and installation/removal on each supported OS.

Current development environment: universal macOS compilation/signature verification and Node checks are possible. Windows execution and Linux GTK/WebKit compilation are unavailable locally; CI results must be reviewed separately. No platform package should be called release-ready solely because its source was added.
