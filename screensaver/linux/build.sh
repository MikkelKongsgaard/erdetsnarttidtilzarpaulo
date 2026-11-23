#!/bin/sh
set -eu
cd "$(dirname "$0")/../.."
mkdir -p build/linux/ui
cc -std=c11 -D_POSIX_C_SOURCE=200809L -Wall -Wextra -Werror screensaver/linux/main.c -o build/linux/zar-paulo $(pkg-config --cflags --libs gtk+-3.0 webkit2gtk-4.1 x11)
cp screensaver/shared/index.html countdown.js build/linux/ui/
