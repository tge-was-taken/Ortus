#include "pch.h"
#include "fs.h"
#include "log.h"

namespace ortus::fs
{
	static std::vector<std::function<RootFileHandler>> sRootFileHandlers;
	static std::vector<std::function<ArchiveFileHandler>> sArchiveFileHandlers;
	static std::unordered_map<void*, FileHandle*> sBufferToFileHandle;

	static void log(const char* fmt, ...)
	{
		if (config::log::logFileAccesses)
		{
			va_list args;
			va_start(args, fmt);
			ortus::log::log("FILE", fmt, args);
			va_end(args);
		}
	}

	static const char* getBufferFileName(void** pBuffers)
	{
		void* pBuffer = *pBuffers;
		for (size_t i = 0; i < game::g_FileManager->nextHandleIndex; i++)
		{
			auto* handle = game::g_FileManager->fileHandles[i];
			if (handle->pBuffer == pBuffer)
				return (const char*)&handle->filePath;
		}

		if (sBufferToFileHandle.count(pBuffer))
			return (const char*)&sBufferToFileHandle[pBuffer]->filePath;

		return nullptr;
	}

	HOOK(FileHandle*, __fastcall, FileRead__Manager__readFile, ASLR(0x00E9DF40),
		void* pThis, void* _, char* a2, const char* filePath, int a4, char a5, int a6)
	{
		auto _origRootFileCallback = [](RootFileHandlerCtx& ctx) {
			ctx.pResult = orig_FileRead__Manager__readFile(
				ctx.pThis, nullptr, ctx.a2, ctx.pFilePath, ctx.a4, ctx.a5, ctx.a6);
		};

		RootFileHandlerCtx ctx = { _origRootFileCallback, pThis, a2, filePath, a4, a5, a6 };

		log("read file %s", filePath);
		for (auto& handler : sRootFileHandlers)
		{
			if (handler(ctx) == HandlerStatus::Stop)
				return ctx.pResult;
		}
		
		ctx.pResult = orig_FileRead__Manager__readFile(ctx.pThis, _, 
			ctx.a2, ctx.pFilePath, ctx.a4, ctx.a5, ctx.a6);
		return ctx.pResult;
	}

	HOOK(void*, __fastcall, cFmerge__getFileByName, ASLR(0x00DE4500),
		void* pThis, void* _, const char* pName)
	{
		auto _origArchiveFileCallback = [](ArchiveFileHandlerCtx& ctx) {
			ctx.pResult = orig_cFmerge__getFileByName(
				ctx.pThis, nullptr, ctx.pName
			);
		};

		ArchiveFileHandlerCtx ctx = { _origArchiveFileCallback, pThis, pName };

		const char* pFileName = getBufferFileName((void**)pThis);

		log("read archive file %s %s", pFileName, pName);

		for (auto& handler : sArchiveFileHandlers)
		{
			if (handler(ctx) == HandlerStatus::Stop)
				return ctx.pResult;
		}

		ctx.pResult = orig_cFmerge__getFileByName(ctx.pThis, _, ctx.pName);
		return ctx.pResult;
	}

	HOOK(void*, __fastcall, cFmerge__getFileByName_2, ASLR(0x00DE3D30),
		void* pThis, void* _, int index, const char* pName, int a4)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (2) %s %s", pFileName, pName);
		return orig_cFmerge__getFileByName_2(pThis, _, index, pName, a4);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByName_1, ASLR(0x00DE3D80),
		void* pThis, void* _, int index, const char* pName)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (1) %s %s", pFileName, pName);
		return orig_cFmerge__getFileByName_1(pThis, _, index, pName);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByNameHash, ASLR(0x00DE45A0),
		void* pThis, void* _, const char* pName)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (hash) %s %s", pFileName, pName);
		return orig_cFmerge__getFileByNameHash(pThis, _, pName);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByName_0, ASLR(0x00DE46A0),
		void* pThis, void* _, const char* pName)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (0) %s %s", pFileName, pName);
		return orig_cFmerge__getFileByName_0(pThis, _, pName);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByNameHash_0, ASLR(0x00DE4710),
		void* pThis, void* _, const char* pName)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (hash 0) %s %s", pFileName, pName);
		return orig_cFmerge__getFileByNameHash_0(pThis, _, pName);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByExt, ASLR(0x00DE44B0),
		void* pThis, void* _, const char* pExt, int a3)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (ext) %s %s", pFileName, pExt);
		return orig_cFmerge__getFileByExt(pThis, _, pExt, a3);
	}

	HOOK(void*, __fastcall, cFmerge__getFileByExt_0, ASLR(0x00DE4650),
		void* pThis, void* _, const char* pExt, int a3)
	{
		const char* pFileName = getBufferFileName((void**)pThis);
		log("read archive file (ext 0) %s %s", pFileName, pExt);
		return orig_cFmerge__getFileByExt_0(pThis, _, pExt, a3);
	}

	HOOK(void*, __fastcall, FileManager__getFileBuffer, ASLR(0x00E9D0B0), 
		FileManager* pThis, void* _, int id)
	{
		using namespace game;
		unsigned int count; // esi
		unsigned int i; // edx
		FileHandle** ppHandle; // ecx
		FileHandle* pHandle; // eax
		void* pResult; // eax

		if (!id)
			goto LABEL_7;
		count = pThis->nextHandleIndex;
		i = 0;
		if (!count)
			goto LABEL_7;
		ppHandle = pThis->fileHandles;
		while (1)
		{
			pHandle = *ppHandle;
			if ((*ppHandle)->status)
			{
				if (pHandle->id == id)
					break;
			}
			++i;
			++ppHandle;
			if (i >= count)
				goto LABEL_7;
		}
		if (pHandle->status == 6)
		{
			pResult = pHandle->pBuffer;
			sBufferToFileHandle[pResult] = pHandle;
		}
		else
			LABEL_7:
		pResult = 0;
		return pResult;
	}

	void addRootFileHandler(std::function<RootFileHandler> handler)
	{
		sRootFileHandlers.push_back(handler);
	}

	void addArchiveFileHandler(std::function<ArchiveFileHandler> handler)
	{
		sArchiveFileHandlers.push_back(handler);
	}

	void init()
	{
		INSTALL_HOOK(FileRead__Manager__readFile);
		INSTALL_HOOK(FileManager__getFileBuffer);
		INSTALL_HOOK(cFmerge__getFileByName);
		INSTALL_HOOK(cFmerge__getFileByName_2);
		INSTALL_HOOK(cFmerge__getFileByName_1);
		INSTALL_HOOK(cFmerge__getFileByNameHash);
		INSTALL_HOOK(cFmerge__getFileByName_0);
		INSTALL_HOOK(cFmerge__getFileByNameHash_0);
		INSTALL_HOOK(cFmerge__getFileByExt);
		INSTALL_HOOK(cFmerge__getFileByExt_0);
	}
}