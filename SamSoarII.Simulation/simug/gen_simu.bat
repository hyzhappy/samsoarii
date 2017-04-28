i686-w64-mingw32-gcc -c -DBUILD_DLL usbdll.c -o usbdll.o
i686-w64-mingw32-gcc -shared -o ../simu.dll simug.o -Wl,--kill-at
