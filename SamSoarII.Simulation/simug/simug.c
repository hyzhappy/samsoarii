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
typedef void(*viiDllfun)(int, int);
typedef int(*isii32Dllfun)(char*, int, int32_t*);
typedef int(*isii64Dllfun)(char*, int, int64_t*);
typedef int(*isi64Dllfun)(char*, int64_t*);
typedef int(*isiv64Dllfun)(char*, int64_t);
typedef int(*isif64Dllfun)(char*, int, double*);
typedef int(*isiiv32Dllfun)(char*, int, int32_t);
typedef int(*isiiv64Dllfun)(char*, int, int64_t);
typedef int(*isifv64Dllfun)(char*, int, double);
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
static isii32Dllfun dfSetBit;
static isii32Dllfun dfSetWord;
static isii64Dllfun dfSetDWord;
static isif64Dllfun dfSetFloat;
static isii32Dllfun dfGetBit;
static isii32Dllfun dfGetWord;
static isii64Dllfun dfGetDWord;
static isif64Dllfun dfGetFloat;
static isi64Dllfun dfGetFeq;
static isiv64Dllfun dfSetFeq;
static vsiiDllfun dfSetEnable;
static viDllfun dfInitClock;
static iDllfun dfGetClock;
static viDllfun dfSetClockRate;
static viDllfun dfSetBaseBit;
static iDllfun dfGetCallCount;
static iDllfun dfGetBPAddr;
static viiDllfun dfSetBPAddr;
static viiDllfun dfSetCPAddr;
static viDllfun dfSetBPEnable;
static iDllfun dfGetBPPause;
static viDllfun dfSetBPPause;
static vDllfun dfMoveStep;
static vDllfun dfCallStep;
static viDllfun dfJumpTo;
static vDllfun dfJumpOut;

EXPORT void CreateDll(char* simucPath, char* simufPath, char* simudllPath, char* simuaPath)
{
	sprintf(cmd, "Compiler\\tcc\\tcc simug\\simulib.c %s %s -o %s -shared -DBUILD_DLL",
		simucPath, simufPath, simudllPath);
	system(cmd);
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
	dfRunLadder = (vDllfun)GetProcAddress(hdll, "_RunLadder@0");
	if (dfRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 2;
	}
	dfGetBit = (isii32Dllfun)GetProcAddress(hdll, "_GetBit@12");
	if (dfGetBit == NULL)
	{
		FreeLibrary(hdll);
		return 3;
	}
	dfGetWord = (isii32Dllfun)GetProcAddress(hdll, "_GetWord@12");
	if (dfGetWord == NULL)
	{
		FreeLibrary(hdll);
		return 4;
	}
	dfGetDWord = (isii64Dllfun)GetProcAddress(hdll, "_GetDoubleWord@12");
	if (dfGetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 5;
	}
	dfGetFloat = (isif64Dllfun)GetProcAddress(hdll, "_GetFloat@12");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 6;
	}
	dfGetFeq = (isi64Dllfun)GetProcAddress(hdll, "_GetFeq@8");
	if (dfGetFeq == NULL)
	{
		FreeLibrary(hdll);
		return 7;
	}
	dfSetBit = (isii32Dllfun)GetProcAddress(hdll, "_SetBit@12");
	if (dfSetBit == NULL)
	{
		FreeLibrary(hdll);
		return 8;
	}
	dfSetWord = (isii32Dllfun)GetProcAddress(hdll, "_SetWord@12");
	if (dfSetWord == NULL)
	{
		FreeLibrary(hdll);
		return 9;
	}
	dfSetDWord = (isii64Dllfun)GetProcAddress(hdll, "_SetDoubleWord@12");
	if (dfSetDWord == NULL)
	{
		FreeLibrary(hdll);
		return 10;
	}
	dfSetFloat = (isif64Dllfun)GetProcAddress(hdll, "_SetFloat@12");
	if (dfGetFloat == NULL)
	{
		FreeLibrary(hdll);
		return 11;
	}
	dfSetFeq = (isiv64Dllfun)GetProcAddress(hdll, "_SetFeq@12");
	if (dfSetFeq == NULL)
	{
		FreeLibrary(hdll);
		return 12;
	}
	dfSetEnable = (vsiiDllfun)GetProcAddress(hdll, "_SetEnable@12");
	if (dfSetEnable == NULL)
	{
		FreeLibrary(hdll);
		return 13;
	}
	dfBeforeRunLadder = (vDllfun)GetProcAddress(hdll, "_BeforeRunLadder@0");
	if (dfBeforeRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 14;
	}
	dfAfterRunLadder = (vDllfun)GetProcAddress(hdll, "_AfterRunLadder@0");
	if (dfAfterRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 15;
	}
	dfInitRunLadder = (vDllfun)GetProcAddress(hdll, "_InitRunLadder@0");
	if (dfInitRunLadder == NULL)
	{
		FreeLibrary(hdll);
		return 16;
	}
	dfInitClock = (viDllfun)GetProcAddress(hdll, "_InitClock@4");
	if (dfInitClock == NULL)
	{
		FreeLibrary(hdll);
		return 17;
	}
	dfGetClock = (iDllfun)GetProcAddress(hdll, "_GetClock@0");
	if (dfGetClock == NULL)
	{
		FreeLibrary(hdll);
		return 18;
	}
	dfSetClockRate = (viDllfun)GetProcAddress(hdll, "_SetClockRate@4");
	if (dfSetClockRate == NULL)
	{
		FreeLibrary(hdll);
		return 19;
	}
	dfSetBaseBit = (viDllfun)GetProcAddress(hdll, "_SetBaseBit@4");
	if (dfSetBaseBit == NULL)
	{
		FreeLibrary(hdll);
		return 20;
	}
	dfGetCallCount = (iDllfun)GetProcAddress(hdll, "_GetCallCount@0");
	if (dfGetCallCount == NULL)
	{
		FreeLibrary(hdll);
		return 21;
	}
	dfGetBPAddr = (iDllfun)GetProcAddress(hdll, "_GetBPAddr@0");
	if (dfGetBPAddr == NULL)
	{
		FreeLibrary(hdll);
		return 22;
	}
	dfSetBPAddr = (viiDllfun)GetProcAddress(hdll, "_SetBPAddr@8");
	if (dfSetBPAddr == NULL)
	{
		FreeLibrary(hdll);
		return 23;
	}
	dfSetCPAddr = (viiDllfun)GetProcAddress(hdll, "_SetCPAddr@8");
	if (dfSetCPAddr == NULL)
	{
		FreeLibrary(hdll);
		return 24;
	}
	dfGetBPPause = (iDllfun)GetProcAddress(hdll, "_GetBPPause@0");
	if (dfGetBPPause == NULL)
	{
		FreeLibrary(hdll);
		return 25;
	}
	dfSetBPPause = (viDllfun)GetProcAddress(hdll, "_SetBPPause@4");
	if (dfSetBPPause == NULL)
	{
		FreeLibrary(hdll);
		return 26;
	}
	dfMoveStep = (vDllfun)GetProcAddress(hdll, "_MoveStep@0");
	if (dfMoveStep == NULL)
	{
		FreeLibrary(hdll);
		return 27;
	}
	dfCallStep = (vDllfun)GetProcAddress(hdll, "_CallStep@0");
	if (dfCallStep == NULL)
	{
		FreeLibrary(hdll);
		return 28;
	}
	dfJumpTo = (viDllfun)GetProcAddress(hdll, "_JumpTo@4");
	if (dfJumpTo == NULL)
	{
		FreeLibrary(hdll);
		return 29;
	}
	dfJumpOut = (vDllfun)GetProcAddress(hdll, "_JumpOut@0");
	if (dfJumpOut == NULL)
	{
		FreeLibrary(hdll);
		return 30;
	}
	dfSetBPEnable = (viDllfun)GetProcAddress(hdll, "_SetBPEnable@4");
	if (dfSetBPEnable == NULL)
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

