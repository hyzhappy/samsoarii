#include <stdint.h>

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

int _Assert(char* name, int size, void** addr, int* offset);
// Get the BIT value from targeted bit register (X/Y/M/C/T/S)
EXPORT int GetBit(char* name, int size, uint32_t* output);
// Get the WORD value from targeted register (D/CV/TV)
EXPORT int GetWord(char* name, int size, uint32_t* output);
// Get the DWORD (32 bit unsigned int) value from targeted register (D/CV32)
EXPORT int GetDoubleWord(char* name, int size, uint64_t* output);
// Get the FLOAT value from targeted register (D)
EXPORT int GetFloat(char* name, int size, double* output);
// Get the signal frequency
EXPORT int GetFeq(char* name, uint64_t* output);
// Set the signal frequency
EXPORT int SetFeq(char* name, uint64_t input);
// Set the Bit value to targeted bit register (X/Y/M/C/T/S)
EXPORT int SetBit(char* name, int size, uint32_t* input);
// Set the WORD value to targeted register (D/CV/TV)
EXPORT int SetWord(char* name, int size, uint32_t* input);
// Set the DWORD value to targeted register (D)
EXPORT int SetDoubleWord(char* name, int size, uint64_t* input);
// Set the FLOAT value to targeted register (D)
EXPORT int SetFloat(char* name, int size, double* input);
// Set the writeable enable value of targeted register
EXPORT void SetEnable(char* name, int size, int value);

EXPORT void SetBaseBit(int _basebit);

EXPORT void InitClock(int _counttimems);

void UpdateClock();

EXPORT int GetClock();

EXPORT void SetClockRate(int _timerate);

void callinto();

void callleave();

void bpcycle(int32_t _bpaddr);

void cpcycle(int32_t _bpaddr, int32_t value);

EXPORT int GetCallCount();

EXPORT int GetBPAddr();

EXPORT void SetBPAddr(int32_t _bpaddr, int32_t islock);

EXPORT void SetBPCount(int32_t _bpaddr, int32_t maxcount);

EXPORT int GetBPPause();

EXPORT void SetBPPause(int _bppause);

EXPORT void* GetRBP();

EXPORT int GetBackTrace(int* data);

EXPORT void MoveStep();

EXPORT void CallStep();

EXPORT void JumpTo();

EXPORT void JumpOut();

EXPORT void SetCPAddr(int32_t _cpaddr, int32_t _cpmsg);

void InitRegisters();

void InitUserRegisters();

EXPORT void InitRunLadder();

EXPORT void BeforeRunLadder();

EXPORT void AfterRunLadder();

EXPORT void RunLadder();

