#pragma once

#include <Windows.h>

#include "types.h"
#include "lib.h"
#include "file.h"

struct struct_char60
{
	_BYTE gap0[64];
	void(__thiscall* pfunc40)(void**, int, _DWORD, int, _DWORD, const char*);
};

struct __declspec(align(4)) struct_dword3C
{
	_BYTE gap0[1200];
	_DWORD id;
};

struct __declspec(align(4)) Entity
{
	_BYTE gap0[4];
	char name;
	_BYTE gap5[31];
	_DWORD id;
	_DWORD dword28;
	_DWORD handle;
	_BYTE fileBuffer[8];
	_DWORD* entitySystem_field8;
	struct_dword3C* dword3C;
	struct Animation::Unit* animation;
	_DWORD entitySystem_fieldc;
	struct Behavior* behavior;
	_DWORD entitySystem_field10;
	_DWORD dword50;
	_DWORD dword54;
	_DWORD subParamsFieldC_or_Field10;
	_DWORD dword5C;
};

struct __declspec(align(4)) EntitySystem
{
	_DWORD dword0;
	_DWORD heap;
	int field_8;
	int* field_C;
	void* field_10;
	_DWORD dword14;
	struct _RTL_CRITICAL_SECTION lock;
	int useLock;
	int field_34;
	int entityFixedList;
	int field_3C;
	int field_40;
	int field_44;
	int field_48;
	int field_4C;
	int field_50;
	int field_54;
	int field_58;
	int field_5C;
	struct struct_char60* entityHeap;
	_BYTE gap64[92];
	struct _RTL_CRITICAL_SECTION rtl_critical_sectionC0;
	int gapD8;
	int field_DC;
	lib::AllocatedArray<struct Entity*> entityArray;
	lib::AllocatedArray<struct Entity*> entityArray2;
	struct _RTL_CRITICAL_SECTION rtl_critical_section110;
};

struct __declspec(align(4)) EntityTransform
{
	_DWORD dword0;
	_DWORD dword4;
	_DWORD dword8;
	_DWORD dwordC;
	float float10;
	float float14;
	float float18;
	float float1C;
	float float20;
	float float24;
	float float28;
	float float2C;
	float float30;
	float float34;
	float float38;
	float float3C;
	float float40;
	float float44;
	float float48;
	float float4C;
	Vec3 pos;
	Quat rot;
	float float6C;
	float float70;
	_DWORD dword74;
	_DWORD dword78;
	_DWORD dword7C;

	EntityTransform() {
		this->float48 = 0.0;
		this->float44 = 0.0;
		this->float40 = 0.0;
		this->float3C = 0.0;
		this->float34 = 0.0;
		this->float30 = 0.0;
		this->float2C = 0.0;
		this->float28 = 0.0;
		this->float20 = 0.0;
		this->float1C = 0.0;
		this->float18 = 0.0;
		this->float14 = 0.0;
		this->float4C = 1.0;
		this->float38 = 1.0;
		this->float24 = 1.0;
		this->float10 = 1.0;
		this->dword0 = 0;
		this->dword4 = 0;
		this->dwordC = 0;
		this->dword8 = 0;
		this->pos.x = 0.0;
		this->pos.y = 0.0;
		this->pos.z = 0.0;
		this->rot.x = 0.0;
		this->rot.y = 0.0;
		this->rot.z = 0.0;
		this->rot.w = 1.0;
		this->float6C = 1.0;
		this->float70 = 1.0;
		this->dword7C = 0;
		this->dword78 = 0;
		this->dword74 = -1;
	}
};

struct EntityCreateParams
{
	const char* name;
	int id;
	int id2;
	EntityTransform* transform;
	int field_10;
	int field_14;
	struct struct_field_18* animData;
	int field_1C;
	struct cModel* model;
	int wtaFile;
	int wtbFile;
	int paramBxmFile;
};

struct EntityCreateParams2
{
	EntitySystem* entitySystem;
	void* entitySystem_field8;
	int* entitySystem_fieldc;
	EntityCreateParams* subParams;
	void* entitySystem_field10;
};

