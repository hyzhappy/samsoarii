
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

EXPORT int Open();

EXPORT int Close();

EXPORT int Send(uint8_t* data, int len);

EXPORT int Receive(uint8_t* data, int len);

EXPORT int Transfer(uint8_t* data, int len, int endpoint);

EXPORT void Config(int bit);

EXPORT int Read(char* name, int len, uint8_t* input);

EXPORT int Read16(char* name, int len, uint16_t* input);

EXPORT int Read32(char* name, int len, uint32_t* input);

EXPORT int Read64(char* name, int len, uint64_t* input);

EXPORT int Read32f(char* name, int len, float* input);

EXPORT int Read64f(char* name, int len, double* input);

EXPORT int Write(char* name, int len, uint8_t* output);

EXPORT int Write16(char* name, int len, uint16_t* output);

EXPORT int Write32(char* name, int len, uint32_t* output);

EXPORT int Write64(char* name, int len, uint64_t* output);

EXPORT int Write32f(char* name, int len, float* output);

EXPORT int Write64f(char* name, int len, double* output);

EXPORT int Upload(char* file_bin, char* file_pro);

EXPORT int Download(char* file_bin, char* file_pro);
