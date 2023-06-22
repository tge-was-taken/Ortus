#include "pch.h"
#include "config.h"
#include "app.h"

#include "lib/inipp/inipp.h"
#include <iostream>
#include <fstream>

namespace ortus::config {
	static bool tryGetValue(inipp::Ini<char> ini, std::string sectionKey, std::string valueKey,
		std::string& outValue)
	{
		if (!ini.sections.contains(sectionKey))
			return false;

		auto& section = ini.sections[sectionKey];
		if (!section.contains(valueKey))
			return false;

		outValue = section[valueKey];
		return true;
	}

	static bool getBool(inipp::Ini<char> ini, std::string sectionKey, std::string valueKey, bool defaultValue)
	{
		std::string value;
		if (!tryGetValue(ini, sectionKey, valueKey, value))
			return defaultValue;

		return _stricmp(value.c_str(), "true") == 0;
	}

	static int getInt(inipp::Ini<char> ini, std::string sectionKey, std::string valueKey, int defaultValue, bool isHex = false)
	{
		std::string value;
		if (!tryGetValue(ini, sectionKey, valueKey, value))
			return defaultValue;

		auto radix = isHex ? 16 : 10;
		auto offset = 0;
		if (value.starts_with("0x"))
		{
			radix = 16;
			offset = 2;
		}
	
		return std::strtol(value.c_str() + offset, nullptr, radix);
	}

	void init()
	{
		std::ifstream stream(app::getCwd() + "/Ortus.Runtime.ini");
		if (!stream.is_open())
			return;

		inipp::Ini<char> ini;
		ini.parse(stream);

		// load general config
		general::enable = getBool(ini, "general", "enable", true);

		// load log config
		log::enable = getBool(ini, "log", "enable", true);
		log::restoreDebugPrints = getBool(ini, "log", "restore_debug_prints", true);
		log::logEntities = getBool(ini, "log", "log_entities", true);
		log::logFileAccesses = getBool(ini, "log", "log_file_accesses", true);

		// load debug config
		debug::enable = getBool(ini, "debug", "enable", false);

		// load resolution patch config
		respatch::enable = getBool(ini, "resolution_patch", "enable", false);
		respatch::width = getInt(ini, "resolution_patch", "width", 1920);
		respatch::height = getInt(ini, "resolution_patch", "height", 1080);

		// load overlay config
		overlay::enable = getBool(ini, "overlay", "enable", false);

		// load playas config
		playas::enable = getBool(ini, "playas", "enable", false);
		playas::playerType = getInt(ini, "playas", "player_type", 8);

		// load fastboot config
		fastboot::enable = getBool(ini, "fastboot", "enable", true);
		fastboot::skipLogos = getBool(ini, "fastboot", "skip_logos", true);
		fastboot::bootPhaseId = getInt(ini, "fastboot", "boot_phase_id", 0xf01, true);
		fastboot::uncapTitleScreen = getBool(ini, "fastboot", "uncap_title_screen_fps", true);
		fastboot::ignoreSteamNotRunning = getBool(ini, "fastboot", "ignore_steam_not_running", true);
		fastboot::skipTitleScreenAnimation = getBool(ini, "fastboot", "skip_title_screen_animation", false);
		fastboot::autoUnlock = getBool(ini, "fastboot", "auto_unlock", false);
	}

} // namespace ortus::config