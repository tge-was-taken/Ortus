#pragma once

#include "pch.h"
#include <d3d9.h>

#include "lib/game/lib.h"
#include "lib/game/sys.h"
#include "lib/game/phase.h"
#include "lib/game/animation.h"
#include "lib/game/behavior.h"
#include "lib/game/entity.h"
#include "lib/game/file.h"
#include "lib/game/hw.h"
#include "lib/game/ui.h"

namespace ortus::game
{
	constexpr int FACTORY_ENTRY_COUNT = 791;
	constexpr int PHASE_TABLE_COUNT = 144;

	inline int* g_stageState = (int*)ASLR(0x1BE9930);
	inline PhaseManager* g_PhaseManager = (PhaseManager*)ASLR(0x18B9140);
	inline IDirect3D9** g_pD3D = (IDirect3D9**)ASLR(0x1F206D8);
	inline IDirect3DDevice9** g_pD3DDevice = (IDirect3DDevice9**)ASLR(0x1F206D4);
	inline HWND* g_hWnd = (HWND*)ASLR(0x1DD504C);
	inline int* g_playerType = (int*)ASLR(0x1BEA030);
	inline int* g_dlcPlayerModelId = (int*)ASLR(0x1BEA040);
	inline int* g_playerWigId = (int*)ASLR(0x01BE9FB0);
	inline int* g_playerWigId2 = (int*)ASLR(0x1BEA014);
	inline int* g_playerModelId = (int*)ASLR(0x1BEA028);
	inline int* g_playerModelId2 = (int*)ASLR(0x01BE9FB4);
	inline FileManager* g_FileManager = (FileManager*)ASLR(0x01DDA840);
	inline EntitySystem* g_EntitySystem = (EntitySystem*)ASLR(0x01BE9A98);
	inline cScene* g_Scene = (cScene*)ASLR(0x01BE8E40);
	inline Hw::cFactory::entry* g_FactoryEntries = (Hw::cFactory::entry*)ASLR(0x018A1D70);
	inline PhaseTableEntry* g_PhaseTable = (PhaseTableEntry*)ASLR(0x018B8CA0);

	inline FUNCTION_PTR(Entity*, __fastcall, EntitySystem__createEntity, ASLR(0x00A82090), EntitySystem* _this, void* _, const char* name, int id, EntityTransform* transform);
	inline FUNCTION_PTR(int, __fastcall, PhaseManager__requestPhaseChange, ASLR(0x00D5E850), PhaseManager* pThis, void* _, int phaseId, const char* phaseName);
	inline FUNCTION_PTR(int, __fastcall, PhaseManager__requestSubPhaseChange, ASLR(0x00D5EA40), PhaseManager* pThis, void* _, const char* phaseName, int flag, int a4);
	inline FUNCTION_PTR(int, __fastcall, cScene__startPhase, ASLR(0x00A4AC40), cScene* pThis, void* _, int phaseId, const char* phaseName, int roomId);
}