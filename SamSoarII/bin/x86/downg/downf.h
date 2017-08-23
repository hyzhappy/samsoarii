#include <stdint.h>
typedef int32_t* _BIT;
typedef int16_t* _WORD;
typedef uint16_t* U_WORD;
typedef int32_t* D_WORD;
typedef uint32_t* UD_WORD;
typedef float* _FLOAT;
void CACI(_WORD in,_FLOAT out);
void RPCI(_WORD in,_FLOAT out);
void CACF(_FLOAT in,_FLOAT out);
void RPCF(_FLOAT in,_FLOAT out);
void ITF(_FLOAT in,_FLOAT out);
void FAC(_WORD in,_WORD out);
void PCI(_WORD d,_WORD e,_WORD out);
void PCF(_FLOAT d,_WORD e,_FLOAT out);
void NL10(_FLOAT in,_FLOAT out);
void NLO(_FLOAT in,_FLOAT out);
void ASSH(_FLOAT in1,_FLOAT in2,_FLOAT out);
void FAAS(_FLOAT in1,_FLOAT in2,_FLOAT out);
void QEF(_FLOAT in,_FLOAT out);
void QEI(_WORD in,_WORD out);
void EIOF(_FLOAT in,_FLOAT out);
void EIOI(_WORD in,_WORD out);
void SUMI16(_WORD in,_WORD ct,_WORD out);
void DSF32(_FLOAT in,_WORD ct,_FLOAT out);
void MULI(_WORD in,_WORD ct,_WORD out);
void MDPF(_FLOAT in,_WORD ct,_FLOAT out);
void CRCC(_WORD in,_WORD ct);
void DDO(_WORD in,_WORD ct);
void DSL(_WORD in,_WORD ct);
void aaa();