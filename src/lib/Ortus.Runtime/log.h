#pragma once

#include <varargs.h>

namespace ortus::log
{
    void log(const char* cat, const char* fmt, va_list args);
    void log(const char* cat, const char* fmt, ...);
    void info(const char* fmt, ...);
    void warn(const char* fmt, ...);
    void error(const char* fmt, ...);
    void trace(const char* fmt, ...);
    void debug(const char* fmt, ...);
	void init();
}