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
typedef void(*vv8Dllfun)(int8_t);
typedef void(*viiDllfun)(int, int);
typedef void(*viv8Dllfun)(int, int8_t);
typedef void*(*pDllfun)(void);
typedef int8_t(*v8Dllfun)(void);
typedef int(*iDllfun)(void);
typedef int(*ipDllfun)(int*);
typedef int(*isi32Dllfun)(char*, int32_t*);
typedef int(*isv32Dllfun)(char*, int32_t);
typedef int(*isii8Dllfun)(char*, int, int8_t*);
typedef int(*isii16Dllfun)(char*, int, int16_t*);
typedef int(*isii32Dllfun)(char*, int, int32_t*);
typedef int(*isif32Dllfun)(char*, int, float*);
typedef int(*isiv8Dllfun)(char*, int, int8_t);

static char edata[65536];
static char cmd[1024];
static char dllPath[256];
static HINSTANCE hdll;
static vDllfun dfBeforeRunLadder;
static vDllfun dfRunLadder;
static vDllfun dfAfterRunLadder;
static vDllfun dfInitRunLadder;
static isii8Dllfun dfSetBit;
static isii16Dllfun dfSetWord;
static isii32Dllfun dfSetDWord;
static isif32Dllfun dfSetFloat;
static isii8Dllfun dfGetBit;
static isii16Dllfun dfGetWord;
static isii32Dllfun dfGetDWord;
static isif32Dllfun dfGetFloat;
static isi32Dllfun dfGetFeq;
static isv32Dllfun dfSetFeq;
static isiv8Dllfun dfSetEnable;
static viDllfun dfInitClock;
static iDllfun dfGetClock;
static viDllfun dfSetClockRate;
static iDllfun dfGetCallCount;
static iDllfun dfGetBPAddr;
static viv8Dllfun dfSetBPAddr;
static viiDllfun dfSetBPCount;
static viiDllfun dfSetCPAddr;
static vv8Dllfun dfSetBPEnable;
static v8Dllfun dfGetBPPause;
static vv8Dllfun dfSetBPPause;
static pDllfun dfGetRBP;
static ipDllfun dfGetBackTrace;
static vDllfun dfMoveStep;
static vDllfun dfCallStep;
static viDllfun dfJumpTo;
static vDllfun dfJumpOut;

EXPORT void Encode(char* ifile, char* ofile)
{
	int i = 0;
	FILE* fi = fopen(ifile, "rb");
	FILE* fo = fopen(ofile, "wb");
	
	if (!fi || !fo) return;
	fseek(fi, 0, SEEK_END);
	int szi = ftell(fi);
	fseek(fi, 0, SEEK_SET);
	fread(edata, sizeof(char), szi, fi);
	for (i = 0 ; i < szi ; i++)
		edata[i] = ~edata[i];
	fwrite(edata, sizeof(char), szi, fo);
	
	fclose(fi);
	fclose(fo);
}

EXPORT int IsDllAlive()
{
	return hdll != NULL ? 1 : 0;
}

