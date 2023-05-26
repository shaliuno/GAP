using ClickableTransparentOverlay;
using ImGuiNET;
using System.Text;
namespace Stas.GA;
public partial class DrawMain : Overlay {
    public IntPtr icons;
    public string title_name { get; }
    public static bool b_ready = false;
    public DrawMain(string title) : base(title, true) {
    }
    protected override Task PostInitialized() {
        AddOrGetImagePointer(ui.sett.icons_fname, true, out var p, out var _, out var _);
        icons = p;
        b_ready = true;
        return Task.CompletedTask;
    }

    ImDrawListPtr map_ptr;
    StringBuilder sp_warn = new StringBuilder();
    SW sw_main = new SW("Draw Map:");
    int fi = 0;
    bool b_clickable = false;
    protected override void Render() {
        var b_top = ui.b_game_top || ui.b_imgui_top;
        var test_ui_elem = ui.test_elem != null && ui.sett.b_gui_debug_on_top && !ui.b_alt;
        if (test_ui_elem || ui.b_alt_shift) { //  &&
            b_clickable = true;
        }
        else {
            b_clickable = false;
        }
        if (ui.b_vs) {
            return;
        }
        if (ui.test_elem != null && ui.sett.b_develop) { // && ui.tasker.ui_root.IsValid
            DrawUiFrames();
            DrawInfo();
            return;
        }
        sw_main.Restart();
        DrawMap(); //we need init map_ptr for debug same on it window, so check map visible cond insade
        sw_main.MakeRes();
        sp_warn.Clear();
        var me_wrong = ui.me == null || ui.me.Address == default || !ui.me.IsValid;
        if (!on_top) sp_warn.Append("NOT on top... ");
        if (me_wrong) sp_warn.Append("me is Wrong... ");
        if (ui.sett.b_debug) sp_warn.Append("w8 debug... ");
        if (!ui.b_ingame) sp_warn.Append("NOT in game... ");

        ui.warning = sp_warn.ToString();
        if ((on_top && !ui.b_busy) || ui.b_show_info_over  || ui.sett.b_league_start)
            DrawInfo();
    }
}