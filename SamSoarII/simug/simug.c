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
typedef void(*vsii16Dllfun)(char*, int, uint16_t*);
typedef void(*vsii32Dllfun)(char*, int, uint32_t*);
typedef void(*vsif32Dllfun)(char*, int, float*);
typedef void(*vsid64Dllfun)(char*, int, double*);
typedef void(*vsiiDllfun)(char*, int, int);
typedef void(*vsDllfun)(char*);

static char cmd[1024];
static HINSTANCE hdll;
static vDllfun dfRunLadder;
static vsii32Dllfun dfSetBit;
static vsii16Dllfun dfSetWord;
static vsii32Dllfun dfSetDWord;
static vsif32Dllfun dfSetFloat;
static vsid64Dllfun dfSetDouble;
static vsii32Dllfun dfGetBit;
static vsii16Dllfun dfGetWord;
static vsii32Dllfun dfGetDWord;
static vsif32Dllfun dfGetFloat;
static vsid64Dllfun dfGetDouble;
static vsiiDllfun dfSetEnable;
static vDllfun dfInitDataPoint;
static vsii32Dllfun dfAddBitDataPoint;
static vsii16Dllfun dfAddWordDataPoint;
static vsii32Dllfun dfAddDWordDataPoint;
static vsif32Dllfun dfAddFloatDataPoint;
static vsid64Dllfun dfAddDoubleDataPoint;
static vsii32Dllfun dfRemoveBitDataPoint;
static vsii16Dllfun dfRemoveWordDataPoint;
static vsii32Dllfun dfRemoveDWordDataPoint;
static vsif32Dllfun dfRemoveFloatDataPoint;
static vsid64Dllfun dfRemoveDoubleDataPoint;
static vsDllfun dfRunData;

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
	fclose(f);
	system("simucmd.bat");
}

EXPORT int LoadDll(char* simudllPath)
{
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
	dfGetWord = (vsii16Dllfun)GetProcAddress(hdll, "GetWord");
	if (dfGetWord == NULL)
	{
		FreeLibrary(hdll);
		return 4;
	}
	dfGetDWord = (vsii32Dllfun)GetProcAddress(hdll, "GetDoubleWord");
	if (dfGetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 5;
	}
	dfGetFloat = (vsif32Dllfun)GetProcAddress(hdll, "GetFloat");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 6;
	}
	dfGetDouble = (vsid64Dllfun)GetProcAddress(hdll, "GetDouble");
	if (dfGetDouble == NULL)
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
	dfSetWord = (vsii16Dllfun)GetProcAddress(hdll, "SetWord");
	if (dfSetWord == NULL)
	{
		FreeLibrary(hdll);
		return 9;
	}
	dfSetDWord = (vsii32Dllfun)GetProcAddress(hdll, "SetDoubleWord");
	if (dfSetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 10;
	}
	dfSetFloat = (vsif32Dllfun)GetProcAddress(hdll, "SetFloat");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 11;
	}
	dfSetDouble = (vsid64Dllfun)GetProcAddress(hdll, "SetDouble");
	if (dfGetDouble == NULL)
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
	dfInitDataPoint = (vDllfun)GetProcAddress(hdll, "InitDataPoint");
	if (dfInitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 14;
	}
	dfAddBitDataPoint = (vsii32Dllfun)GetProcAddress(hdll, "AddBitDataPoint");
	if (dfAddBitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 15;
	}
	dfAddWordDataPoint = (vsii16Dllfun)GetProcAddress(hdll, "AddWordDataPoint");
	if (dfAddWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 16;
	}
	dfAddDWordDataPoint = (vsii32Dllfun)GetProcAddress(hdll, "AddDoubleWordDataPoint");
	if (dfAddDWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 17;
	}
	dfAddFloatDataPoint = (vsif32Dllfun)GetProcAddress(hdll, "AddFloatDataPoint");
	if (dfAddFloatDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 18;
	}
	dfAddDoubleDataPoint = (vsid64Dllfun)GetProcAddress(hdll, "AddDoubleDataPoint");
	if (dfAddDoubleDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 19;
	}
	dfRemoveBitDataPoint = (vsii32Dllfun)GetProcAddress(hdll, "RemoveBitDataPoint");
	if (dfRemoveBitDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 20;
	}
	dfRemoveWordDataPoint = (vsii16Dllfun)GetProcAddress(hdll, "RmoveWordDataPoint");
	if (dfRemoveWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 21;
	}
	dfRemoveDWordDataPoint = (vsii32Dllfun)GetProcAddress(hdll, "RemoveDoubleWordDataPoint");
	if (dfRemoveDWordDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 22;
	}
	dfRemoveFloatDataPoint = (vsif32Dllfun)GetProcAddress(hdll, "RemoveFloatDataPoint");
	if (dfRemoveFloatDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 23;
	}
	dfRemoveDoubleDataPoint = (vsid64Dllfun)GetProcAddress(hdll, "RemoveDoubleDataPoint");
	if (dfRemoveDoubleDataPoint == NULL)
	{
		FreeLibrary(hdll);
		return 24;
	}
	dfRunData = (vsDllfun)GetProcAddress(hdll, "RunData");
	if (dfRunData == NULL)
	{
		FreeLibrary(hdll);
		return 25;
	}
	return 0;
}

