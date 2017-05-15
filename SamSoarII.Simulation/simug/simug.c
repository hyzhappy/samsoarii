#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif // BUILD_DLL

#ifdef __cplusplus
#define EXPORT extern "C" DLL_EXPORT __stdcall
#else
#define EXPORT DLL_EXPORT __stdcall
#endif

typedef void(*vDllfun)(void);
typedef void(*viDllfun)(int);
typedef void(*vsii32Dllfun)(char*, int, int32_t*);
typedef void(*vsii64Dllfun)(char*, int, int64_t*);
typedef void(*vsi64Dllfun)(char*, int64_t*);
typedef void(*vsiv64Dllfun)(char*, int64_t);
typedef void(*vsif64Dllfun)(char*, int, double*);
typedef void(*vsiiv32Dllfun)(char*, int, int32_t);
typedef void(*vsiiv64Dllfun)(char*, int, int64_t);
typedef void(*vsifv64Dllfun)(char*, int, double);
typedef void(*vsiiDllfun)(char*, int, int);
typedef void(*vsiDllfun)(char*, int);
typedef void(*vsDllfun)(char*);
typedef int(*iDllfun)(void);

static char cmd[1024];
static HINSTANCE hdll;
static vDllfun dfBeforeRunLadder;
static vDllfun dfRunLadder;
static vDllfun dfAfterRunLadder;
static vDllfun dfInitRunLadder;
static vsii32Dllfun dfSetBit;
static vsii32Dllfun dfSetWord;
static vsii64Dllfun dfSetDWord;
static vsif64Dllfun dfSetFloat;
static vsii32Dllfun dfGetBit;
static vsii32Dllfun dfGetWord;
static vsii64Dllfun dfGetDWord;
static vsif64Dllfun dfGetFloat;
static vsi64Dllfun dfGetFeq;
static vsiv64Dllfun dfSetFeq;
static vsiiDllfun dfSetEnable;
static viDllfun dfInitClock;
static iDllfun dfGetClock;
static viDllfun dfSetClockRate;
static viDllfun dfSetBaseBit;

EXPORT void CreateDll(char* simucPath, char* simufPath, char* simudllPath, char* simuaPath)
{
	FILE* f = fopen("simucmd.bat", "w");
	sprintf(cmd, "i686-w64-mingw32-gcc -c -DBUILD_DLL %s -o simuc.o", simucPath);
	fprintf(f, "%s\n", cmd);
	//system(cmd);
	sprintf(cmd, "i686-w64-mingw32-gcc -c -DBUILD_DLL %s -o simuf.o", simufPath);
	fprintf(f, "%s\n", cmd);
	//system(cmd);
	sprintf(cmd, "i686-w64-mingw32-gcc -c -DBUILD_DLL simug\\simulib.c -o simulib.o");
	fprintf(f, "%s\n", cmd);
	//system(cmd);
	sprintf(cmd, "i686-w64-mingw32-gcc -shared -o %s simuc.o simuf.o simulib.o -Wl,--kill-at", simudllPath);
	fprintf(f, "%s\n", cmd);
	//system(cmd);
	//system("erase simuc.o");
	fprintf(f, "erase simuc.o\n");
	//system("erase simuf.o");
	fprintf(f, "erase simuf.o\n");
	//system("erase simulib.o");
	fprintf(f, "erase simulib.o\n");
	//fprintf(f, "erase %d\n", simcPath);
	//fprintf(f, "erase %d\n", simcfPath);
	fclose(f);
	system("simucmd.bat");
}