EXPORT int GetBit(char* name, int size, uint32_t* output)
{
	return dfGetBit(name, size, output);
}

EXPORT int GetWord(char* name, int size, uint32_t* output)
{
	return dfGetWord(name, size, output);
}

EXPORT int GetDoubleWord(char* name, int size, uint64_t* output)
{
	return dfGetDWord(name, size, output);
}

EXPORT int GetFloat(char* name, int size, double* output)
{
	return dfGetFloat(name, size, output);
}

EXPORT int GetFeq(char* name, uint64_t* output)
{
	return dfGetFeq(name, output);
}

EXPORT int SetFeq(char* name, int64_t input)
{
	return dfSetFeq(name, input);
}

EXPORT int SetBit(char* name, int size, uint32_t* input)
{
	return dfSetBit(name, size, input);
}

EXPORT int SetWord(char* name, int size, uint32_t* input)
{
	return dfSetWord(name, size, input);
}

EXPORT int SetDoubleWord(char* name, int size, uint64_t* input)
{
	return dfSetDWord(name, size, input);
}

EXPORT int SetFloat(char* name, int size, double* input)
{
	return dfSetFloat(name, size, input);
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

EXPORT int GetCallCount()
{
	return dfGetCallCount();
}

EXPORT int GetBPAddr()
{
	return dfGetBPAddr();
}

EXPORT void SetBPAddr(int bpaddr, int islock)
{
	dfSetBPAddr(bpaddr, islock);
}

EXPORT void SetCPAddr(int cpaddr, int cpmsg)
{
	dfSetCPAddr(cpaddr, cpmsg);
}

EXPORT void SetBPEnable(int bpenable)
{
	dfSetBPEnable(bpenable);
}

EXPORT int GetBPPause()
{
	return dfGetBPPause();
}

EXPORT void SetBPPause(int bppause)
{
	dfSetBPPause(bppause);
}

EXPORT void MoveStep()
{
	dfMoveStep();
}

EXPORT void CallStep()
{
	dfCallStep();
}

EXPORT void JumpTo(int _bpaddr)
{
	dfJumpTo(_bpaddr);
}

EXPORT void JumpOut()
{
	dfJumpOut();
}