EXPORT void RunLadder()
{
	dfRunLadder();
}

EXPORT void GetBit(char* name, int size, uint32_t* output)
{
	dfGetBit(name, size, output);
}

EXPORT void GetWord(char* name, int size, uint16_t* output)
{
	dfGetWord(name, size, output);
}

EXPORT void GetDoubleWord(char* name, int size, uint32_t* output)
{
	dfGetDWord(name, size, output);
}

EXPORT void GetFloat(char* name, int size, float* output)
{
	dfGetFloat(name, size, output);
}

EXPORT void GetDouble(char* name, int size, double* output)
{
	dfGetDouble(name, size, output);
}

EXPORT void SetBit(char* name, int size, uint32_t* input)
{
	dfSetBit(name, size, input);
}

EXPORT void SetWord(char* name, int size, uint16_t* input)
{
	dfSetWord(name, size, input);
}

EXPORT void SetDoubleWord(char* name, int size, uint32_t* input)
{
	dfSetDWord(name, size, input);
}

EXPORT void SetFloat(char* name, int size, float* input)
{
	dfSetFloat(name, size, input);
}

EXPORT void SetDouble(char* name, int size, double* input)
{
	dfSetDouble(name, size, input);
}

EXPORT void SetEnable(char* name, int size, int value)
{
	dfSetEnable(name, size, value);
}

EXPORT void InitDataPoint()
{
	dfInitDataPoint();
}

EXPORT void AddBitDataPoint(char* name, int time, int value)
{
	dfSetBitDataPoint(name, time, value);
}

EXPORT void AddWordDataPoint(char* name, int time, int value)
{
	dfSetWordDataPoint(name, time, value);
}

EXPORT void AddDoubleWordDataPoint(char* name, int time, int value)
{
	dfSetDWordDataPoint(name, time, value);
}

EXPORT void AddFloatDataPoint(char* name, int time, float value)
{
	dfSetFloatDataPoint(name, time, value);
}

EXPORT void AddDoubleDataPoint(char* name, int time, double value)
{
	dfSetDoubleDataPoint(name, time, value);
}

EXPORT void RemoveBitDataPoint(char* name, int time, int value)
{
	dfRemoveBitDataPoint(name, time, value);
}

EXPORT void RemoveWordDataPoint(char* name, int time, int value)
{
	dfRemoveWordDataPoint(name, time, value);
}

EXPORT void RemoveDoubleWordDataPoint(char* name, int time, int value)
{
	dfRemoveDWordDataPoint(name, time, value);
}

EXPORT void RemoveFloatDataPoint(char* name, int time, float value)
{
	dfRemoveFloatDataPoint(name, time, value);
}

EXPORT void RemoveDoubleDataPoint(char* name, int time, double value)
{
	dfRemoveDoubleDataPoint(name, time, value);
}

EXPORT void RunData(char* fileName)
{
	dfRunData(fileName);
}