#pragma once

typedef int _DWORD;
typedef char _BYTE;
typedef short _WORD;
typedef unsigned char byte;

struct Vec3 {
	float x, y, z;
};

struct Vec4 {
	float x, y, z, w;
};

/* 268 */
struct Quat
{
	float x;
	float y;
	float z;
	float w;
};

struct cXmlBinary
{
	struct struct___vftable* __vftable;
	int field_4;
	int field_8;
	int field_C;
	int field_10;
	int field_14;
	int field_18;
	int field_1C;
};

struct __declspec(align(4)) struct___vftable
{
	_BYTE gap0[16];
	int(__thiscall* pfunc10)(cXmlBinary*, int);
	int(__thiscall* pfunc14)(cXmlBinary*, int, int);
	int(__thiscall* getElement)(cXmlBinary*, int, const char*);
	_BYTE gap1C[40];
	_DWORD getElementVec4Value;
	_BYTE gap48[12];
	void(__thiscall* getElementFloatValue)(cXmlBinary*, int, float*);
	void(__thiscall* getElementIntValue)(cXmlBinary*, int, _DWORD*);
	_BYTE gap5C[12];
	void(__thiscall* getElementUnsignedIntValue)(cXmlBinary*, int, _DWORD*);
};