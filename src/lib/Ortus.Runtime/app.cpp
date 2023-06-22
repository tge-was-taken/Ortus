#include "pch.h"
#include "app.h"

#include <stdio.h>
#include <varargs.h>

#include "game.h"
#include "overlay.h"
#include "playas.h"
#include "fs.h"
#include "fastboot.h"
#include "log.h"
#include "debug.h"
#include "respatch.h"
#include "callbacks.h"
#include "dump.h"

namespace ortus::app
{
    static std::string sWorkingDir{};

    const std::string& const getCwd()
    {
        if (sWorkingDir.empty())
        {
            char buf[1024];
            GetCurrentDirectoryA(sizeof(buf), buf);
            sWorkingDir = buf;
        }

        return sWorkingDir;
    }

	void init()
	{
        config::init();
        if (!config::general::enable)
            return;

        callbacks::init();

        if (config::log::enable)
            log::init();

        log::info("CWD %s\n", getCwd());

        if (config::debug::enable)
            debug::init();

        fs::init();

        if (config::respatch::enable)
            respatch::init();

        if (config::fastboot::enable)
            fastboot::init();

        if (config::playas::enable)
            playas::init();

        if (config::overlay::enable)
            overlay::init();

        if (config::dump::enable)
            dump::init();
	}
}