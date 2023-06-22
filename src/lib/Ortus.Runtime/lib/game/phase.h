#pragma once

#include "types.h"

struct struc_6
{
	int phaseId;
	int pSubPhaseElement;
	int pSubPhase;
	int pPhaseElement;
};

struct struct_dword8
{
	_BYTE gap0[32];
	_DWORD dword20;
};

struct __declspec(align(4)) struct_v4
{
	_BYTE gap0[4];
	_DWORD dword4;
	struct_dword8* dword8;
};

struct __declspec(align(4)) PhaseManager
{
	_DWORD dword0;
	int state;
	int prevPhaseId;
	int prevPhaseNameHash;
	char prevPhaseName[32];
	int prevField5C;
	int phaseId;
	int phaseNameHash;
	char phaseName[32];
	int field_5C;
	int nextPhaseId2;
	_DWORD nextPhaseNameHash2;
	char nextPhaseName2[32];
	_DWORD dword88;
	int gap8C;
	int field_90;
	int field_94;
	int field_98;
	int field_9C;
	int field_A0;
	int field_A4;
	int field_A8;
	int field_AC;
	int field_B0;
	int field_B4;
	int nextPhaseId;
	int nextPhaseNameHash;
	char nextPhaseName[32];
	int nextField5C;
	_DWORD phaseId2;
	_DWORD phaseNameHash2;
	char phaseName_2[32];
	_DWORD field5C_2;
	struc_6* phaseArray;
	int field_114;
	struct_v4* phaseTableEntry;
	const char* subPhaseName;
	int field_120;
	int field_124;
	int field_128;
	int field_12C;
	Vec4 playerPos;
	Vec4 playerRot;
	int graPos;
	int field_154;
	int field_158;
	int field_15C;
	int roomNoMaybe;
	int field_164;
	int field_168;
	int field_16C;
	int field_170;
	int field_174;
	int field_178;
	int field_17C;
	int field_180;
	int field_184;
	int isRestartPoint;
	int saveRestartPos;
	Vec4 cameraYaw;
	int cameraEnable;
	int cameraXEnable;
	int cameraYEnable;
	int isPlWaitPayment;
	int phaseData;
	int field_1B4;
	cXmlBinary phaseInfoXml;
	cXmlBinary xml;
	_DWORD maybePlayerLoadedFlag;
	int field_1FC;
	void* field_200;
	int field_204;
	int field_208;
	int field_20C;
	int field_210;
	int field_214;
	int field_218;
	int field_21C;
	float field_220;
	float field_224;
	float field_228;
	float field_22C;
	int field_230;
	int field_234;
	int field_238;
	int field_23C;
	int field_240;
	int field_244;
	int field_248;
	int field_24C;
	int field_250;
	int field_254;
	int field_258;
	int field_25C;
	int field_260;
};

struct PhaseTableEntry
{
	int id;
	void* create;
};