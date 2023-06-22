#pragma once

#include "game.h"
#include <functional>

namespace ortus::fs
{
	enum class HandlerStatus {
		Continue,
		Stop
	};

	typedef void RootFileCallback(struct RootFileHandlerCtx& ctx);
	typedef HandlerStatus RootFileHandler(struct RootFileHandlerCtx& ctx);
	struct RootFileHandlerCtx {
		std::function<RootFileCallback> callback;
		void* pThis;
		char* a2;
		const char* pFilePath;
		int a4;
		char a5;
		int a6;
		FileHandle* pResult;
	};

	typedef void ArchiveFileCallback(struct ArchiveFileHandlerCtx& ctx);
	typedef HandlerStatus ArchiveFileHandler(struct ArchiveFileHandlerCtx& ctx);
	struct ArchiveFileHandlerCtx {
		std::function<ArchiveFileCallback> callback;
		void* pThis;
		const char* pName;
		void* pResult;
	};

	void addRootFileHandler(std::function<RootFileHandler> handler);
	void addArchiveFileHandler(std::function<ArchiveFileHandler> handler);
	void init();
}