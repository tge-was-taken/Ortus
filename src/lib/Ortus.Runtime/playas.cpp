#include "pch.h"
#include "playas.h"

#include "fs.h"

namespace ortus::playas
{
	static bool enabled = false;


	static bool isDlc2Player(int playerType)
	{
		return playerType == 8;
	}

	static bool isDlc3Player(int playerType)
	{
		return playerType == 9;
	}

	static bool isDlcPlayer(int playerType)
	{
		return isDlc2Player(playerType) || isDlc3Player(playerType);
	}

	static bool isMenuPhaseId(int phaseId)
	{
		return (phaseId & 0xF00) == 0xF00;
	}

	fs::HandlerStatus overrideLoadFile(fs::ArchiveFileHandlerCtx& ctx)
	{
		if (!enabled || _stricmp(ctx.pName, "combolist.bxm") != 0)
			return fs::HandlerStatus::Continue;

		if (isDlc2Player(*game::g_playerType))
		{
			ctx.pName = "combolist_dlc2.bxm";
		}
		else if (isDlc3Player(*game::g_playerType))
		{
			ctx.pName = "combolist_dlc3.bxm";
		}

		ctx.callback(ctx);
		return fs::HandlerStatus::Stop;
	}

	HOOK(int, __cdecl, getPlayerTypeForPhase, ASLR(0xC13DE0), int phaseId)
	{
		if (isMenuPhaseId(phaseId))
		{
			// Custom player types cause null references on menu screens
			return orig_getPlayerTypeForPhase(phaseId);
		}

		return config::playas::playerType;
	}

	FUNCTION_PTR(void*, __cdecl, newWork_Pl0000, ASLR(0x00AC3340), void* a1);

	HOOK(void*, __cdecl, newWork_Pl1400, ASLR(0x00AC3D00), void* a1)
	{
		auto res = orig_newWork_Pl1400(a1);
		void** vt = *(void***)res;
		void** ovt = (void**)ASLR(0x0169C8EC);
#define RESTORE(idx) WRITE_MEMORY(&vt[idx], void*, ovt[idx])
		// fixes blade mode
		//RESTORE(1);
	
		// init function, loads health
		//RESTORE(16);
		//RESTORE(17);
		//RESTORE(18);

		//RESTORE(19);
		//RESTORE(20);
		//RESTORE(21);

		// handles input
		//RESTORE(37);
		//RESTORE(65); // qte related
		//RESTORE(76); // getAttackInfo
		//RESTORE(77);
		//RESTORE(83);
		RESTORE(84); // handles qtes

		// kinda works
		//RESTORE(105);
		//RESTORE(133);
		//RESTORE(134);
		//RESTORE(146);
		//RESTORE(190);
		//RESTORE(203);
		//RESTORE(204);
		//RESTORE(209);
		//RESTORE(211);
		//RESTORE(213);
		//RESTORE(214);
		//RESTORE(215);
		//RESTORE(216);
		//RESTORE(217);
		//RESTORE(218);
		//RESTORE(219);
		//RESTORE(221);
		//RESTORE(222);
		//RESTORE(223);
		//RESTORE(224);

		// standing state movement
		//RESTORE(226);
		//RESTORE(227);
		//RESTORE(228);
		//RESTORE(229);
		//RESTORE(231);
		//RESTORE(232);

		//RESTORE(233);
		//RESTORE(234);
		//RESTORE(235);
		//RESTORE(236);
		//RESTORE(237);
		//RESTORE(238);
		//RESTORE(239);
		//RESTORE(240);
		// moveset
		//RESTORE(241);
		//RESTORE(242);

		// state stuff i think
		// keeps freezing if i remove these
		//RESTORE(243);
		//RESTORE(244);
		//RESTORE(246);
		//RESTORE(247);
		//RESTORE(248);
		//RESTORE(249);
		//RESTORE(250);
		//RESTORE(251);
		//RESTORE(252);
		//RESTORE(254);
		//RESTORE(255);
		//RESTORE(256);
		//RESTORE(257);
		//RESTORE(258);
		//RESTORE(260);
		//RESTORE(261);
		//RESTORE(262);
		//RESTORE(263);
		//RESTORE(264);

		// model visibility
		//RESTORE(265);

#undef RESTORE
		return res;
	}

	void init()
	{
		enabled = true;
		INSTALL_HOOK(getPlayerTypeForPhase);
		INSTALL_HOOK(newWork_Pl1400);

		if (isDlcPlayer(config::playas::playerType))
		{
			// force isDlcPhase check to 1 in loadPlayerMoveset
			WRITE_MEMORY(ASLR(0xA9E30C), char, 0xB8, 0x01, 0x00, 0x00, 0x00);
		}

		fs::addArchiveFileHandler(overrideLoadFile);
	}
}