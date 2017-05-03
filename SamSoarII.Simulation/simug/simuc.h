/*
	The header file for simulate program of user PLC Device
   */

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

// Register Memorys
uint32_t XBit[128];
uint32_t YBit[128];
uint32_t MBit[256<<5];
uint32_t CBit[256];
uint32_t TBit[256];
uint32_t SBit[32<<5];
uint32_t DWord[8192];
uint32_t CVWord[200];
uint64_t CVDoubleWord[56];
uint32_t TVWord[256];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
uint32_t XEnable[128];
uint32_t YEnable[128];
uint32_t MEnable[256<<5];
uint32_t CEnable[256];
uint32_t TEnable[256];
uint32_t SEnable[32<<5];
uint32_t DEnable[8192];
uint32_t CVEnable[256];
uint32_t TVEnable[256];
// pulse signal frequency
extern uint64_t YFeq[4];

EXPORT void BeforeRunLadder();

EXPORT void RunLadder();

EXPORT void AfterRunLadder();

//EXPORT void GetBit(char* name, int size, uint32_t* output);

//EXPORT void GetWord(char* name, int size, uint16_t* output);

//EXPORT void GetDoubleWord(char* name, int size, uint32_t* output);

//EXPORT void GetFloat(char* name, int size, float* output);

//EXPORT void GetDouble(char* name, int size, double* output);

//EXPORT void SetBit(char* name, int size, uint32_t* input);

//EXPORT void SetWord(char* name, int size, uint16_t* input);

//EXPORT void SetDoubleWord(char* name, int size, uint32_t* input);

//EXPORT void SetFloat(char* name, int size, float* input);

//EXPORT void SetDouble(char* name, int size, double* input);

//EXPORT void SetEnable(char* name, int size, int value);

