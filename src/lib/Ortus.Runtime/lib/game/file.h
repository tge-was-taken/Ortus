#pragma once

#include "types.h" 

struct FileHandle
{
	_DWORD dword0;
	_DWORD dword4;
	byte filePath[32];
	_DWORD id;
	int gap2C;
	void* pBuffer;
	int field_34;
	void* heap;
	_DWORD dword3C;
	int gap40;
	int field_44;
	int field_48;
	int status;
	int field_50;
	_DWORD dword54;
	_DWORD dword58;
};

struct FileManager
{
	int gap0;
	int field_4;
	int heap;
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
	FileHandle** fileHandles;
	int maxHandleCount;
	int nextHandleIndex;
	int field_78;
	int nextHandleId;
};

struct cFmergeBinMetaHeader
{
	int shift;
	int metaIndexTableOff;
	int hashTableOff;
	int idTableOff;
};

struct __declspec(align(4)) cFmergeBinHeader
{
	char magic[4];
	_DWORD fileCount;
	int offsetTableOff;
	int extTableOff;
	_DWORD nameTableOff;
	int sizeTableOff;
	cFmergeBinMetaHeader* metaTableOff;
};

struct cFmerge
{
	cFmergeBinHeader* header;
};