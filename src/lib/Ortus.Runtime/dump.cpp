#include "pch.h"
#include "dump.h"
#include "callbacks.h"
#include "app.h"
#include <filesystem>

namespace ortus::dump
{
	static void dumpEntityTable()
	{
		std::string path = app::getCwd() + "/entities.csv";
		if (std::filesystem::exists(path))
			return;

		std::fstream file(path, std::ios::out);
		if (!file.is_open())
			DebugBreak();

		file << "id,newWorkFunc,priority,name" << std::endl;

		for (size_t i = 0; i < game::FACTORY_ENTRY_COUNT; i++)
		{
			auto& entry = game::g_FactoryEntries[i];
			file << std::hex << entry.id << "," << entry.newWorkFunc << "," << entry.priority << "," << entry.name << std::endl;
		}

		file.flush();
		file.close();
	}

	static void dumpPhaseTable()
	{
		std::string path = app::getCwd() + "/phases.csv";
		if (std::filesystem::exists(path))
			return;

		std::fstream file(path, std::ios::out);
		if (!file.is_open())
			DebugBreak();

		file << "id,createFunc" << std::endl;

		for (size_t i = 0; i < game::PHASE_TABLE_COUNT; i++)
		{
			auto& entry = game::g_PhaseTable[i];
			file << std::hex << entry.id << "," << entry.create << std::endl;
		}

		file.flush();
		file.close();
	}

	void init()
	{
		callbacks::GamePostInit.registerCallback([]() 
		{
			dumpEntityTable();
			dumpPhaseTable();
			return callbacks::Status::Unregister;
		});
	}
}