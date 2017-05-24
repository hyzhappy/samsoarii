i686-w64-mingw32-gcc -c -DBUILD_DLL usbdll.c -o usbdll.o
i686-w64-mingw32-gcc -shared -o plcusb.dll usbdll.o libusb-1.0.dll -Wl,--kill-at