// get a result (32-bit integer) by add a couple of 32-bit integers
int32_t _addw(int32_t ia, int32_t ib);
// get a result (64-bit integer) by add a couple of 64-bit integers
int64_t _addd(int64_t id, int64_t ib);
// get a result (64-bit float) by add a couple of 64-bit floats
double _addf(double id, double ib);
// get a result (32-bit integer) by sub a couple of 32-bit integers
int32_t _subw(int32_t ia, int32_t ib);
// get a result (64-bit integer) by sub a couple of 64-bit integers
int64_t _subd(int64_t ia, int64_t ib);
// get a result (64-bit float) by sub a couple of 64-bit floats
double _subf(double id, double ib);
// get a result (64-bit integer) by mul a couple of 32-bit integers
int64_t _mulwd(int32_t ia, int32_t ib);
// get a result (32-bit integer) by mul a couple of 32-bit integers
int32_t _mulww(int32_t ia, int32_t ib);
// get a result (64-bit integer) by mul a couple of 64-bit integers
int64_t _muldd(int64_t ia, int64_t ib);
// get a result (64-bit float) by mul a couple of 64-bit floats
double _mulff(double ia, double ib);
// get a result data structure (store in 64-bit integer) by div a couple of 32-bit integers
int64_t _divwd(int32_t ia, int32_t ib);
// get a result (32-bit integer) by div a couple of 32-bit integers
int32_t _divww(int32_t ia, int32_t ib);
// get a result (64-bit integer) by div a couple of 64-bit integers
int64_t _divdd(int64_t ia, int64_t ib);
// increase a 32-bit integer by 1
int32_t _incw(int32_t ia);
// increase a 64-bit integer by 1
int64_t _incd(int64_t ia);
// decrease a 32-bit integer by 1
int32_t _decw(int32_t ia);
// decrease a 64-bit integer by 1
int64_t _decd(int64_t ia);
// sin a 64-bit float (degree)
double _sin(double ia);
// cos a 64-bit float
double _cos(double ia);
// tan a 64-bit float
double _tan(double ia);
// ln a 64-bit float
double _ln(double ia);
// exp a 64-bit float
double _exp(double ia);
// Convert 64-Bit integer to 32-Bit integer 
int32_t _DWORD_to_WORD(int64_t ia);
// Convert 32-Bit integer to 64-Bit integer 
int64_t _WORD_to_DWORD(int32_t ia);
// Convert 32-Bit integer to BCD code
int32_t _WORD_to_BCD(int32_t itg);
// Convert BCD code to 32-Bit integer
int32_t _BCD_to_WORD(int32_t BCD);
// Convert 64-Bit integer to 64-Bit float
double _DWORD_to_FLOAT(int64_t ia);
// Convert 64-Bit float to 64-Bit integer by ROUND strategy
int64_t _FLOAT_to_ROUND(double ia);
// Convert 64-Bit float to 64-Bit integer by TRUNC strategy
int64_t _FLOAT_to_TRUNC(double ia);
// shift 32-Bit integer to right
uint32_t _shrw(uint32_t ia, uint32_t ib);
// shift 64-Bit integer to right
uint64_t _shrd(uint64_t ia, uint64_t ib);
// shift 32-Bit integer to left
uint32_t _shlw(uint32_t ia, uint32_t ib);
// shift 64-Bit integer to left
uint64_t _shld(uint64_t ia, uint64_t ib);
// rotate shift 32-bit integer to right
uint32_t _rorw(uint32_t ia, uint32_t ib);
// rotate shift 64-bit integer to right
uint64_t _rord(uint64_t ia, uint64_t ib);
// rotate shift 32-bit integer to left
uint32_t _rolw(uint32_t ia, uint32_t ib);
// rotate shift 64-bit integer to left
uint64_t _rold(uint64_t ia, uint64_t ib);
// shift a series of BITs to left, and fill the blank block from input BITs (keep order)
void _bitshl(int32_t* input, int32_t* addr, int32_t* enable, int32_t size, int32_t move);
// shift a series of BITs to right, and fill the blank block from input BITs (keep order)
void _bitshr(int32_t* input, int32_t* addr, int32_t* enable, int32_t size, int32_t move);
// Set a series of BIT to 1
void _bitset(int32_t* addr, int32_t* enable, int32_t size);
// Reset a series of BIT to 0
void _bitrst(int32_t* addr, int32_t* enable, int32_t size);
// Duplicate a series of BIT to the targeted memory space
void _bitcpy(int32_t* source_addr, int32_t* target_addr, int32_t* enable, int32_t size);
// Time counter
void _ton(int32_t en, int32_t id, int32_t sv, int32_t* otim);
// Time counter (opened preservation)
void _tonr(int32_t en, int32_t id, int32_t sv, int32_t* otim);
// sub function type
typedef void(*vfun)(void);
// attach an interrupt function
void _atch(int id, vfun func);
// detach an interrupt function
void _dtch(int id);
// enable interrupt
void _ei();
// disable interrupt
void _di();
// try to invoke interrupt
void _itr_invoke();
// Get the system real time
void _trd(int32_t* d);
// Set the system real time
void _twr(int32_t* d);
// modbus table
struct ModbusTable
{
	uint32_t port;
	uint32_t code;
	uint32_t slave_reg;
	uint32_t slave_len;
	uint32_t maste_reg;
};
// commuticate in COMPort and process the commands in Modbus table
void _mbus(uint32_t com_id, struct ModbusTable* table, int tlen, uint32_t* wr);
// commuticate in COMPort and send a series of memory to another
void _send(uint32_t com_id, int32_t* addr, int32_t len);
// commuticate in COMPort and reseive a series of memory from another
void _rev(uint32_t com_id, int32_t* addr, int32_t len);
// Create a pulse signal by the giving fequency parameter(in 32-bit integer)
void _plsf(uint32_t feq, int32_t* out);
// Create a pulse signal by the giving fequency parameter(in 64-bit integer)
void _dplsf(uint64_t feq, int32_t* out);
// Extra message : Duty cycle (32-bit integer)
void _pwm(uint32_t feq, uint32_t dc, int32_t* out);
// Extra message : Duty cycle (64-bit integer)
void _dpwm(uint64_t feq, uint64_t dc, int32_t* out);
// Extra message : Pulse number (32-bit integer)
void _plsy(uint32_t feq, uint32_t* pn, int32_t* out);
// Extra message : Pulse number (64-bit integer)
void _dplsy(uint64_t feq, uint64_t* pn, int32_t* out);
// Create a segment-divided and linear-faded pulse by 32-bit parameter
void _plsr(uint32_t* msg, int32_t ct, int32_t* out);
// Create a segment-divided and linear-faded pulse by 64-bit parameter
void _dplsr(uint64_t* msg, int64_t ct, int32_t* out);
// Create a segment-divided, linear-faded and oriented pulse by 32-bit parameter
void _plsrd(uint32_t* msg, int32_t ct, int32_t* out, int32_t* dir);
// Create a segment-divided, linear-faded and oriented pulse by 64-bit parameter
void _dplsrd(uint64_t* msg, int64_t ct, int32_t* out, int32_t* dir);
// Move to next segment
void _plsnext(int32_t* out);
// Stop generating signal
void _plsstop(int32_t* out);
// Pulse signal mountain (32-bit)
void _zrn(uint32_t bv, uint32_t cv, int32_t sg, int32_t* out);
// Pulse signal mountain (64-bit)
void _dzrn(uint64_t bv, uint64_t cv, int32_t sg, int32_t* out);
// Create a segment-divided pulse signal, with the linear fequency in the segment (64-bit)
void _pto(uint64_t* msg, int32_t* out, int32_t* dir);
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (32-bit)
void _drvi(uint32_t feq, uint32_t pn, int32_t* out);
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (64-bit)
void _ddrvi(uint64_t feq, uint64_t pn, int32_t* out);
// High counter
void _hcnt(int64_t* cv, int64_t sv);
// Calculate the log10
double _log(double ia);
// Calculate the power
double _pow(double ia, double ib);
// Calculate the factorial of 32-bit integer
int64_t _fact(int32_t ia);
// Compare a couple of 32-bit integers(named as ia and ib)
void _cmpw(int32_t ia, int32_t ib, int32_t* output);
// compare a couple of 64-bit integers(named as ia and ib)
void _cmpd(int64_t ia, int64_t ib, int32_t* output);
// compare a couple of 64-bit float(named as ia and ib)
void _cmpf(double ia, double ib, int32_t* output);
// check it if the targeted 32-bit integer is inside the range [il, ir] (16-bit)
void _zcpw(int32_t ia, int32_t il, int32_t ir, int32_t* output);
// check it if the targeted 64-bit integer is inside the range [il, ir] (32-bit)
void _zcpd(int64_t ia, int64_t il, int64_t ir, int32_t* output);
// check it if the targeted 64-bit float is inside the range [il, ir] (32-bit float)
void _zcpf(double ia, double il, double ir, int32_t* output);
// make 32-bit integer ia negative 
int32_t _negw(int32_t ia);
// make 64-bit integer ia negative
int64_t _negd(int64_t ia);
// swap a couple of 16-bit integers
void _xchw(int32_t* pa, int32_t* pb);
// swap a couple of 32-bit integer
void _xchd(int64_t* pa, int64_t* pb);
// swap a couple of 32-bit float
void _xchf(double* pa, double* pb);
// binary reserve 32-bit integer ia 
int32_t _cmlw(int32_t ia);
// binary reserve 64-bit integer ia
int64_t _cmld(int64_t ia);
// copy a series of 32-bit memory
void _smov(int32_t source, int32_t sb1, int32_t sb2, int32_t* target, int32_t tb1);
// copy a series of 64-bit memory
void _mvwblk(int32_t* source, int32_t* dest, int32_t* enable, int32_t len);
// move the fragment of a BCD code to anothor at the targeted position.
void _mvdblk(int64_t* source, int64_t* dest, int32_t* enable, int32_t len);
// set a series of 32-bit memeory to the targeted value
void _fmovw(int32_t source, int32_t* dest, int32_t* enable, int32_t size);
// set a series of 32-bit memeory to the targeted value
void _fmovd(int64_t source, int64_t* dest, int32_t* enable, int32_t size);
/*
double asinf(double d);

double acosf(double d);

double atanf(double d);

double log10(double d);

double logf(double d);
*/