#include <stdio.h>
#include <stdlib.h>
#include <string.h>

char cmd[1024];

void main(int argc, char** argv)
{
	sprintf(cmd, "mingw32-gcc -c -DBUILD_DLL %s -o simuc.o", argv[1]);
	system(cmd);
	sprintf(cmd, "mingw32-gcc -c -DBUILD_DLL %s -o simuf.o", argv[2]);
	system(cmd);
	sprintf(cmd, "mingw32-gcc -shared -o %s simuc.o simuf.o -Wl,--kill-at,--out-implib,%s", argv[3], argv[4]);
	system(cmd);
	system("delete simuc.o");
	system("delete simuf.o");
}