struct __declspec(align(4)) struct_field_18
{
	_DWORD dword0;
	int gap4;
	int field_8;
	int field_C;
	int field_10;
	int field_14;
	int field_18;
	int field_1C;
	int field_20;
	int field_24;
	int field_28;
	int field_2C;
	int field_30;
	int field_34;
	int field_38;
	int field_3C;
	int field_40;
	int field_44;
	int field_48;
	int field_4C;
	int field_50;
	int field_54;
	int field_58;
	int field_5C;
	int field_60;
	int field_64;
	int field_68;
	int field_6C;
	int field_70;
	int field_74;
	int field_78;
	int field_7C;
	int field_80;
	int field_84;
	int field_88;
	int field_8C;
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
	int field_B8;
	int field_BC;
	int field_C0;
	int field_C4;
	int field_C8;
	int field_CC;
	int field_D0;
	int field_D4;
	int field_D8;
	int field_DC;
	int field_E0;
	int field_E4;
	int field_E8;
	int field_EC;
	int field_F0;
	int field_F4;
	int field_F8;
	int field_FC;
	int field_100;
	int field_104;
	int field_108;
	int field_10C;
	int field_110;
	int field_114;
	int field_118;
	int field_11C;
	int field_120;
	int field_124;
	int field_128;
	int field_12C;
	int field_130;
	int field_134;
	int field_138;
	int field_13C;
	int field_140;
	int field_144;
	int field_148;
	int field_14C;
	int field_150;
	int field_154;
	int field_158;
	int field_15C;
	int field_160;
	int field_164;
	int field_168;
	int field_16C;
	int field_170;
	int field_174;
	int field_178;
	int field_17C;
	int field_180;
	int field_184;
	int field_188;
	int field_18C;
	int field_190;
	int field_194;
	int field_198;
	int field_19C;
	int field_1A0;
	int field_1A4;
	int field_1A8;
	int field_1AC;
	int field_1B0;
	int field_1B4;
	int field_1B8;
	int field_1BC;
	int field_1C0;
	int field_1C4;
	int field_1C8;
	int field_1CC;
	int field_1D0;
	int field_1D4;
	int field_1D8;
	int field_1DC;
	int field_1E0;
	int field_1E4;
	int field_1E8;
	int field_1EC;
	int field_1F0;
	int field_1F4;
	int field_1F8;
	int field_1FC;
	int field_200;
	int field_204;
	int field_208;
	int field_20C;
	int field_210;
	int field_214;
	int field_218;
	int field_21C;
	int field_220;
	int field_224;
	int field_228;
	int field_22C;
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
	int field_264;
	int field_268;
	int field_26C;
	int field_270;
	int field_274;
	int field_278;
	int field_27C;
	int field_280;
	int field_284;
	int field_288;
	int field_28C;
	int field_290;
	int field_294;
	int field_298;
	int field_29C;
	int field_2A0;
	int field_2A4;
	int field_2A8;
	int field_2AC;
	int field_2B0;
	int field_2B4;
	int field_2B8;
	int field_2BC;
	int field_2C0;
	int field_2C4;
	int field_2C8;
	int field_2CC;
	int field_2D0;
	int field_2D4;
	int field_2D8;
	int field_2DC;
	int field_2E0;
	int field_2E4;
	int field_2E8;
	int field_2EC;
	int field_2F0;
	int field_2F4;
	int field_2F8;
	int field_2FC;
	int field_300;
	int field_304;
	int field_308;
	int field_30C;
	int field_310;
	int field_314;
	int field_318;
	int field_31C;
	int field_320;
	int field_324;
	int field_328;
	int field_32C;
	int field_330;
	int field_334;
	int field_338;
	int field_33C;
	int field_340;
	int field_344;
	int field_348;
	int field_34C;
	int field_350;
	int field_354;
	int field_358;
	int field_35C;
	int field_360;
	int field_364;
	int field_368;
	int field_36C;
	int field_370;
	int field_374;
	int field_378;
	int field_37C;
	int field_380;
	int field_384;
	int field_388;
	int field_38C;
	int field_390;
	int field_394;
	int field_398;
	int field_39C;
	int field_3A0;
	int field_3A4;
	int field_3A8;
	int field_3AC;
	int field_3B0;
	int field_3B4;
	int field_3B8;
	int field_3BC;
	int field_3C0;
	int field_3C4;
	int field_3C8;
	int field_3CC;
	int field_3D0;
	int field_3D4;
	int field_3D8;
	int field_3DC;
	int field_3E0;
	int field_3E4;
	int field_3E8;
	int field_3EC;
	int field_3F0;
	int field_3F4;
	int field_3F8;
	int field_3FC;
	int field_400;
	int field_404;
	int field_408;
	int field_40C;
	int field_410;
	int field_414;
	int field_418;
	int field_41C;
	int field_420;
	int field_424;
	int field_428;
	int field_42C;
	int field_430;
	int field_434;
	int field_438;
	int field_43C;
	int field_440;
	int field_444;
	int field_448;
	int field_44C;
	int field_450;
	int field_454;
	int field_458;
	int field_45C;
	int field_460;
	int field_464;
	int field_468;
	int field_46C;
	int field_470;
	int field_474;
	int field_478;
	int field_47C;
	int field_480;
	int field_484;
	int field_488;
	int field_48C;
	int field_490;
	cFmerge fileBuffer;
	_BYTE gap498[28];
	_DWORD dword4B4;
	int field_4B8;
	int field_4BC;
	int field_4C0;
	int field_4C4;
	int field_4C8;
	int field_4CC;
	int field_4D0;
	int field_4D4;
	int field_4D8;
	int field_4DC;
	int field_4E0;
	int field_4E4;
	int field_4E8;
	int field_4EC;
	int field_4F0;
	int field_4F4;
	int field_4F8;
	int field_4FC;
	int field_500;
	int field_504;
	int field_508;
	int field_50C;
	int field_510;
	int field_514;
	int field_518;
	int field_51C;
	int field_520;
	int field_524;
	int field_528;
	int field_52C;
	int field_530;
	int field_534;
	int field_538;
	int field_53C;
	int field_540;
	int field_544;
	int field_548;
	int field_54C;
	int field_550;
	int field_554;
	int field_558;
	int field_55C;
	int field_560;
	int field_564;
	int field_568;
	int field_56C;
	int field_570;
	int field_574;
	int field_578;
	int field_57C;
	int field_580;
	int field_584;
	int field_588;
	int field_58C;
	int field_590;
	int field_594;
	int field_598;
	int field_59C;
	int field_5A0;
	int field_5A4;
};

struct __declspec(align(4)) cScene
{
	_DWORD roomId;
	_DWORD dword4;
	_BYTE byte8;
	_DWORD dwordC;
	_DWORD dword10;
	_DWORD dword14;
	Entity* playerEntity;
	_BYTE gap1C[100];
	_DWORD dword80;
	_DWORD dword84;
	_BYTE gap88[68];
	float floatCC;
	float floatD0;
	_BYTE gapD4[4];
	_DWORD dwordD8;
	_DWORD dwordDC;
	_DWORD dwordE0;
};