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

int _Assert(char* name, int size, int32_t** addr, int* offset);
// Get the BIT value from targeted bit register (X/Y/M/C/T/S)
EXPORT int GetBit(char* name, int size, int8_t* output);
// Get the WORD value from targeted register (D/CV/TV)
EXPORT int GetWord(char* name, int size, int16_t* output);
// Get the DWORD (32 bit unsigned int) value from targeted register (D/CV32)
EXPORT int GetDoubleWord(char* name, int size, int32_t* output);
// Get the FLOAT value from targeted register (D)
EXPORT int GetFloat(char* name, int size, float* output);
// Get the signal frequency
EXPORT int GetFeq(char* name, uint32_t* output);
// Set the signal frequency
EXPORT int SetFeq(char* name, uint32_t input);
// Set the Bit value to targeted bit register (X/Y/M/C/T/S)
EXPORT int SetBit(char* name, int size, int8_t* input);
// Set the WORD value to targeted register (D/CV/TV)
EXPORT int SetWord(char* name, int size, int16_t* input);
// Set the DWORD value to targeted register (D)
EXPORT int SetDoubleWord(char* name, int size, int32_t* input);
// Set the FLOAT value to targeted register (D)
EXPORT int SetFloat(char* name, int size, float* input);
// Set the writeable enable value of targeted register
EXPORT int SetEnable(char* name, int size, int8_t value);

EXPORT void InitClock(int _counttimems);

void UpdateClock();

EXPORT int GetClock();

EXPORT void SetClockRate(int _timerate);

void callinto();

void callleave();

void bpcycle(int32_t _bpaddr);

void cpcycle(int32_t _bpaddr, int8_t value);

EXPORT int GetCallCount();

EXPORT int GetBPAddr();

EXPORT void SetBPAddr(int32_t _bpaddr, int8_t islock);

EXPORT void SetBPCount(int32_t _bpaddr, int32_t maxcount);

EXPORT int8_t GetBPPause();

EXPORT void SetBPPause(int8_t _bppause);

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

