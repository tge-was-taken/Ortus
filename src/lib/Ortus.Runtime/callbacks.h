#pragma once

#include <mutex>
#include <functional>

namespace ortus::callbacks 
{
	enum class Status
	{
		Unregister,
		Keep
	};

	using CallbackFn = Status();

	template<typename HookFunc, int Addr>
	struct CallbackListBase
	{
	public:
		inline static HookFunc* mOrig;

		inline static std::mutex mMutex;
		inline static std::list<std::function<CallbackFn>> mList;

		inline static void executeCallbacks()
		{
			std::scoped_lock(mMutex);

			auto cur = mList.cbegin();
			while (cur != mList.cend())
			{
				auto next = std::next(cur);

				if ((*cur)() == Status::Unregister)
					mList.erase(cur);
				cur = next;
			}
		}

	public:
		inline CallbackListBase()
		{
			mOrig = (HookFunc*)(ASLR(Addr));
		}

		inline void registerCallback(CallbackFn cb)
		{
			std::scoped_lock(mMutex);
			mList.push_back(cb);
		}
	};

	template<typename HookFunc, int Addr>
	struct CallbackListCDecl : CallbackListBase<HookFunc, Addr>
	{
		inline static void __cdecl impl()
		{
			CallbackListCDecl::executeCallbacks();
			return CallbackListCDecl::mOrig();
		}

	public:
		inline CallbackListCDecl()
		{
			INSTALL_HOOK3(CallbackListCDecl::mOrig, CallbackListCDecl::impl);
		}
	};

	template<typename HookFunc, int Addr>
	struct CallbackListThisCall : CallbackListBase<HookFunc, Addr>
	{
		inline static void __fastcall impl(void* pThis)
		{
			CallbackListThisCall::executeCallbacks();
			return CallbackListThisCall::mOrig(pThis);
		}

	public:
		inline CallbackListThisCall()
		{
			INSTALL_HOOK3(CallbackListThisCall::mOrig, CallbackListThisCall::impl);
		}
	};

	/// <summary>
	/// After init, before first update
	/// </summary>
	inline CallbackListCDecl<void(), 0xA52E10> GamePostInit;

	/// <summary>
	/// Called before scene update.
	/// </summary>
	inline CallbackListCDecl<void(), 0xA54430> SceneUpdate;

	/// <summary>
	/// Called before sequence update.
	/// </summary>
	inline CallbackListThisCall<void(void*), 0xA528A0> SequenceUpdate;
	
	void init();
}