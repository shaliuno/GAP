using ImGuiNET;
using System.Drawing;
using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;
namespace Stas.GA;
partial class DrawMain {
    //https://github.com/ocornut/imgui/blob/169e3981fdf037b179f1c7296548892ba7837dae/imgui_demo.cpp#L3871-L3978

    bool on_top => ui.b_game_top || ui.b_imgui_top;
    bool b_map => ui.curr_map != null && ui.curr_map.b_ready;
    V2 my_display_res;
    void DrawMap() {
        my_display_res = new V2(ui.game_window_rect.Width, ui.game_window_rect.Height);
        ImGui.SetNextWindowContentSize(my_display_res);
        ImGui.SetNextWindowPos(new V2(ui.w_offs.X, ui.w_offs.Y));
        ImGui.Begin(
            "Background Screen",
                ImGuiWindowFlags.NoInputs |
                ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar);

        map_ptr = ImGui.GetWindowDrawList();
        if (ui.b_game_top || ui.b_imgui_top) { // && ui.b_debug
            DrawDebug();
        }

        if (ui.sett.b_league_start) {
            DrawMapContent();
        }
        else {
            if (b_map && on_top && !ui.b_busy//&& !ui.sett.test_draw
                && !ui.b_draw_bad_centr && !ui.b_draw_save_screen) {
                DrawMePos();
                DrawMapContent();
                b_cant_draw_map = false;
            }
            else {
                b_cant_draw_map = true;
                ui.AddToLog("can't draw map..", MessType.Warning);
            }
        }
        ImGui.End();
    }
    void DrawMePos() {
        if (!ui.sett.b_draw_me_pos)
            return;
        var cpa = ui.curr_map.me_pos.ToArray();//thread safe copy of
        if (cpa.Length < 4)
            return;
        var rm = ui.MTransform();
        for (int i = 0; i < cpa.Length - 1; i++) {
            var p1 = V2.Transform(cpa[i], rm);
            var p2 = V2.Transform(cpa[i + 1], rm);
            map_ptr.AddLine(p1, p2, Color.Red.ToImgui(), 2f);
        }
    }
    SW sw_map = new SW("Map");
}