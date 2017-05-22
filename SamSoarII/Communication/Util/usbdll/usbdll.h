#ifndef INCLUDE_USBDLL
#define INCLUDE_USBDLL

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

EXPORT int open();

EXPORT int close();

EXPORT int transfer(uint8_t* data, int len);

EXPORT void config(int bit);

EXPORT int read(char* name, int len, uint8_t* input);

EXPORT int read16(char* name, int len, uint16_t* input);

EXPORT int read32(char* name, int len, uint32_t* input);

EXPORT int read64(char* name, int len, uint64_t* input);

EXPORT int read32f(char* name, int len, float* input);

EXPORT int read64f(char* name, int len, double* input);

EXPORT int write(char* name, int len, uint8_t* output);

EXPORT int write16(char* name, int len, uint16_t* output);

EXPORT int write32(char* name, int len, uint32_t* output);

EXPORT int write64(char* name, int len, uint64_t* output);

EXPORT int write32f(char* name, int len, float* output);

EXPORT int write64f(char* name, int len, double* output);

EXPORT int upload(char* file_bin, char* file_pro);

EXPORT int download(char* file_bin, char* file_pro);

#endif
