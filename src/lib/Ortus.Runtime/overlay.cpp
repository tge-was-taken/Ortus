#include "pch.h"
#include "overlay.h"

#include <d3d9.h>

#include "lib/ImGui/imgui.h"
#include "lib/ImGui/backends/imgui_impl_dx9.h"
#include "lib/ImGui/backends/imgui_impl_win32.h"

#include <vector>

#include "callbacks.h"

namespace ortus::overlay {

    struct RenderTextInfo
    {
        float x;
        float y;
        float size;
        int color;
        const char* text;
    };

    struct EntitySpawnInfo
    {
        char name[128];
        EntityTransform transform;
        int id;
    };

    static std::vector<RenderTextInfo> sRenderTextQueue{};
    static std::vector<EntitySpawnInfo> sEntitySpawnQueue{};

    VTABLE_HOOK(HRESULT, __stdcall, IDirect3DDevice9, EndScene)
    {
        static bool init = false;
        static bool detachRequested = false;
        static bool showInfo = true;
        static bool showHacks = true;
        static bool showPlayer = true;
        static bool showSpawner = true;

        if (!init)
        {
            ImGui::CreateContext();
            ImGuiIO& io = ImGui::GetIO();
            io.ConfigFlags = ImGuiConfigFlags_NoMouseCursorChange;
            ImGui_ImplWin32_Init(*game::g_hWnd);
            ImGui_ImplDX9_Init(This);
            ImGui::StyleColorsDark();
            init = true;
        }

        if (!init) 
            return originalIDirect3DDevice9EndScene(This);

        ImGui_ImplDX9_NewFrame();
        ImGui_ImplWin32_NewFrame();
        ImGui::NewFrame();

        ImGui::BeginMainMenuBar();
        ImGui::MenuItem("phase", nullptr, &showInfo, true);
        ImGui::MenuItem("player info", nullptr, &showPlayer, true);
        ImGui::MenuItem("entity spawner", nullptr, &showSpawner, true);
        ImGui::EndMainMenuBar();

        if (showInfo)
        {
            ImGui::Begin("phase");
            {
                auto pm = game::g_PhaseManager;

                auto stringOrEmpty = [](const char* s) {
                    if (!s) return "";
                    return s;
                };

                {
                    ImGui::InputInt("state", &pm->state);

                    ImGui::InputInt("prevPhaseId", &pm->prevPhaseId, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::InputInt("prevPhaseNameHash", &pm->prevPhaseNameHash, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::LabelText("prevPhaseName", pm->prevPhaseName);
                    ImGui::InputInt("prevField5C", &pm->prevField5C);

                    ImGui::InputInt("phaseId", &pm->phaseId, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::InputInt("phaseNameHash", &pm->phaseNameHash, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::LabelText("phaseName", pm->phaseName);
                    ImGui::InputInt("field5C", &pm->field_5C);

                    ImGui::InputInt("nextPhaseId2", &pm->nextPhaseId2, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::InputInt("nextPhaseNameHash2", &pm->nextPhaseNameHash2, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::LabelText("nextPhaseName2", pm->nextPhaseName2);
                    ImGui::InputInt("dword88", &pm->dword88);

                    ImGui::InputInt("nextPhaseId", &pm->nextPhaseId, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::InputInt("phaseNameHash", &pm->nextPhaseNameHash, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::LabelText("phaseName", pm->nextPhaseName);
                    ImGui::InputInt("nextField5C", &pm->nextField5C);

                    ImGui::LabelText("subPhaseName", pm->subPhaseName != nullptr ? pm->subPhaseName : "");
                    ImGui::InputFloat4("playerPos", (float*)&pm->playerPos);
                    ImGui::InputFloat4("playerRot", (float*)&pm->playerRot);
                    ImGui::InputInt("roomId", &pm->roomNoMaybe, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::Checkbox("isRestartPoint", (bool*)&pm->isRestartPoint);
                    ImGui::InputInt("saveRestartPos", &pm->saveRestartPos);
                    ImGui::InputFloat4("cameraYaw", (float*)&pm->cameraYaw);
                    ImGui::Checkbox("cameraEnable", (bool*)&pm->cameraEnable);
                    ImGui::Checkbox("cameraXEnable", (bool*)&pm->cameraXEnable);
                    ImGui::Checkbox("cameraYEnable", (bool*)&pm->cameraYEnable);
                    ImGui::Checkbox("isPlWaitPayment", (bool*)&pm->isPlWaitPayment);
                    ImGui::Checkbox("maybePlayerLoadedFlag", (bool*)&pm->maybePlayerLoadedFlag);
                }

                ImGui::Begin("load phase");
                {
                    static char phaseName[128];
                    static int phaseId{};
                    static int eventId = -1;
                    ImGui::InputText("phaseName", phaseName, sizeof(phaseName));
                    ImGui::InputInt("phaseId", &phaseId, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                    ImGui::InputInt("eventId", &eventId, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);

                    if (ImGui::Button("load"))
                    {
                        callbacks::SceneUpdate.registerCallback([]()
                            {
                                game::cScene__startPhase(game::g_Scene, nullptr, phaseId, phaseName, eventId);
                                return callbacks::Status::Unregister;
                            });
                    }
                }
                ImGui::End();

                ImGui::Begin("load subphase");
                {
                    static char name[128];
                    static bool flag = true;
                    static int unknown = 0;
                    ImGui::InputText("name", name, sizeof(name));
                    ImGui::Checkbox("flag", &flag);
                    ImGui::InputInt("unknown", &unknown);

                    if (ImGui::Button("load"))
                    {
                        callbacks::SceneUpdate.registerCallback([]()
                            {
                                game::PhaseManager__requestSubPhaseChange(game::g_PhaseManager, nullptr, name, flag, unknown);
                                return callbacks::Status::Unregister;
                            });
                    }
                    ImGui::End();
                }
            }
            ImGui::End();
        }

        if (showPlayer)
        {
            ImGui::Begin("player info");
            {
                if (game::g_Scene->playerEntity)
                {
                    ImGui::InputFloat3("pos", (float*)&game::g_Scene->playerEntity->behavior->m.base.base.base.base.pos);
                    ImGui::InputFloat4("rot", (float*)&game::g_Scene->playerEntity->behavior->m.base.base.base.base.rot);
                }
                ImGui::InputInt("player type", game::g_playerType);
                ImGui::InputInt("player wig id", game::g_playerWigId);
                ImGui::InputInt("player wig id 2", game::g_playerWigId2);
                ImGui::InputInt("player model id", game::g_playerModelId);
                ImGui::InputInt("player model id 2", game::g_playerModelId2);
                ImGui::InputInt("dlc player model id", game::g_dlcPlayerModelId);
            }
            ImGui::End();
        }

        if (showSpawner)
        {
            ImGui::Begin("entity spawner");
            {
                static char name[128];
                static int id{};
                static EntityTransform transform{};
                ImGui::InputText("name", name, sizeof(name));
                ImGui::InputInt("id", &id, 1, 100, ImGuiInputTextFlags_CharsHexadecimal);
                ImGui::InputFloat3("pos", (float*)&transform.pos);
                ImGui::InputFloat4("rot", (float*)&transform.rot);
                if (ImGui::Button("fetch from player"))
                {
                    if (game::g_Scene->playerEntity)
                    {
                        transform.pos = game::g_Scene->playerEntity->behavior->m.base.base.base.base.pos;
                        transform.rot = game::g_Scene->playerEntity->behavior->m.base.base.base.base.rot;
                    }
                }

                if (ImGui::Button("spawn"))
                {
                    callbacks::SceneUpdate.registerCallback([]() 
                    {
                        game::EntitySystem__createEntity(game::g_EntitySystem, nullptr, name, id, &transform);
                        return callbacks::Status::Unregister;
                    });
                }

            }
            ImGui::End();
        }

        // render debug text
        for (int i = sRenderTextQueue.size() - 1; i >= 0; i--)
        {
            auto& info = sRenderTextQueue[i];
            ImGui::GetForegroundDrawList()->AddText({ info.x, info.y }, info.color, info.text);
        }
        sRenderTextQueue.clear();

        ImGui::EndFrame();
        ImGui::Render();
        ImGui_ImplDX9_RenderDrawData(ImGui::GetDrawData());
        return originalIDirect3DDevice9EndScene(This);
    }

    HOOK(LRESULT, CALLBACK, WndProc, ASLR(0xDF9610), HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
    {
        auto res = ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam);
        return orig_WndProc(hWnd, uMsg, wParam, lParam);
    }

    HOOK(void, __cdecl, debugRenderText, ASLR(0xF963B0), float x, float y, float size, int color, const char* text)
    {
        // TODO scale to resolution
        sRenderTextQueue.push_back({ x, y, size, color, text });
    }

    void init()
    {
        callbacks::GamePostInit.registerCallback([]() 
        {
            INSTALL_VTABLE_HOOK(IDirect3DDevice9, *game::g_pD3DDevice, EndScene, 42);
            INSTALL_HOOK(WndProc);
            INSTALL_HOOK(debugRenderText);
            return callbacks::Status::Unregister;
        });
    }
}