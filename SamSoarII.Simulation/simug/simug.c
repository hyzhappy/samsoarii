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
typedef void(*vsii32Dllfun)(char*, int, uint32_t*);
typedef void(*vsii64Dllfun)(char*, int, uint64_t*);
typedef void(*vsi64Dllfun)(char*, uint64_t*);
typedef void(*vsif64Dllfun)(char*, int, double*);
typedef void(*vsiiv32Dllfun)(char*, int, uint32_t);
typedef void(*vsiiv64Dllfun)(char*, int, uint64_t);
typedef void(*vsifv64Dllfun)(char*, int, double);
typedef void(*vsiiDllfun)(char*, int, int);
typedef void(*vsiDllfun)(char*, int);
typedef void(*vsDllfun)(char*);

static char cmd[1024];
static HINSTANCE hdll;
static vDllfun dfBeforeRunLadder;
static vDllfun dfRunLadder;
static vDllfun dfAfterRunLadder;
static vsii32Dllfun dfSetBit;
static vsii32Dllfun dfSetWord;
static vsii64Dllfun dfSetDWord;
static vsif64Dllfun dfSetFloat;
static vsii32Dllfun dfGetBit;
static vsii32Dllfun dfGetWord;
static vsii64Dllfun dfGetDWord;
static vsif64Dllfun dfGetFloat;
static vsi64Dllfun dfGetFeq;
static vsiiDllfun dfSetEnable;
static vDllfun dfInitDataPoint;
static vsiiv32Dllfun dfAddBitDataPoint;
static vsiiv32Dllfun dfAddWordDataPoint;
static vsiiv64Dllfun dfAddDWordDataPoint;
static vsifv64Dllfun dfAddFloatDataPoint;
static vsiiv32Dllfun dfRemoveBitDataPoint;
static vsiiv32Dllfun dfRemoveWordDataPoint;
static vsiiv64Dllfun dfRemoveDWordDataPoint;
static vsifv64Dllfun dfRemoveFloatDataPoint;
static vsiDllfun dfAddViewInput;
static vsiDllfun dfAddViewOutput;
static vsiDllfun dfRemoveViewInput;
static vsiDllfun dfRemoveViewOutput;
static vsiiDllfun dfRunData;

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
	dfSetEnable = (vsiiDllfun)GetProcAddress(hdll, "SetEnable");
	if (dfSetEnable == NULL)
	{
		FreeLibrary(hdll);
		return 13;
	}
	dfInitDataPoint = (vDllfun)GetProcAddress(hdll, "InitDataPoint");
	if (dfInitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 14;
	}
	dfAddBitDataPoint = (vsiiv32Dllfun)GetProcAddress(hdll, "AddBitDataPoint");
	if (dfAddBitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 15;
	}
	dfAddWordDataPoint = (vsiiv32Dllfun)GetProcAddress(hdll, "AddWordDataPoint");
	if (dfAddWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 16;
	}
	dfAddDWordDataPoint = (vsiiv64Dllfun)GetProcAddress(hdll, "AddDoubleWordDataPoint");
	if (dfAddDWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 17;
	}
	dfAddFloatDataPoint = (vsifv64Dllfun)GetProcAddress(hdll, "AddFloatDataPoint");
	if (dfAddFloatDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 18;
	}
	dfRemoveBitDataPoint = (vsiiv32Dllfun)GetProcAddress(hdll, "RemoveBitDataPoint");
	if (dfRemoveBitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 20;
	}
	dfRemoveWordDataPoint = (vsiiv32Dllfun)GetProcAddress(hdll, "RemoveWordDataPoint");
	if (dfRemoveWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 21;
	}
	dfRemoveDWordDataPoint = (vsiiv64Dllfun)GetProcAddress(hdll, "RemoveDoubleWordDataPoint");
	if (dfRemoveDWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 22;
	}
	dfRemoveFloatDataPoint = (vsifv64Dllfun)GetProcAddress(hdll, "RemoveFloatDataPoint");
	if (dfRemoveFloatDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 23;
	}
	dfAddViewInput = (vsiDllfun)GetProcAddress(hdll, "AddViewInput");
	if (dfAddViewInput == NULL)
	{
		FreeLibrary(hdll);
		return 25;
	}
	dfAddViewOutput = (vsiDllfun)GetProcAddress(hdll, "AddViewOutput");
	if (dfAddViewOutput == NULL)
	{
		FreeLibrary(hdll);
		return 26;
	}
	dfRemoveViewInput = (vsiDllfun)GetProcAddress(hdll, "RemoveViewInput");
	if (dfRemoveViewInput == NULL)
	{
		FreeLibrary(hdll);
		return 27;
	}
	dfRemoveViewOutput = (vsiDllfun)GetProcAddress(hdll, "RemoveViewOutput");
	if (dfRemoveViewOutput == NULL)
	{
		FreeLibrary(hdll);
		return 28;
	}
	dfRunData = (vsiiDllfun)GetProcAddress(hdll, "RunData");
	if (dfRunData == NULL)
	{
		FreeLibrary(hdll);
		return 29;
	}
	dfBeforeRunLadder = (vDllfun)GetProcAddress(hdll, "BeforeRunLadder");
	if (dfRunData == NULL)
	{
		FreeLibrary(hdll);
		return 30;
	}
	dfAfterRunLadder = (vDllfun)GetProcAddress(hdll, "AfterRunLadder");
	if (dfRunData == NULL)
	{
		FreeLibrary(hdll);
		return 31;
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

EXPORT void InitDataPoint()
{
	dfInitDataPoint();
}

EXPORT void AddBitDataPoint(char* name, int time, uint32_t value)
{
	dfAddBitDataPoint(name, time, value);
}

EXPORT void AddWordDataPoint(char* name, int time, uint32_t value)
{
	dfAddWordDataPoint(name, time, value);
}

EXPORT void AddDoubleWordDataPoint(char* name, int time, uint64_t value)
{
	dfAddDWordDataPoint(name, time, value);
}

EXPORT void AddFloatDataPoint(char* name, int time, double value)
{
	dfAddFloatDataPoint(name, time, value);
}

EXPORT void RemoveBitDataPoint(char* name, int time, uint32_t value)
{
	dfRemoveBitDataPoint(name, time, value);
}

EXPORT void RemoveWordDataPoint(char* name, int time, uint32_t value)
{
	dfRemoveWordDataPoint(name, time, value);
}

EXPORT void RemoveDoubleWordDataPoint(char* name, int time, uint64_t value)
{
	dfRemoveDWordDataPoint(name, time, value);
}

EXPORT void RemoveFloatDataPoint(char* name, int time, double value)
{
	dfRemoveFloatDataPoint(name, time, value);
}

EXPORT void AddViewInput(char* name, int type)
{
	dfAddViewInput(name, type);
}

EXPORT void AddViewOutput(char* name, int type)
{
	dfAddViewOutput(name, type);
}

EXPORT void RemoveViewInput(char* name, int type)
{
	dfRemoveViewInput(name, type);
}

EXPORT void RemoveViewOutput(char* name, int type)
{
	dfRemoveViewOutput(name, type);
}

EXPORT void RunData(char* outputFile, int starttime, int endtime)
{
	dfRunData(outputFile, starttime, endtime);
}