// get a result (16-bit integer) by add a couple of 16-bit integers
int16_t _addw(int16_t ia, int16_t ib);
// get a result (32-bit integer) by add a couple of 32-bit integers
int32_t _addd(int32_t id, int32_t ib);
// get a result (32-bit float) by add a couple of 32-bit floats
float _addf(float id, float ib);
// get a result (16-bit integer) by sub a couple of 16-bit integers
int16_t _subw(int16_t ia, int16_t ib);
// get a result (32-bit integer) by sub a couple of 32-bit integers
int32_t _subd(int32_t ia, int32_t ib);
// get a result (32-bit float) by sub a couple of 32-bit floats
float _subf(float id, float ib);
// get a result (32-bit integer) by mul a couple of 16-bit integers
int32_t _mulwd(int16_t ia, int16_t ib);
// get a result (16-bit integer) by mul a couple of 16-bit integers
int16_t _mulww(int16_t ia, int16_t ib);
// get a result (32-bit integer) by mul a couple of 32-bit integers
int32_t _muldd(int32_t ia, int32_t ib);
// get a result (32-bit float) by mul a couple of 32-bit floats
float _mulff(float ia, float ib);
// get a result data structure (store in 32-bit integer) by div a couple of 16-bit integers
int32_t _divwd(int16_t ia, int16_t ib);
// get a result (16-bit integer) by div a couple of 16-bit integers
int16_t _divww(int16_t ia, int16_t ib);
// get a result (32-bit integer) by div a couple of 32-bit integers
int32_t _divdd(int32_t ia, int32_t ib);
// get a result (32-bit float) by div a couple of 32-bit float
float _divff(float ia, float ib);
// increase a 16-bit integer by 1
int16_t _incw(int16_t ia);
// increase a 32-bit integer by 1
int32_t _incd(int32_t ia);
// decrease a 16-bit integer by 1
int16_t _decw(int16_t ia);
// decrease a 32-bit integer by 1
int32_t _decd(int32_t ia);
// sin a 32-bit float (degree)
float _sin(float ia);
// cos a 32-bit float
float _cos(float ia);
// tan a 32-bit float
float _tan(float ia);
// ln a 32-bit float
float _ln(float ia);
// exp a 32-bit float
float _exp(float ia);
// Convert 64-Bit integer to 32-Bit integer 
int16_t _DWORD_to_WORD(int32_t ia);
// Convert 32-Bit integer to 64-Bit integer 
int32_t _WORD_to_DWORD(int16_t ia);
// Convert 32-Bit integer to BCD code
int16_t _WORD_to_BCD(int16_t itg);
// Convert BCD code to 32-Bit integer
int16_t _BCD_to_WORD(int16_t BCD);
// Convert 64-Bit integer to 64-Bit float
float _DWORD_to_FLOAT(int32_t ia);
// Convert 64-Bit float to 64-Bit integer by ROUND strategy
int32_t _FLOAT_to_ROUND(float ia);
// Convert 64-Bit float to 64-Bit integer by TRUNC strategy
int32_t _FLOAT_to_TRUNC(float ia);
// shift 32-Bit integer to right
uint16_t _shrw(uint16_t ia, uint16_t ib);
// shift 64-Bit integer to right
uint32_t _shrd(uint32_t ia, uint32_t ib);
// shift 32-Bit integer to left
uint16_t _shlw(uint16_t ia, uint16_t ib);
// shift 64-Bit integer to left
uint32_t _shld(uint32_t ia, uint32_t ib);
// rotate shift 16-bit integer to right
uint16_t _rorw(uint16_t ia, uint16_t ib);
// rotate shift 32-bit integer to right
uint32_t _rord(uint32_t ia, uint32_t ib);
// rotate shift 16-bit integer to left
uint16_t _rolw(uint16_t ia, uint16_t ib);
// rotate shift 32-bit integer to left
uint32_t _rold(uint32_t ia, uint32_t ib);
// shift a series of BITs to left, and fill the blank block from input BITs (keep order)
void _bitshl(int8_t* input, int8_t* addr, int8_t* enable, int16_t size, int16_t move);
// shift a series of BITs to right, and fill the blank block from input BITs (keep order)
void _bitshr(int8_t* input, int8_t* addr, int8_t* enable, int16_t size, int16_t move);
// Set a series of BIT to 1
void _bitset(int8_t* addr, int8_t* enable, int16_t size);
// Reset a series of BIT to 0
void _bitrst(int8_t* addr, int8_t* enable, int16_t size);
// Duplicate a series of BIT to the targeted memory space
void _bitcpy(int8_t* source_addr, int8_t* target_addr, int8_t* enable, int16_t size);
// Time counter
void _ton(int8_t en, int16_t id, int16_t sv, int32_t* otim);
// Time counter (opened preservation)
void _tonr(int8_t en, int16_t id, int16_t sv, int32_t* otim);
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
void _trd(int16_t* d);
// Set the system real time
void _twr(int16_t* d);
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
void _mbus(uint32_t com_id, struct ModbusTable* table, int tlen, int16_t* wr);
// commuticate in COMPort and send a series of memory to another
void _send(uint32_t com_id, int16_t* addr, int16_t len);
// commuticate in COMPort and reseive a series of memory from another
void _rev(uint32_t com_id, int16_t* addr, int16_t len);
// Create a pulse signal by the giving fequency parameter(in 32-bit integer)
void _plsf(uint16_t feq, int8_t* out);
// Create a pulse signal by the giving fequency parameter(in 64-bit integer)
void _dplsf(uint32_t feq, int8_t* out);
// Extra message : Duty cycle (32-bit integer)
void _pwm(uint16_t feq, uint16_t dc, int8_t* out);
// Extra message : Duty cycle (64-bit integer)
void _dpwm(uint32_t feq, uint32_t dc, int8_t* out);
// Extra message : Pulse number (32-bit integer)
void _plsy(uint16_t feq, uint16_t* pn, int8_t* out);
// Extra message : Pulse number (64-bit integer)
void _dplsy(uint32_t feq, uint32_t* pn, int8_t* out);
// Create a segment-divided and linear-faded pulse by 32-bit parameter
void _plsr(uint16_t* msg, int16_t ct, int8_t* out);
// Create a segment-divided and linear-faded pulse by 64-bit parameter
void _dplsr(uint32_t* msg, int32_t ct, int8_t* out);
// Create a segment-divided, linear-faded and oriented pulse by 32-bit parameter
void _plsrd(uint16_t* msg, int16_t ct, int8_t* out, int8_t* dir);
// Create a segment-divided, linear-faded and oriented pulse by 64-bit parameter
void _dplsrd(uint32_t* msg, int32_t ct, int8_t* out, int8_t* dir);
// Move to next segment
void _plsnext(int8_t* out);
// Stop generating signal
void _plsstop(int8_t* out);
// Pulse signal mountain (32-bit)
void _zrn(uint16_t bv, uint16_t cv, int8_t sg, int8_t* out);
// Pulse signal mountain (64-bit)
void _dzrn(uint32_t bv, uint32_t cv, int8_t sg, int8_t* out);
// Create a segment-divided pulse signal, with the linear fequency in the segment (64-bit)
void _pto(uint32_t* msg, int8_t* out, int8_t* dir);
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (32-bit)
void _drvi(uint16_t feq, uint16_t pn, int8_t* out);
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (64-bit)
void _ddrvi(uint32_t feq, uint32_t pn, int8_t* out);
// High counter
void _hcnt(int32_t* cv, int32_t sv);
// Calculate the log10
float _log(float ia);
// Calculate the power
float _pow(float ia, float ib);
// Calculate the factorial of 32-bit integer
int32_t _fact(int16_t ia);
// Calculate the squarter of 64-bit float
float _sqrt(float ia);
// Compare a couple of 32-bit integers(named as ia and ib)
void _cmpw(int16_t ia, int16_t ib, int8_t* output);
// compare a couple of 64-bit integers(named as ia and ib)
void _cmpd(int32_t ia, int32_t ib, int8_t* output);
// compare a couple of 64-bit float(named as ia and ib)
void _cmpf(float ia, float ib, int8_t* output);
// check it if the targeted 32-bit integer is inside the range [il, ir] (16-bit)
void _zcpw(int16_t ia, int16_t il, int16_t ir, int8_t* output);
// check it if the targeted 64-bit integer is inside the range [il, ir] (32-bit)
void _zcpd(int32_t ia, int32_t il, int32_t ir, int8_t* output);
// check it if the targeted 64-bit float is inside the range [il, ir] (32-bit float)
void _zcpf(float ia, float il, float ir, int8_t* output);
// make 32-bit integer ia negative 
int16_t _negw(int16_t ia);
// make 64-bit integer ia negative
int32_t _negd(int32_t ia);
// swap a couple of 16-bit integers
void _xchw(int16_t* pa, int16_t* pb);
// swap a couple of 32-bit integer
void _xchd(int32_t* pa, int32_t* pb);
// swap a couple of 32-bit float
void _xchf(float* pa, float* pb);
// binary reserve 32-bit integer ia 
int16_t _cmlw(int16_t ia);
// binary reserve 64-bit integer ia
int32_t _cmld(int32_t ia);
// copy a series of 32-bit memory
void _smov(int16_t source, int16_t sb1, int16_t sb2, int16_t* target, int16_t tb1);
// copy a series of 64-bit memory
void _mvwblk(int16_t* source, int16_t* dest, int8_t* enable, int16_t len);
// move the fragment of a BCD code to anothor at the targeted position.
void _mvdblk(int32_t* source, int32_t* dest, int8_t* enable, int16_t len);
// set a series of 32-bit memeory to the targeted value
void _fmovw(int16_t source, int16_t* dest, int8_t* enable, int16_t size);
// set a series of 32-bit memeory to the targeted value
void _fmovd(int32_t source, int32_t* dest, int8_t* enable, int16_t size);

