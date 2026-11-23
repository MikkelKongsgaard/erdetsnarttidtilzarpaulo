#!/bin/sh
set -eu
cd "$(dirname "$0")/../.."
destination="${ZAR_BUILD_DIR:-build/macos}"
out="$destination/ZarPaulo.saver/Contents"
mkdir -p "$out/MacOS" "$out/Resources"
clang -fmodules-cache-path=build/module-cache -fobjc-arc -fmodules -bundle -arch arm64 -arch x86_64 -mmacosx-version-min=11.0 -framework ScreenSaver -framework WebKit screensaver/macos/ZarPaulo.m -o "$out/MacOS/ZarPaulo"
cp screensaver/shared/index.html countdown.js "$out/Resources/"
cat > "$out/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd"><plist version="1.0"><dict><key>CFBundleIdentifier</key><string>dk.erdetsnarttidtilzarpaulo.screensaver</string><key>CFBundleName</key><string>Zar Paulo</string><key>CFBundleExecutable</key><string>ZarPaulo</string><key>CFBundlePackageType</key><string>BNDL</string><key>CFBundleVersion</key><string>1</string><key>CFBundleShortVersionString</key><string>0.1.0</string><key>NSPrincipalClass</key><string>ZarPauloView</string><key>LSMinimumSystemVersion</key><string>11.0</string></dict></plist>
PLIST
xattr -cr "$destination/ZarPaulo.saver"
codesign --force --sign - "$destination/ZarPaulo.saver"
codesign --verify --strict "$destination/ZarPaulo.saver"
lipo "$out/MacOS/ZarPaulo" -verify_arch arm64 x86_64
