#include "pch.h"
#include "fastboot.h"

namespace ortus::fastboot
{
	HOOK(int, __cdecl, set30FpsMode, ASLR(0xF98AD0), int a1)
	{
		return orig_set30FpsMode(0);
	}

	HOOK(int, __fastcall, initSteam, 0x00981CB0, void* pThis)
	{
		orig_initSteam(pThis);
		return 1;
	}

	HOOK(int, __fastcall, cPf01__checkSkipTitleScreenIntro, ASLR(0x00D46C60))
	{
		return 1;
	}

	void init() 
	{
		if (config::fastboot::skipLogos)
		{
			// skip logos
			WRITE_MEMORY(ASLR(0xA52C92), char, 0x31, 0xC9, 0x90, 0x90, 0x90, 0x90);
		}

		// overwrite initial phase id
		WRITE_MEMORY(ASLR(0xA52DA3) + 6, int, config::fastboot::bootPhaseId);

		if (config::fastboot::uncapTitleScreen)
		{
			// speed up title screen by uncapping fps
			INSTALL_HOOK(set30FpsMode);
		}

		if (config::fastboot::ignoreSteamNotRunning)
		{
			// don't exit game after starting if steam isn't running
			INSTALL_HOOK(initSteam);
		}

		if (config::fastboot::skipTitleScreenAnimation)
		{
			INSTALL_HOOK(cPf01__checkSkipTitleScreenIntro);
		}

		if (config::fastboot::autoUnlock)
		{
			WRITE_NOP(ASLR(0x00D4FB76), 2);
			WRITE_MEMORY(ASLR(0x01BEA070), int, 0xFFFFFFFF);
			WRITE_MEMORY(ASLR(0x01BEA088), int, 0xFFFFFFFF);
			WRITE_MEMORY(ASLR(0x01BEA098), int, 0xFFFFFFFF);
		}
	}
}