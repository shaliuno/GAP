using ImGuiNET;
using Color = System.Drawing.Color;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using System.Diagnostics;
using sh = Stas.GA.SpriteHelper;

namespace Stas.GA;
public partial class DrawMain {
    void DrawExpedSett() {
        if (ui.exped_sett == null) {
            ImGui.Button("exped_sett==null");
            return;
        }
        var deton = ui.curr_map.exped_detonator;
        if (deton == null) {
            ImGui.Button("I don't see the blasting machine. " +
                "\nIf you reloaded the map, just get close enough" +
                "\n to it to get it to load");
        }
        else {
            DrawContent();
        }
        if (ImGui.Button("Reload sett")) {
            ui.exped_sett = new ExpedSett().Load<ExpedSett>();
        }
        ImGui.SameLine();
        if (ImGui.Button("Reset sett")) {
            ui.exped_sett = new ExpedSett();
            ui.exped_sett.Save();
        }
        void DrawContent() {
            ImGui.SetNextItemWidth(150);
            if (ImGui.SliderInt("Radius", ref ui.exped_sett.radius_persent, 0, 100)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("Increased Explosive Radius(35% from tree)");
            ImGui.SetNextItemWidth(150);
            ImGui.SameLine();
            if (ImGui.SliderInt("Range", ref ui.exped_sett.range_persent, 0, 100)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("increased Explosive plascement range(40% from tree)");


            if (ImGui.Checkbox("Phys", ref ui.exped_sett.PhysImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("PhysImmune");
            ImGui.SameLine();

            if (ImGui.Checkbox("Fire", ref ui.exped_sett.FireImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("FireImmune");
            ImGui.SameLine();

            if (ImGui.Checkbox("Cold", ref ui.exped_sett.ColdImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ColdImmune");
            ImGui.SameLine();

            if (ImGui.Checkbox("Lightn", ref ui.exped_sett.LightningImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("LightningImmune");
            ImGui.SameLine();

            if (ImGui.Checkbox("Chaos", ref ui.exped_sett.ChaosImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ChaosImmune");

            if (ImGui.Checkbox("Ailment", ref ui.exped_sett.AilmentImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("AilmentImmune");
            ImGui.SameLine();

            //NEw line
            if (ImGui.Checkbox("Crit", ref ui.exped_sett.CritImmune)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("CritImmune");
            ImGui.SameLine();

            if (ImGui.Checkbox("Culling", ref ui.exped_sett.Culling)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("CullingStrikeTwentyPercent");
            ImGui.SameLine();

            if (ImGui.Checkbox("Corrupt", ref ui.exped_sett.CorruptedItems)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ExpeditionCorruptedItemsElite");
            ImGui.SameLine();

            if (ImGui.Checkbox("Regen", ref ui.exped_sett.Regen)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ElitesRegenerateLifeEveryFourSeconds");

            //NEw line
            if (ImGui.Checkbox("Block", ref ui.exped_sett.BlockChance)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("AttackBlockSpellBlockMaxBlockChance");
            ImGui.SameLine();

            if (ImGui.Checkbox("Resist", ref ui.exped_sett.MaxResistances)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ResistancesAndMaxResistances");
            ImGui.SameLine();

            if (ImGui.Checkbox("Leech", ref ui.exped_sett.NoLeech)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("CannotBeLeechedFrom");
            ImGui.SameLine();

            if (ImGui.Checkbox("Curse", ref ui.exped_sett.NoCurse)) {
                ui.exped_sett.Save();
            }
            ImGuiExt.ToolTip("ImmuneToCurses");

        }
    }
}
