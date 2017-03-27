
uint16_t _WORD_to_BCD(uint16_t itg);

uint16_t _BCD_to_WORD(uint16_t BCD);

void _bitset(uint32_t* addr, uint16_t size);

void _bitrst(uint32_t* addr, uint16_t size);

void _bitcpy(uint32_t* source_addr, uint32_t* target_addr, uint16_t size);

void _ton(uint16_t* tval, uint32_t* tbit, uint16_t cod, uint16_t sv, uint32_t* otim);

void _tonr(uint16_t* tval, uint32_t* tbit, uint16_t cod, uint16_t sv, uint32_t* otim);

uint32_t _fact(uint16_t itg);

uint16_t _cmpw(uint16_t ia, uint16_t ib);

uint16_t _cmpd(uint32_t ia, uint32_t ib);

uint16_t _cmpf(float ia, float ib);

uint16_t _zcpw(uint16_t ia, uint16_t il, uint16_t ir);

uint16_t _zcpd(uint32_t ia, uint32_t il, uint32_t ir);

uint16_t _zcpf(float ia, float il, float ir);

void _xchw(uint16_t* pa, uint16_t* pb);

void _xchd(uint16_t* pa, uint16_t* pb);

void _xchf(float* pa, float* pb);

void _interrupt_timer_create(uint16_t code, uint16_t t1, uint16_t t2);

void _interrupt_timer_cancel(uint16_t code);

uint16_t _interrupt_timer_arrive(uint16_t code);

void _memsetw(uint16_t* dest, uint16_t* source, uint16_t size);

void _memsetd(uint32_t* dest, uint32_t* source, uint32_t size);

void _smov(uint16_t source, uint16_t sb1, uint16_t sb2, uint16_t* target, uint16_t tb1);