static char dllPath[256];
EXPORT int LoadDll(char* simudllPath)
{
	strcpy(dllPath, simudllPath);
	hdll = LoadLibrary(simudllPath);
	if (hdll == NULL)
	{
		FreeLibrary(hdll);
		return 1;
	}
	dfRunLadder = (vDllfun)GetProcAddress(hdll, "RunLadder");
	if (dfRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 2;
	}
	dfGetBit = (vsii32Dllfun)GetProcAddress(hdll, "GetBit");
	if (dfGetBit == NULL)
	{
		FreeLibrary(hdll);
		return 3;
	}
	dfGetWord = (vsii32Dllfun)GetProcAddress(hdll, "GetWord");
	if (dfGetWord == NULL)
	{
		FreeLibrary(hdll);
		return 4;
	}
	dfGetDWord = (vsii64Dllfun)GetProcAddress(hdll, "GetDoubleWord");
	if (dfGetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 5;
	}
	dfGetFloat = (vsif64Dllfun)GetProcAddress(hdll, "GetFloat");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 6;
	}
	dfGetFeq = (vsi64Dllfun)GetProcAddress(hdll, "GetFeq");
	if (dfGetFeq == NULL)
	{
		FreeLibrary(hdll);
		return 7;
	}
	dfSetBit = (vsii32Dllfun)GetProcAddress(hdll, "SetBit");
	if (dfSetBit == NULL)
	{
		FreeLibrary(hdll);
		return 8;
	}
	dfSetWord = (vsii32Dllfun)GetProcAddress(hdll, "SetWord");
	if (dfSetWord == NULL)
	{
		FreeLibrary(hdll);
		return 9;
	}
	dfSetDWord = (vsii64Dllfun)GetProcAddress(hdll, "SetDoubleWord");
	if (dfSetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 10;
	}
	dfSetFloat = (vsif64Dllfun)GetProcAddress(hdll, "SetFloat");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 11;
	}
	dfSetFeq = (vsiv64Dllfun)GetProcAddress(hdll, "SetFeq");
	if (dfSetFeq == NULL)
	{
		FreeLibrary(hdll);
		return 12;
	}
	dfSetEnable = (vsiiDllfun)GetProcAddress(hdll, "SetEnable");
	if (dfSetEnable == NULL)
	{
		FreeLibrary(hdll);
		return 13;
	}
	dfBeforeRunLadder = (vDllfun)GetProcAddress(hdll, "BeforeRunLadder");
	if (dfBeforeRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 14;
	}
	dfAfterRunLadder = (vDllfun)GetProcAddress(hdll, "AfterRunLadder");
	if (dfAfterRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 15;
	}
	dfInitRunLadder = (vDllfun)GetProcAddress(hdll, "InitRunLadder");
	if (dfInitRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 16;
	}
	dfInitClock = (viDllfun)GetProcAddress(hdll, "InitClock");
	if (dfInitClock == NULL)
	{
		FreeLibrary(hdll);
		return 17;
	}
	dfGetClock = (iDllfun)GetProcAddress(hdll, "GetClock");
	if (dfGetClock == NULL)
	{
		FreeLibrary(hdll);
		return 18;
	}
	dfSetClockRate = (viDllfun)GetProcAddress(hdll, "SetClockRate");
	if (dfSetClockRate == NULL)
	{
		FreeLibrary(hdll);
		return 19;
	}
	dfSetBaseBit = (viDllfun)GetProcAddress(hdll, "SetBaseBit");
	if (dfSetBaseBit == NULL)
	{
		FreeLibrary(hdll);
		return 20;
	}
	return 0;
}

EXPORT void FreeDll()
{
	FreeLibrary(hdll);
	sprintf(cmd, "erase %s", dllPath);
	system(cmd);
}

EXPORT void RunLadder()
{
	dfRunLadder();
}

EXPORT void InitRunLadder()
{
	dfInitRunLadder();
}

EXPORT void BeforeRunLadder()
{
	dfBeforeRunLadder();
}

EXPORT void AfterRunLadder()
{
	dfAfterRunLadder();
}

EXPORT void GetBit(char* name, int size, uint32_t* output)
{
	dfGetBit(name, size, output);
}

EXPORT void GetWord(char* name, int size, uint32_t* output)
{
	dfGetWord(name, size, output);
}

EXPORT void GetDoubleWord(char* name, int size, uint64_t* output)
{
	dfGetDWord(name, size, output);
}

EXPORT void GetFloat(char* name, int size, double* output)
{
	dfGetFloat(name, size, output);
}

EXPORT void GetFeq(char* name, uint64_t* output)
{
	dfGetFeq(name, output);
}

EXPORT void SetFeq(char* name, int64_t input)
{
	dfSetFeq(name, input);
}

EXPORT void SetBit(char* name, int size, uint32_t* input)
{
	dfSetBit(name, size, input);
}

EXPORT void SetWord(char* name, int size, uint32_t* input)
{
	dfSetWord(name, size, input);
}

EXPORT void SetDoubleWord(char* name, int size, uint64_t* input)
{
	dfSetDWord(name, size, input);
}

EXPORT void SetFloat(char* name, int size, double* input)
{
	dfSetFloat(name, size, input);
}

EXPORT void SetEnable(char* name, int size, int value)
{
	dfSetEnable(name, size, value);
}

EXPORT void InitClock(int _time)
{
	dfInitClock(_time);
}

EXPORT int GetClock()
{
	return dfGetClock();
}

EXPORT void SetClockRate(int _rate)
{
	dfSetClockRate(_rate);
}

EXPORT void SetBaseBit(int _basebit)
{
	dfSetBaseBit(_basebit);
}
