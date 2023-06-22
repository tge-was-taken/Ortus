#include "pch.h"
#include "debug.h"
#include "log.h"

namespace ortus::debug
{
	static void waitForDebugger()
	{
		while (!IsDebuggerPresent())
			Sleep(1000);
	}

    static double GetTimeInMs()
    {
        LARGE_INTEGER t, freq;
        uint64_t val;

        QueryPerformanceCounter(&t);
        QueryPerformanceFrequency(&freq);
        return (double)((double)t.QuadPart / (double)freq.QuadPart) * 1000;
    }

    static s32 getInstructionSize(const u8* code)
    {
        LONG extra;
        const u8* codeEnd = (const u8*)DetourCopyInstruction(NULL, NULL, (void*)code, NULL, &extra);
        return codeEnd - code;
    }

    static LONG WINAPI VectoredExceptionHandler(PEXCEPTION_POINTERS pExceptionInfo)
    {
        if (pExceptionInfo->ExceptionRecord->ExceptionCode == EXCEPTION_ACCESS_VIOLATION)
        {
            const u8* code = (const u8*)pExceptionInfo->ContextRecord->Eip;
            const s32 instrSize = getInstructionSize(code);

            //WRITE_NOP(code, instrSize);
            pExceptionInfo->ContextRecord->Eip += instrSize;
            return EXCEPTION_CONTINUE_EXECUTION;
        }
        else if (pExceptionInfo->ExceptionRecord->ExceptionCode == EXCEPTION_SINGLE_STEP)
        {
            return EXCEPTION_CONTINUE_EXECUTION;
        }

        return EXCEPTION_CONTINUE_SEARCH;
    }

	void init()
	{
        log::debug("MODULE_HANDLE = 0x%08X\n", (size_t)MODULE_HANDLE);
        log::debug("address offset = 0x%08X (0x%08X)\n", ASLR(0), ASLR(0x00F03DE0));
		AddVectoredExceptionHandler(0, VectoredExceptionHandler);
	}
}