EXPORT void FreeDll()
{
	FreeLibrary(hdll);
	hdll = NULL;
	sprintf(cmd, "erase %s", dllPath);
	system(cmd);
}

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
		FreeDll();
		return 2;
	}
	dfGetBit = (isii8Dllfun)GetProcAddress(hdll, "_GetBit@12");
	if (dfGetBit == NULL)
	{
		FreeDll();
		return 3;
	}
	dfGetWord = (isii16Dllfun)GetProcAddress(hdll, "_GetWord@12");
	if (dfGetWord == NULL)
	{
		FreeDll();
		return 4;
	}
	dfGetDWord = (isii32Dllfun)GetProcAddress(hdll, "_GetDoubleWord@12");
	if (dfGetDWord == NULL)
	{
		FreeDll();
		return 5;
	}
	dfGetFloat = (isif32Dllfun)GetProcAddress(hdll, "_GetFloat@12");
	if (dfGetFloat == NULL)
	{
		FreeDll();
		return 6;
	}
	dfGetFeq = (isi32Dllfun)GetProcAddress(hdll, "_GetFeq@8");
	if (dfGetFeq == NULL)
	{
		FreeDll();
		return 7;
	}
	dfSetBit = (isii8Dllfun)GetProcAddress(hdll, "_SetBit@12");
	if (dfSetBit == NULL)
	{
		FreeDll();
		return 8;
	}
	dfSetWord = (isii16Dllfun)GetProcAddress(hdll, "_SetWord@12");
	if (dfSetWord == NULL)
	{
		FreeDll();
		return 9;
	}
	dfSetDWord = (isii32Dllfun)GetProcAddress(hdll, "_SetDoubleWord@12");
	if (dfSetDWord == NULL)
	{
		FreeDll();
		return 10;
	}
	dfSetFloat = (isif32Dllfun)GetProcAddress(hdll, "_SetFloat@12");
	if (dfGetFloat == NULL)
	{
		FreeDll();
		return 11;
	}
	dfSetFeq = (isv32Dllfun)GetProcAddress(hdll, "_SetFeq@8");
	if (dfSetFeq == NULL)
	{
		FreeDll();
		return 12;
	}
	dfSetEnable = (isiv8Dllfun)GetProcAddress(hdll, "_SetEnable@12");
	if (dfSetEnable == NULL)
	{
		FreeDll();
		return 13;
	}
	dfBeforeRunLadder = (vDllfun)GetProcAddress(hdll, "_BeforeRunLadder@0");
	if (dfBeforeRunLadder == NULL)
	{
		FreeDll();
		return 14;
	}
	dfAfterRunLadder = (vDllfun)GetProcAddress(hdll, "_AfterRunLadder@0");
	if (dfAfterRunLadder == NULL)
	{
		FreeDll();
		return 15;
	}
	dfInitRunLadder = (vDllfun)GetProcAddress(hdll, "_InitRunLadder@0");
	if (dfInitRunLadder == NULL)
	{
		FreeDll();
		return 16;
	}
	dfInitClock = (viDllfun)GetProcAddress(hdll, "_InitClock@4");
	if (dfInitClock == NULL)
	{
		FreeDll();
		return 17;
	}
	dfGetClock = (iDllfun)GetProcAddress(hdll, "_GetClock@0");
	if (dfGetClock == NULL)
	{
		FreeDll();
		return 18;
	}
	dfSetClockRate = (viDllfun)GetProcAddress(hdll, "_SetClockRate@4");
	if (dfSetClockRate == NULL)
	{
		FreeDll();
		return 19;
	}
	dfGetCallCount = (iDllfun)GetProcAddress(hdll, "_GetCallCount@0");
	if (dfGetCallCount == NULL)
	{
		FreeDll();
		return 21;
	}
	dfGetBPAddr = (iDllfun)GetProcAddress(hdll, "_GetBPAddr@0");
	if (dfGetBPAddr == NULL)
	{
		FreeDll();
		return 22;
	}
	dfSetBPAddr = (viv8Dllfun)GetProcAddress(hdll, "_SetBPAddr@8");
	if (dfSetBPAddr == NULL)
	{
		FreeDll();
		return 23;
	}
	dfSetCPAddr = (viiDllfun)GetProcAddress(hdll, "_SetCPAddr@8");
	if (dfSetCPAddr == NULL)
	{
		FreeDll();
		return 24;
	}
	dfGetBPPause = (v8Dllfun)GetProcAddress(hdll, "_GetBPPause@0");
	if (dfGetBPPause == NULL)
	{
		FreeDll();
		return 25;
	}
	dfSetBPPause = (vv8Dllfun)GetProcAddress(hdll, "_SetBPPause@4");
	if (dfSetBPPause == NULL)
	{
		FreeDll();
		return 26;
	}
	dfMoveStep = (vDllfun)GetProcAddress(hdll, "_MoveStep@0");
	if (dfMoveStep == NULL)
	{
		FreeDll();
		return 27;
	}
	dfCallStep = (vDllfun)GetProcAddress(hdll, "_CallStep@0");
	if (dfCallStep == NULL)
	{
		FreeDll();
		return 28;
	}
	dfJumpTo = (viDllfun)GetProcAddress(hdll, "_JumpTo@4");
	if (dfJumpTo == NULL)
	{
		FreeDll();
		return 29;
	}
	dfJumpOut = (vDllfun)GetProcAddress(hdll, "_JumpOut@0");
	if (dfJumpOut == NULL)
	{
		FreeDll();
		return 30;
	}
	dfSetBPEnable = (vv8Dllfun)GetProcAddress(hdll, "_SetBPEnable@4");
	if (dfSetBPEnable == NULL)
	{
		FreeDll();
		return 31;
	}
	dfSetBPCount = (viiDllfun)GetProcAddress(hdll, "_SetBPCount@8");
	if (dfSetBPCount == NULL)
	{
		FreeDll();
		return 32;
	}
	dfGetRBP = (pDllfun)GetProcAddress(hdll, "_GetRBP@0");
	if (dfGetRBP == NULL)
	{
		FreeDll();
		return 33;
	}
	dfGetBackTrace = (ipDllfun)GetProcAddress(hdll, "_GetBackTrace@4");
	if (dfGetBackTrace == NULL)
	{
		FreeDll();
		return 34;
	}
	return 0;
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

