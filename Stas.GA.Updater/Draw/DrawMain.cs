using ClickableTransparentOverlay;
using ImGuiNET;
using Stas.Utils;
using System.Drawing;
using System.Numerics;

namespace Stas.GA.Updater;
partial class DrawMain : Overlay {
    public IntPtr icons;
    static string _title;
    static bool b_minimize = false;
    public DrawMain(string title):base(title) {
        _title = title;
    }
    int fi = 0;
    protected override void Render() {
        if (!ui.b_running)
            return;
        var isMainMenuExpanded = ImGui.Begin(_title!, ImGuiWindowFlags.AlwaysAutoResize 
            | ImGuiWindowFlags.NoFocusOnAppearing);
           //ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.MenuBar
        if (!isMainMenuExpanded) {
            ImGui.End();
            return;
        }
        info_ptr = ImGui.GetWindowDrawList();
        if (ImGui.CollapsingHeader("Setup information - study pls")) {
            var text = "edit your names in the file Stas.GA.Updater.Settings.sett" +
                  "\r\nfields: boosty_name and discord_name and press for reload...";
            DrawInfoOnQuad(text);

            ImGui.Text("Your name on boosty.to/gameassist");
            ImGui.SameLine();
            if (ImGui.Button(ui.sett.boosty_mail)) {
                ui.sett = new Settings().Load<Settings>();
            }
            ToolTip("click this to load the current value from the settings file");

            ImGui.Text("your name (with #) on my discord...");
            ImGui.SameLine();
            if (ImGui.Button(ui.sett.discord_name)) {
                ui.sett = new Settings().Load<Settings>();
            }
            ToolTip("click this to load the current value from the settings file");

        }

        ImGui.Separator();
        if (ui.curr_state >= State.Connected) {
            ImGui.PushStyleColor(ImGuiCol.Button, Color.Green.ToImgui());
            ImGui.Button("Conn ok");
        }
        if (ui.curr_state != State.Auth_ok) {
            ImGui.PushStyleColor(ImGuiCol.Button, Color.Red.ToImgui());
        }
        else
            ImGui.PushStyleColor(ImGuiCol.Button, Color.Green.ToImgui());
        ImGui.Button(ui.curr_state.ToString());
        ImGui.PopStyleColor();

        // DrawDisabledButton("Sign UP");
        if (ImGui.Button("sign up")) {
            ui.SetDebugPossible(() => {
                ui.SignUp();
            });
        }
        ImGui.SameLine();
        if (ImGui.Button("Login")) {
            ui.SetDebugPossible(null);
            ui.Login();

        }
        ImGui.SameLine();
        if (ImGui.Button("Quit")) {
            ui.sett.Save();
            ui.SetDebugPossible(null);
            ui.b_running = false;
            Thread.Sleep(500);
            ImGui.End();
            base.Close();

            Environment.Exit(-1);
        }

        //new line
        ImGui.SameLine();
        if (ImGui.Checkbox("AutoBot", ref ui.sett.b_auto_bot)) {
            ui.sett.Save();
        }
        ToolTip(" restart bot if it not loaded");

        ImGui.SameLine();
        if (ImGui.Checkbox("AutoGame", ref ui.sett.b_auto_game)) {
            ui.sett.Save();
        }
        ToolTip("restart game if it not loaded\n" +
            "and log in with the saved login\n " +
            "for the last character you played");

        ImGui.Separator();
        //new line
        DrawLog(ui.log);
        ImGui.Separator();
        //ImGui.NewLine();
        if (File.Exists("image.png")) {
            AddOrGetImagePointer(
                "image.png",
                false,
                out IntPtr imgPtr,
                out uint w,
                out uint h);
            ImGui.Image(imgPtr, new Vector2(w, h));
        }
        else {
            ImGui.Text("advertising spot");
        }
        ImGui.End();
    }
    public static uint ImGuiColor(uint r, uint g, uint b, uint a) {
        return (a << 24) | (b << 16) | (g << 8) | r;
    }
    void DrawDisabledButton(string buttonLabel) {
        var col = ImGuiColor(204, 204, 204, 128);
        ImGui.PushStyleColor(ImGuiCol.Button, col);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, col);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, col);
        ImGui.Button(buttonLabel);
        ImGui.PopStyleColor(3);
    }
    void ToolTip(string text) {
        if (ImGui.IsItemHovered()) {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(text);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }
    ImDrawListPtr info_ptr;
    void DrawInfoOnQuad(string text) {
        var impTextColor = Color.White.ToImguiVec4();
        var sp = ImGui.GetCursorScreenPos();
        var ts = ImGui.CalcTextSize(text);
        var lt = sp;
        var rt = sp.Increase(ts.X, 0);
        var rb = sp.Increase(ts.X, ts.Y);
        var lb = sp.Increase(0, ts.Y);
        info_ptr.AddQuadFilled(lt, rt, rb, lb, Color.Black.ToImgui());
        ImGui.TextColored(impTextColor, text);
    }
}