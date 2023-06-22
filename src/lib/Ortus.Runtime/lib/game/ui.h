#pragma once

#include "types.h"

struct cCustomObjCtrlManager_vftable
{
	void* dtor;
	void* init;
	void* vf2;
	void* vf3;
	void* vf4;
	void* vf5;
	void* vf6;
};

struct __declspec(align(4)) cCustomObjCtrlManager
{
	cCustomObjCtrlManager_vftable* __vftable;
	_DWORD dword4;
	_DWORD dword8;
	_DWORD type;
	_DWORD dword10;
	_DWORD dword14;
	_DWORD dword18;
};

struct __declspec(align(4)) cTitleMenu
{
	cCustomObjCtrlManager base;
	_DWORD dword1C;
	_DWORD dword20;
	_BYTE gap24[2];
	_WORD word26;
	_DWORD dword28;
	int gap2C;
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
	int field_A0[1];
	int field_A4;
	int field_A8;
	int field_AC;
	int field_B0;
	int field_B4;
	int field_B8;
	int field_BC;
	int field_C0;
	_DWORD dwordC4;
	byte nextMenuMaybe;
	char field_C9;
	char field_CA;
	char field_CB;
	int state;
	_DWORD dwordD0;
	_BYTE gapD4[4];
	_DWORD dwordD8;
	_BYTE gapDC[8];
	_DWORD dwordE4;
	_DWORD dwordE8;
	_DWORD dwordEC;
	_DWORD dwordF0;
	_BYTE gapF4[4];
	_BYTE byteF8;
	_BYTE byteF9;
	byte field_FA;
	char field_FB;
	_BYTE byteFC;
	_BYTE gapFD[3];
	_DWORD dword100;
	_WORD word104;
	_BYTE byte106;
	_BYTE gap107;
	_BYTE byte108;
};