#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>

typedef void(*vfunc)(void);

vfunc dfOpen;

int main(int argc, char** argv)
{
	HINSTANCE hDll = LoadLibrary("plcusb.dll");
	dfOpen = (vfunc)GetProcAddress(hDll, "Open");
	dfOpen();
}