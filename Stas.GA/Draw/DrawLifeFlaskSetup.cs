using ImGuiNET;
using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

partial class DrawMain {
    void DrawLifeFlaskSetup() {
        //ui.SetDebugPossible(null);
        var flist = ui.flask_keys.Select(x => x.ToString()).ToArray();

        if (ui.worker != null) {
            if (ImGui.Checkbox("Life  ", ref ui.worker.b_use_life_flask)) {
                ui.worker.Save();
            }
        }
        else {
            if (ImGui.Checkbox("Life  ", ref ui.sett.b_use_life_flask))
                ui.sett.Save();
        }
        ImGuiExt.ToolTip("Will also use this flask if you left [%] life");

        ImGui.SameLine();
        ImGui.SetNextItemWidth(40);
        if (ui.worker != null) {
            var mf_index = ui.flask_keys.IndexOf(ui.worker.life_flask_key);
            if (ImGui.Combo("Key ", ref mf_index, flist, flist.Length)) {
                ui.worker.life_flask_key = ui.flask_keys[mf_index];
                ui.worker.Save();
            }
        }
        else {
            var mf_index = ui.flask_keys.IndexOf(ui.sett.life_flask_key);
            if (ImGui.Combo("Key ", ref mf_index, flist, flist.Length)) {
                ui.sett.life_flask_key = ui.flask_keys[mf_index];
                ui.sett.Save();
            }
        }
        ImGuiExt.ToolTip("The hot button to be used for the mana flask");

        ImGui.SetNextItemWidth(60);
        ImGui.SameLine();
        if (ui.worker == null) {
            if (ImGui.SliderInt("last %   ", ref ui.sett.trigger_life_left_persent, 30, 80)) {
                ui.sett.Save();
            }
        }
        else {
            if (ImGui.SliderInt("last %   ", ref ui.worker.min_life_percent, 30, 80)) {
                ui.worker.Save();
            }
        }
        ImGuiExt.ToolTip("Set the triggering percentage (default==50)");

        ImGui.SetNextItemWidth(60);
        ImGui.SameLine();
        if (ui.worker == null) {
            if (ImGui.SliderInt("Time(ms) ", ref ui.sett.life_flask_ms, 1000, 4000)) {
                ui.sett.Save();
            }
        }
        else {
            if (ImGui.SliderInt("Time(ms) ", ref ui.worker.life_flask_ms, 1000, 4000)) {
                ui.worker.Save();
            }
        }
        ImGuiExt.ToolTip("Life flask action time");
    }
}
