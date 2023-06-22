#pragma once

namespace Hw 
{
	struct cFactory
	{
		struct entry
		{
			int id;
			void* newWorkFunc;
			int priority;
			const char* name;
		};
	};
}