EXPORT int GetBit(char* name, int size, int8_t* output)
{
	return dfGetBit(name, size, output);
}

EXPORT int GetWord(char* name, int size, int16_t* output)
{
	return dfGetWord(name, size, output);
}

EXPORT int GetDoubleWord(char* name, int size, int32_t* output)
{
	return dfGetDWord(name, size, output);
}

EXPORT int GetFloat(char* name, int size, float* output)
{
	return dfGetFloat(name, size, output);
}

EXPORT int GetFeq(char* name, int32_t* output)
{
	return dfGetFeq(name, output);
}

EXPORT int SetFeq(char* name, int32_t input)
{
	return dfSetFeq(name, input);
}

EXPORT int SetBit(char* name, int size, int8_t* input)
{
	return dfSetBit(name, size, input);
}

EXPORT int SetWord(char* name, int size, int16_t* input)
{
	return dfSetWord(name, size, input);
}

EXPORT int SetDoubleWord(char* name, int size, int32_t* input)
{
	return dfSetDWord(name, size, input);
}

EXPORT int SetFloat(char* name, int size, float* input)
{
	return dfSetFloat(name, size, input);
}

EXPORT void SetEnable(char* name, int size, int8_t value)
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

EXPORT int GetCallCount()
{
	return dfGetCallCount();
}

EXPORT int GetBPAddr()
{
	return dfGetBPAddr();
}

EXPORT void SetBPAddr(int bpaddr, int8_t islock)
{
	dfSetBPAddr(bpaddr, islock);
}

EXPORT void SetBPCount(int bpaddr, int maxcount)
{
	dfSetBPCount(bpaddr, maxcount);
}

EXPORT void SetCPAddr(int cpaddr, int cpmsg)
{
	dfSetCPAddr(cpaddr, cpmsg);
}

EXPORT void SetBPEnable(int8_t bpenable)
{
	dfSetBPEnable(bpenable);
}

EXPORT int8_t GetBPPause()
{
	return dfGetBPPause();
}

EXPORT void SetBPPause(int8_t bppause)
{
	dfSetBPPause(bppause);
}

EXPORT void* GetRBP()
{
	return dfGetRBP();
}

EXPORT int GetBackTrace(int* data)
{
	return dfGetBackTrace(data);
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
