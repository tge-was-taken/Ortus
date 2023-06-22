#include "pch.h"
#include "log.h"
#include <string>

namespace ortus::log
{
    void log(const char* cat, const char* fmt, va_list args)
    {
        static char buf[1024];
        sprintf_s(buf, sizeof(buf), "[%s] %s\n", cat, fmt);
        vprintf(buf, args);
    }

    void log(const char* cat, const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log(cat, fmt, args);
        va_end(args);
    }

    void info(const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log("INFO ", fmt, args);
        va_end(args);
    }

    void warn(const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log("WARN ", fmt, args);
        va_end(args);
    }

    void error(const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log("ERROR", fmt, args);
        va_end(args);
    }

    void trace(const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log("TRACE", fmt, args);
        va_end(args);
    }

    void debug(const char* fmt, ...)
    {
        va_list args;
        va_start(args, fmt);
        log("DEBUG", fmt, args);
        va_end(args);
    }

    static void initConsole()
    {
        AllocConsole();
        AttachConsole(GetCurrentProcessId());
        SetConsoleOutputCP(932);
        freopen("CON", "w", stdout);
        info("hello world");
    }

    static void sjis2utf8(const char* sjis, wchar_t* utf8, size_t utf8len)
    {
        ::MultiByteToWideChar(932, 0, sjis, strlen(sjis), utf8, utf8len);
    }

    static void __cdecl debugPrint(const char* fmt, ...)
    {
        static char buf[1024];
        sprintf_s(buf, sizeof(buf), "[DEBUG] %s\n", fmt);
        va_list args;
        va_start(args, fmt);
        vprintf(buf, args);
        va_end(args);
    }

    HOOK(void*, __fastcall, CreateEntity, ASLR(0xA81B80), void* _this, void* _, EntityCreateParams* params)
    {
        printf("[ENTITY] create entity %s (%08X)\n", params->name, params->id);
        return orig_CreateEntity(_this, _, params);
    }

	void init()
	{
        initConsole();

        if (config::log::restoreDebugPrints)
        {
            // Enable debug prints
            WRITE_JUMP(ASLR(0xDD5650), debugPrint);
        }

        if (config::log::logEntities)
        {
            // Print created entities
            INSTALL_HOOK(CreateEntity);
        }
	}
}