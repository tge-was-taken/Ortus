#pragma once

#include "types.h"
#include "sys.h"

namespace lib {
	template<typename T, int N>
	struct StaticArray {
		_DWORD dword0;
		_DWORD dword4;
		_DWORD dword8;
		_DWORD dwordC;
	};

	namespace helper {
		struct AllocatorProxy {
			template <typename T>
			struct CoreT {

			};
		};

		template<typename T>
		struct DeleterByAllocator {

		};
	}

	namespace detail {
		template<typename T, typename T2, typename T3>
		struct SharedCoreImpl {

		};
	}

	struct __declspec(align(4)) struct_this_6
	{
		lib::helper::AllocatorProxy::CoreT<sys::AllocatorByHeap>* allocator;
		lib::detail::SharedCoreImpl<lib::helper::AllocatorProxy::CoreT<sys::AllocatorByHeap>,
			lib::helper::DeleterByAllocator<sys::AllocatorByHeap>, sys::AllocatorByHeap>* sharedCore;
	};

	template<typename T>
	struct AllocatedArray {
		_DWORD __vftable;
		_DWORD dword4;
		_DWORD dword8;
		_DWORD dwordC;
		struct_this_6 allocatorSharedCore;
	};
}