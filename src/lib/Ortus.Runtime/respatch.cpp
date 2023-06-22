#include "pch.h"
#include "respatch.h"

namespace ortus::respatch
{
	void init()
	{
        // patch switch case in initWindow()
        WRITE_MEMORY(ASLR(0xA1D6B1) + 1, int, (int)config::respatch::width);
        WRITE_MEMORY(ASLR(0xA1D6B6) + 1, int, (int)config::respatch::height);

        // patch switch case in Graphics::Startup
        WRITE_MEMORY(ASLR(0xA30EAA) + 1, int, (int)config::respatch::width);
        WRITE_MEMORY(ASLR(0xA30EB9) + 1, int, (int)config::respatch::height);

        // patch resolution floats for 1080
        WRITE_MEMORY(ASLR(0x16EBF20), float, (float)config::respatch::width);
        WRITE_MEMORY(ASLR(0x16EBF1C), float, (float)config::respatch::height);
	}
}