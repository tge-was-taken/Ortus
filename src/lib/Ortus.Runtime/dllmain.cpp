#include "pch.h"

#include <stdio.h>
#include <varargs.h>

#include "app.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        ortus::app::init();
        break;
    }
    return TRUE;
}


