#pragma once

namespace ortus::config {

	namespace general {
		inline bool enable = true;
	}

	namespace log {
		inline bool enable = true;
		inline bool restoreDebugPrints = true;
		inline bool logEntities = true;
		inline bool logFileAccesses = true;
	}

	namespace debug {
		inline bool enable = true;
	}

	namespace respatch {
		inline bool enable = false;
		inline int width = 1920;
		inline int height = 1080;
	}

	namespace overlay {
		inline bool enable = true;
	}

	namespace playas {
		inline bool enable = false;
		inline int playerType = 8;
	}

	namespace fastboot {
		inline bool enable = true;
		inline bool skipLogos = true;
		inline int bootPhaseId = 0xF01;
		inline bool uncapTitleScreen = true;
		inline bool ignoreSteamNotRunning = true;
		inline bool skipTitleScreenAnimation = true;
		inline bool autoUnlock = true;
	}

	namespace dump {
		inline bool enable = true;
	}

	void init();

} // namespace ortus::config