// Get the BIT value from targeted bit register (X/Y/M/C/T/S) 
EXPORT void GetBit(char* name, int size, uint32_t* output)
{
	int addr = 0;
	sscanf(name + 1, "%d", &addr);
	switch (name[0])
	{
	case 'X':
		memcpy(output, XBit + addr, size * sizeof(uint32_t));
		break;
	case 'Y':
		memcpy(output, YBit + addr, size * sizeof(uint32_t));
		break;
	case 'M':
		memcpy(output, MBit + addr, size * sizeof(uint32_t));
		break;
	case 'C':
		memcpy(output, CBit + addr, size * sizeof(uint32_t));
		break;
	case 'T':
		memcpy(output, TBit + addr, size * sizeof(uint32_t));
		break;
	case 'S':
		memcpy(output, SBit + addr, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Get the WORD value from targeted register (D/CV/TV)
EXPORT void GetWord(char* name, int size, uint32_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(uint32_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, CVWord + addr, size * sizeof(uint32_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, TVWord + addr, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Get the DWORD (32 bit unsigned int) value from targeted register (D/CV32)
EXPORT void GetDoubleWord(char* name, int size, uint64_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(uint64_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, CVDoubleWord + (addr - 200), size * sizeof(uint64_t));
		break;
	default:
		break;
	}
}
// Get the FLOAT value from targeted register (D)
EXPORT void GetFloat(char* name, int size, double* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(double));
		break;
	default:
		break;
	}
}
// Get the signal frequency
EXPORT void GetFeq(char* name, uint64_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'Y':
		sscanf(name + 1, "%d", &addr);
		*output = YFeq[addr];
		break;
	default:
		break;
	}
}
// Set the signal frequency
EXPORT void SetFeq(char* name, uint64_t input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'Y':
		sscanf(name + 1, "%d", &addr);
		YFeq[addr] = input;
		break;
	default:
		break;
	}
}
// Set the Bit value to targeted bit register (X/Y/M/C/T/S)
EXPORT void SetBit(char* name, int size, uint32_t* input)
{
	int addr = 0;
	sscanf(name + 1, "%d", &addr);
	switch (name[0])
	{
	case 'X':
		memcpy(XBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'Y':
		memcpy(YBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'M':
		memcpy(MBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'C':
		memcpy(CBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'T':
		memcpy(TBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'S':
		memcpy(SBit + addr, input, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Set the WORD value to targeted register (D/CV/TV)
EXPORT void SetWord(char* name, int size, uint32_t* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(uint32_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(CVWord + addr, input, size * sizeof(uint32_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(TVWord + addr, input, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Set the DWORD value to targeted register (D)
EXPORT void SetDoubleWord(char* name, int size, uint64_t* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(uint64_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(CVDoubleWord + (addr - 200), input, size * sizeof(uint64_t));
		break;
	default:
		break;
	}
}
// Set the FLOAT value to targeted register (D)
EXPORT void SetFloat(char* name, int size, double* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(double));
		break;
	default:
		break;
	}
}

// Set the writeable enable value of targeted register
EXPORT void SetEnable(char* name, int size, int value)
{
	int addr = 0;
	switch (name[0])
	{
	case 'X':
		sscanf(name + 1, "%d", &addr);
		while (size--) XEnable[addr + size] = value;
		break;
	case 'Y':
		sscanf(name + 1, "%d", &addr);
		while (size--) YEnable[addr + size] = value;
		break;
	case 'M':
		sscanf(name + 1, "%d", &addr);
		while (size--) MEnable[addr + size] = value;
		break;
	case 'S':
		sscanf(name + 1, "%d", &addr);
		while (size--) SEnable[addr + size] = value;
		break;
	case 'D':
		sscanf(name + 1, "%d", &addr);
		while (size--) DEnable[addr + size] = value;
		break;
	case 'C':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &addr);
			while (size--) CVEnable[addr + size] = value;
			break;
		default:
			sscanf(name + 1, "%d", &addr);
			while (size--) CEnable[addr + size] = value;
			break;
		}
		break;
	case 'T':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &addr);
			while (size--) TVEnable[addr + size] = value;
			break;
		default:
			sscanf(name + 1, "%d", &addr);
			while (size--) TEnable[addr + size] = value;
			break;
		}
		break;
	}
}

static int currenttime;
static int beforetime;
static int aftertime;
static int deltatime;
int counttime;
int counttimems;
int timerate = 50;
static int scanperiod = 1000;

EXPORT void InitClock(int _counttimems)
{
	counttimems = _counttimems;
	counttime = counttimems * 1000;
}

void UpdateClock()
{
	LARGE_INTEGER lpCount;
	QueryPerformanceCounter(&lpCount);
	currenttime = (int)(lpCount.QuadPart / timerate);
}

EXPORT int GetClock()
{
	return counttimems;
}

EXPORT void SetClockRate(int _timerate)
{
	timerate = _timerate;
}

void InitRegisters()
{
	memset(XBit, 0, sizeof(XBit));
	memset(YBit, 0, sizeof(YBit));
	memset(SBit, 0, sizeof(SBit));
	memset(MBit, 0, sizeof(MBit));
	memset(TBit, 0, sizeof(TBit));
	memset(CBit, 0, sizeof(CBit));
	memset(DWord, 0, sizeof(DWord));
	memset(CVWord, 0, sizeof(CVWord));
	memset(TVWord, 0, sizeof(TVWord));
	memset(CVDoubleWord, 0, sizeof(CVDoubleWord));
	
	MBit[8151] = 0;
	MBit[8152] = 1;
}

EXPORT void BeforeRunLadder()
{
	UpdateClock();
	beforetime = currenttime;
	
	MBit[8158] = ((counttimems / 5) & 1);
	MBit[8159] = ((counttimems / 50) & 1);
	MBit[8160] = ((counttimems / 500) & 1);
	MBit[8161] = ((counttimems / 30000) & 1);
}

EXPORT void AfterRunLadder()
{
	UpdateClock();
	aftertime = currenttime;
	deltatime = aftertime - beforetime;
	counttime += deltatime;
	counttimems = counttime / 1000;
}

EXPORT void InitRunLadder()
{
	InitRegisters();
	InitClock(0);
	BeforeRunLadder();
	RunLadder();
	AfterRunLadder();
	MBit[8151] = 1;
	MBit[8152] = 0;
}

