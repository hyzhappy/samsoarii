i686-w64-mingw32-gcc -c -DBUILD_DLL simug.c -o simug.o
i686-w64-mingw32-gcc -shared -o ../simu.dll simug.o -Wl,--kill-at
