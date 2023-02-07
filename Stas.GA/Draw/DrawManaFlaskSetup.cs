using ImGuiNET;
using System.Diagnostics;
using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

partial class DrawMain {
    void DrawManaFlaskSetup() {
        //ui.SetDebugPossible(null); 
        var flist = ui.flask_keys.Select(x => x.ToString()).ToArray();


        if (ui.worker != null) {
            if (ImGui.Checkbox("Mana  ", ref ui.worker.b_use_mana_flask)) {
                ui.worker.Save();
            }
        }
        else {
            if (ImGui.Checkbox("Mana  ", ref ui.sett.b_use_mana_flask)) {
                ui.sett.Save();
            }
        }
        ImGuiExt.ToolTip("Will also use this flask if you do't have" +
            "\n enough mana to use the mainskill");

        ImGui.SameLine();
        ImGui.SetNextItemWidth(40);
        if (ui.worker != null) {
            var mf_index = ui.flask_keys.IndexOf(ui.worker.mana_flask_key);
            if (ImGui.Combo("mKey ", ref mf_index, flist, flist.Length)) {
                ui.worker.mana_flask_key = ui.flask_keys[mf_index];
                ui.worker.Save();
            }
        }
        else {
            var mf_index = ui.flask_keys.IndexOf(ui.sett.mana_flask_key);
            if (ImGui.Combo("mKey ", ref mf_index, flist, flist.Length)) {
                ui.sett.mana_flask_key = ui.flask_keys[mf_index];
                ui.sett.Save();
            }
        }
        ImGuiExt.ToolTip("The hot button to be used for the mana flask");


        ImGui.SetNextItemWidth(60);
        ImGui.SameLine();
        if (ui.worker == null) {
            if (ImGui.SliderInt("Price ", ref ui.sett.mana_cast_price, 5, 140)) {
                ui.sett.Save();
            }
        }
        else {
            if (ImGui.SliderInt("Price ", ref ui.worker.main.mana_cost, 5, 140)) {
                ui.worker.Save();
            }
        }   
        ImGuiExt.ToolTip("Set the remaining amount of mana " +
            "\nbelow which the flask will be used (default==20)\n" +
            "Note: if you have configured Worker, \n" +
            "this value is taken automatically");


        ImGui.SetNextItemWidth(60);
        ImGui.SameLine();
        if (ui.worker == null) {
            if (ImGui.SliderInt("mTime(ms) ", ref ui.sett.mana_flask_ms, 4000, 7000)) {
                ui.sett.Save();
            }
        }
        else {
            if (ImGui.SliderInt("mTime(ms) ", ref ui.worker.mana_flask_ms, 4000, 7000)) {
                ui.worker.Save();
            }
        }
        ImGuiExt.ToolTip("Mana flask action time");

        ImGui.SameLine();
        if (ui.worker != null) {
            if (ImGui.Checkbox("Auto", ref ui.worker.b_mana_use_auto)) {
                ui.worker.Save();
            }
        }
        else {
            if (ImGui.Checkbox("Auto", ref ui.sett.b_mana_use_auto)) {
                ui.sett.Save();
            }
        }
        ImGuiExt.ToolTip("automatically cast in the presence of danger nearby\n" +
            "For example, if there is a mod on the map...\n" +
            "Players cannot regenerate life, mana and ES.\n" +
            "Note: Vote for automatic activation of this option under the right conditions");
    }
}