void _set_wbit(int16_t* src, int16_t loc, int8_t* en, int16_t size, int8_t value);

int8_t _get_wbit(int16_t* src, int16_t loc);

void _mov_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int8_t* en, int16_t size);

void _mov_wbit_to_bit(int16_t* src, int16_t sloc, int8_t* dst, int8_t* en, int16_t size);

void _mov_bit_to_wbit(int8_t* src, int16_t* dst, int16_t dloc, int8_t* en, int16_t size);

void _shl_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int8_t* en, int16_t size, int16_t move);

void _shl_wbit_to_bit(int16_t* src, int16_t sloc, int8_t* dst, int8_t* en, int16_t size, int16_t move);

void _shl_bit_to_wbit(int8_t* src, int16_t* dst, int16_t dloc, int8_t* en, int16_t size, int16_t move);

void _shr_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int8_t* en, int16_t size, int16_t move);

void _shr_wbit_to_bit(int16_t* src, int16_t sloc, int8_t* dst, int8_t* en, int16_t size, int16_t move);

void _shr_bit_to_wbit(int8_t* src, int16_t* dst, int16_t dloc, int8_t* en, int16_t size, int16_t move);

void _set_bword(int8_t* src, int16_t size, int8_t* en, int16_t value);

int16_t _get_bword(int8_t* src, int16_t size);

void _set_bdword(int8_t* src, int16_t size, int8_t* en, int32_t value);

int32_t _get_bdword(int8_t* src, int16_t size);

void _xch_bword_to_word(int8_t* bit, int16_t size, int8_t* en, int16_t* word);

void _xch_bword_to_bword(int8_t* bit1, int16_t size1, int8_t* en1, int8_t* bit2, int16_t size2, int8_t* en2);

void _xchd_bdword_to_dword(int8_t* bit, int16_t size, int8_t* en, int32_t* dword);

void _xchd_bdword_to_bdword(int8_t* bit1, int16_t size1, int8_t* en1, int8_t* bit2, int16_t size2, int8_t* en2);

void _cmpw_wbit(int16_t ia, int16_t ib, int16_t* ic, int16_t loc);

void _cmpd_wbit(int32_t ia, int32_t ib, int16_t* ic, int16_t loc);

void _cmpf_wbit(float ia, float ib, int16_t* ic, int16_t loc);

void _zcpw_wbit(int16_t ia, int16_t il, int16_t ir, int16_t* out, int16_t loc);

void _zcpd_wbit(int32_t ia, int32_t il, int32_t ir, int16_t* out, int16_t loc);

void _zcpf_wbit(float ia, float il, float ir, int16_t* out, int16_t loc);
