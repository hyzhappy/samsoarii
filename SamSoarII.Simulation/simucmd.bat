i686-w64-mingw32-gcc -c -DBUILD_DLL C:\Samkoon\SamSoarII\Debug\simug\simuc.c -o simuc.o
i686-w64-mingw32-gcc -c -DBUILD_DLL C:\Samkoon\SamSoarII\Debug\simug\simuf.c -o simuf.o
i686-w64-mingw32-gcc -c -DBUILD_DLL simug\simulib.c -o simulib.o
i686-w64-mingw32-gcc -shared -o C:\Samkoon\SamSoarII\Debug\simuc.dll simuc.o simuf.o simulib.o -Wl,--kill-at
erase simuc.o
erase simuf.o
erase simulib.o
