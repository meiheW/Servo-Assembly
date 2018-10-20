// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 MYPROCESS_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// MYPROCESS_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。
#ifdef MYPROCESS_EXPORTS
#define MYPROCESS_API __declspec(dllexport)
#else
#define MYPROCESS_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

struct Result_stru 
{
	int x;
	int y;
	int status;
};


MYPROCESS_API int _stdcall process(void* pdata,unsigned int w, unsigned int h ,double* worldx,double* worldy);
MYPROCESS_API double _stdcall calibration(char* filename);
MYPROCESS_API int __stdcall test();
MYPROCESS_API void _stdcall processtest(double* worldx,double* worldy);
MYPROCESS_API void _stdcall buffertt(void* pdata, int w,  int h );


#ifdef __cplusplus
}
#endif