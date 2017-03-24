#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>

#ifndef _SIMUC_H__
#define _SIMUC_H__

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif // BUILD_DLL

#endif // _SIMUC_H__

#ifdef __cplusplus
#define EXPORT extern "C" DLL_EXPORT __stdcall
#else
#define EXPORT DLL_EXPORT __stdcall
#endif

uint32_t XBit[128];
uint32_t YBit[128];
uint32_t MBit[256<<5];
uint32_t CBit[256];
uint32_t TBit[256];
uint32_t SBit[32<<5];
uint16_t DWord[8192];
uint16_t CVWord[200];
uint32_t CVDoubleWord[56];
uint16_t TVWord[256];

uint32_t XEnable[128];
uint32_t YEnable[128];
uint32_t MEnable[256<<5];
uint32_t CEnable[256];
uint32_t TEnable[256];
uint32_t SEnable[32<<5];
uint32_t DEnable[8192];
uint32_t CVEnable[256];
uint32_t TVEnable[256];

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

EXPORT void GetWord(char* name, int size, uint16_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(uint16_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, CVWord + addr, size * sizeof(uint16_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, TVWord + addr, size * sizeof(uint16_t));
		break;
	default:
		break;
	}
}

EXPORT void GetDoubleWord(char* name, int size, uint32_t* output)
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
		memcpy(output, CVDoubleWord + (addr - 200), size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}

EXPORT void GetFloat(char* name, int size, float* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(float));
		break;
	default:
		break;
	}
}

EXPORT void GetDouble(char* name, int size, double* output)
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

EXPORT void SetWord(char* name, int size, uint16_t* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(uint16_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(CVWord + addr, input, size * sizeof(uint16_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(TVWord + addr, input, size * sizeof(uint16_t));
		break;
	default:
		break;
	}
}

EXPORT void SetDoubleWord(char* name, int size, uint32_t* input)
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
		memcpy(CVDoubleWord + (addr - 200), input, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}

EXPORT void SetFloat(char* name, int size, float* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(float));
		break;
	default:
		break;
	}
}

EXPORT void SetDouble(char* name, int size, double* input)
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
