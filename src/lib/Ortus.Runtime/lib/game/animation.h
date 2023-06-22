#pragma once

#include "types.h"

namespace Animation {
	struct __declspec(align(4)) Unit
	{
		char fileBuffer;
		_BYTE gap1[7];
		_DWORD dword8;
		_DWORD id2;
		int id;
		_BYTE gap14[16];
		int char24;
		int field_28;
		_DWORD dword2C;
		_BYTE gap30[28];
		_DWORD dword4C;
		_BYTE gap50[28];
		_DWORD dword6C;
		_BYTE gap70[28];
		_DWORD dword8C;
		_DWORD flags90;
		_DWORD dword94;
		_DWORD dword98;
		_DWORD dword9C;
		_DWORD dwordA0;
		_DWORD dwordA4;
		_DWORD dwordA8;
		_BYTE gapAC[4];
		_DWORD dwordB0;
		_DWORD dwordB4;
		_DWORD dwordB8;
		_DWORD dwordBC;
		_DWORD dwordC0;
		_DWORD dwordC4;
		_DWORD dwordC8;
		_DWORD dwordCC;
		_DWORD dwordD0;
		_DWORD dwordD4;
		int gapD8;
		int field_DC;
		int field_E0;
		float field_E4;
		float field_E8;
		float field_EC;
		int field_F0;
		char motionUnit;
		_BYTE gapF5[407];
		_DWORD dword28C;
		_DWORD dword290;
		_DWORD dword294;
		int scriptOperatorUnit;
		_DWORD dword29C;
		_DWORD dword2A0;
		_DWORD dword2A4;
		_DWORD dword2A8;
		char char2AC;
		_BYTE gap2AD[127];
		_DWORD dword32C;
		_DWORD dword330;
		_BYTE gap334[4];
		_DWORD dword338;
		_DWORD dword33C;
		_DWORD dword340;
	};
}
