using System.Drawing;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

namespace Stas.GA;
public class SafeScreen : iSett {
    [JsonInclude]
    public int Width { get; protected private set; }
    [JsonInclude]
    public int Height { get; protected private set; }

    public Dictionary<string, Cell> Blocks = new();
    public Dictionary<string, Cell> Rounts = new();
    public Dictionary<string, Cell> Centr = new();
    List<(string, V2, V2)> CentrPoints = new();
    List<(string, V2, V2)> RoutPoints = new();
    [JsonInclude]
    public Cell league;
    [JsonInclude]
    public Cell ritual;
    [JsonInclude]
    public Cell flare_tnt;
    [JsonInclude]
    public Cell debuff;
    [JsonInclude]
    public Cell top { get; protected private set; }
    [JsonInclude]
    public Cell left { get; protected private set; }
    [JsonInclude]
    public Cell bott { get; protected private set; }
    [JsonInclude]
    public Cell right { get; protected private set; }
    [JsonInclude]
    public Cell buffs { get; protected private set; }
    [JsonInclude]
    public Cell party { get; protected private set; }
    [JsonInclude]
    public Cell chat { get; protected private set; }
    [JsonInclude]
    public Cell chat_help { get; protected private set; }
    [JsonInclude]
    public Cell menu { get; protected private set; }
    [JsonInclude]
    public Cell flask { get; protected private set; }
    [JsonInclude]
    public Cell xp_bar { get; protected private set; }
    [JsonInclude]
    public Cell left_panel { get; protected private set; }
    [JsonInclude]
    public Cell right_panel { get; protected private set; }
    /// <summary>
    /// passive point available
    /// </summary>
    [JsonInclude]
    public Cell ppa;
    [JsonInclude]
    public Cell skills { get; protected private set; } //3E0
    protected V2 my_sp;
    Thread worker;
    List<Element> need_init_well;
    bool b_init = false;
    int id = 0;
    public SafeScreen() : base("SafeScreen") {
        MakePoints();
        var center_updater = new Thread(() => {
            while (true) {
                if (!b_init) {
                    Init();//only ones here
                    Thread.Sleep(500);
                    continue;
                }
                foreach (var p in CentrPoints) {
                    Centr[p.Item1] = new Cell(ui.me.gpos.Increase(p.Item2),
                        ui.me.gpos.Increase(p.Item3)) { b_block = true };
                    id++;
                }
                foreach (var p in RoutPoints) {
                    if (p.Item1 == null)
                        continue;
                    Rounts[p.Item1] = new Cell(ui.me.gpos.Increase(p.Item2),
                        ui.me.gpos.Increase(p.Item3)) { b_block = false };
                    id++;
                }
                Thread.Sleep(1000 / 60);
            }
        });
        center_updater.IsBackground = true;
        center_updater.Start();
        
        worker = new Thread(() => {
            while (ui.b_running) {
                if (!b_init) {//dont init here
                    Thread.Sleep(500);
                    continue;
                }
                if (ui.curr_state != gState.InGameState) {
                    Thread.Sleep(500);
                    continue;
                }
                UpdateFrames();

                Blocks["top"] = top;
                Blocks["left"] = left;
                Blocks["bott"] = bott;
                Blocks["right"] = right;
                Blocks["buffs"] = buffs;
                Blocks["party"] = party;
                Blocks["chat"] = chat;
                Blocks["chat_help"] = chat_help;
                Blocks["menu"] = menu;
                Blocks["flask"] = flask;
                Blocks["xp_bar"] = xp_bar;
                Blocks["skills"] = skills;
                Blocks["ppa"] = ppa;
                //Blocks["league"] = league;
                //Blocks["ritual"] = ritual;
                Blocks["debuff"] = debuff;
                Blocks["flare_tnt"] = flare_tnt;
                Blocks["left_panel"] = left_panel;
                Blocks["right_panel"] = right_panel;
                Thread.Sleep(150);
            }
        });
        worker.IsBackground = true;
        worker.Start();
    }
    void MakePoints() {
        var wgs = ui.worldToGridScale;
        //CentrPoints.Add(("centre", new V2(27, 26) *wgs,  new V2(130, 80) *wgs));
        var x = 1.5f; var y = 0f;
        CentrPoints.Add(("C", new V2(x, y), new V2(x + 3f, y + 1.1f)));
        x = 1; y += 1.1f;
        CentrPoints.Add(("1", new V2(x, y), new V2(x + 3.6f, y + 1.5f)));
        x = 1; y += 1.5f;
        CentrPoints.Add(("2", new V2(x, y), new V2(x + 3.3f, y + 1.5f)));

        x = 0.5f; y = 0f;
        RoutPoints.Add(("0", new V2(x, y), new V2(x + 1f, y + 1f)));
        x = 0.5f; y -= 1f;
        RoutPoints.Add(("1", new V2(x, y), new V2(x + 1f, y + 1f)));
        for (int i = 0; i < 4; i++) {
            x += 1f;
            RoutPoints.Add(("r" + i, new V2(x, y), new V2(x + 1f, y + 1f)));
        }

        y += 1f;
        RoutPoints.Add(("t1", new V2(x, y), new V2(x + 1f, y + 1.2f)));
        y += 1.2f;
        RoutPoints.Add(("t2", new V2(x, y), new V2(x + 1f, y + 1.3f)));
        y += 1.5f; x -= 0.3f;
        RoutPoints.Add(("t3", new V2(x, y), new V2(x + 1f, y + 1.5f)));
        y += 1.5f;
        RoutPoints.Add(("t4", new V2(x, y), new V2(x + 1f, y + 1f)));
        for (int i = 0; i < 4; i++) {
            x -= 1.0f;
            RoutPoints.Add(("l" + i, new V2(x, y), new V2(x + 1f, y + 1f)));
        }
        x -= 0.2f; y -= 1.0f;
        RoutPoints.Add(("b", new V2(x, y), new V2(x + 1f, y + 1f)));
        for (int i = 0; i < 3; i++) {
            y -= 1.0f;
            RoutPoints.Add(("b" + i, new V2(x, y), new V2(x + 1f, y + 1f)));
        }
    }
    List<string> need_elem_nams = new() { "gui.ui_passive_point", "gui.chat_box_elem?.arrows",
        "gui.ui_flask_root", "gui.party_panel" };
    /// <summary>
    /// need w8 ui.gui init for init same frame here
    /// </summary>
    void Init() {
        int ni = 0;
        if (ui.gui == null || !ui.gui.IsValid)
            return;
        Width = ui.game_window_rect.Width;
        Height = ui.game_window_rect.Height;
        top = new Cell(0, 0, Width, 10);
        left = new Cell(0, 0, 10, Height);
        bott = new Cell(0, Height - 14, Width, Height);
        right = new Cell(Width - 16, 0, Width, Height);
        left_panel = new Cell(0, 0, 0, 0);
        right_panel = new Cell(0, 0, 0, 0);
        need_init_well = new() { ui.gui.ui_ppa, ui.gui.chat_box_elem?.arrows,
            ui.gui.ui_flask_root, ui.gui.party_panel };
        foreach (var e in need_init_well) {
            if (e == null || !e.IsValid) {
                ui.AddToLog("SafeScreen init err for " + need_elem_nams[ni], MessType.Warning);
                return;
            }
            ni++;
        }

        var chhe = ui.gui.ChatHelpPop.get_client_rectangle();
        chat_help = new Cell(chhe.X, chhe.Y, chhe.X + chhe.Width, chhe.Y + chhe.Height);

        var mbn = ui.gui.ui_menu_btn.get_client_rectangle();
        menu = new Cell(mbn.X, mbn.Y, mbn.X + mbn.Width, mbn.Y + mbn.Height);

        var flask_elem = ui.gui.ui_flask_root[0];
        var flr = new RectangleF();
        for (int i = 1; i < flask_elem.chld_count; i++) {
            if (i == 1)
                flr = flask_elem[i].get_client_rectangle();
            else
                flr = RectangleF.Union(flr, flask_elem[i].get_client_rectangle());
        }
        flr.Inflate(3, 3);
        flask = new Cell(flr.X, flr.Y, flr.X + flr.Width, flr.Y + flr.Height);
        var xp = ui.gui.ui_xp_bar.get_client_rectangle();
        xp_bar = new Cell(xp.X, xp.Y, xp.X + xp.Width, xp.Y + xp.Height);
        var sb = ui.gui.SkillBar.get_client_rectangle();
        sb.Inflate(10, 3);
        skills = new Cell(sb.X, sb.Y, sb.X + sb.Width, sb.Y + sb.Height);

        b_init = true;

    }
  
    int last_party_count = 0;
    protected void UpdateFrames() {
        var curr_buff = ui.gui.MyBuffPanel;//.children[6];
        if (buffs == null || curr_buff.Position != buffs.a || curr_buff.Width != buffs.width) {
            var brect = curr_buff.get_client_rectangle();
            buffs = new Cell(brect.X, brect.Y, brect.X + brect.Width, brect.Y + brect.Height);
        }

        var party_elem = ui.gui.party_panel;
        if (party_elem.members != null && party_elem.members.Count != last_party_count) {
            last_party_count = party_elem.members.Count;
            var face = new RectangleF();
            for (int i = 0; i < party_elem.members.Count; i++) {
                var p = party_elem.members[i];
                if (p.face_icon == null) {
                    ui.AddToLog(tname+ ".UpdateFrames face_icon==null", MessType.Error);
                    continue;
                }
                if (i == 0)
                    face = p.face_icon.get_client_rectangle();
                else
                    face = RectangleF.Union(face, p.face_icon.get_client_rectangle());
            }
            party = new Cell(face.X, face.Y, face.X + face.Width, face.Y + face.Height);
        }
        //CheckSameElement(ui.gui.league, ref ritual);
        //CheckSameElement(ui.gui.ui_ritual_rewards,ref ritual);
        CheckSameElement(ui.gui.debuffs_pannel, ref debuff);
        CheckSameElement(ui.gui.ui_ppa, ref ppa);

        if (ui.gui.chat_box_elem.arrows != null) {
            var chs = ui.gui.chat_box_elem.arrows.get_client_rectangle();
            if (chat == null || chat.a.X != chs.X)
                chat = new Cell(chs.X, chs.Y, chs.X + chs.Width, chs.Y + chs.Height);
        }
        else {
            ui.AddToLog(tname + ".UpdateFrames chbe=>arrows is null", MessType.Error);
        }

        GetLeftRightPanels();
        GetFlares();
    }
    Cell zerro_cell = new Cell(0, 0, 0, 0);
    void GetLeftRightPanels() {
        if (ui.gui.open_left_panel.IsVisible) {
            var lpr = ui.gui.open_left_panel.get_client_rectangle();
            left_panel = new Cell(lpr.X, lpr.Y, lpr.X + lpr.Width, lpr.Y + lpr.Height);
        }
        else {
            left_panel.min =  left_panel.max = default;
        }
        if (ui.gui.open_right_panel.IsVisible) {
            var rpr = ui.gui.open_right_panel.get_client_rectangle();
            right_panel = new Cell(rpr.X, rpr.Y, rpr.X + rpr.Width, rpr.Y + rpr.Height);
        }
        else {
            right_panel.min = right_panel.max =default;
        }
    }
    bool b_last_flares_state;
    void GetFlares() {
        var flare = ui.gui.SkillBar.ui_flare_tnt;
        if (flare?.Width > 0) {
            if (!b_last_flares_state) {
                b_last_flares_state = true;
                var rect = ui.gui.SkillBar.ui_flare_tnt.get_client_rectangle();
                flare_tnt = new Cell(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            }

        }
        else {
            if (b_last_flares_state) {
                flare_tnt = new Cell(0, 0, 0, 0);
                b_last_flares_state = false;
            }
        }
    }
    
   
    void CheckSameElement(Element elem, ref Cell cell) {
        if (elem.IsVisible) {
            var rect = elem.get_client_rectangle();
            var min = new V2(rect.X, rect.Y);
            var max = new V2(rect.X + rect.Width, rect.Y + rect.Height);
            if (cell == null || cell.min != min ||  cell.max!=max)
                cell = new Cell(min, max);
        }
        else {
            cell ??= zerro_cell;
            cell.min = default;
            cell.max = default;
        }
    }

    public bool b_gp_Inside_centr(V2 gp) {
        foreach (var b in Centr) {
            if (b.Value.Insade(gp)) {
                return true;
            }
        }
        return false;
    }
    public bool SetMouseBestCentrSP(V2 gp, aTask from) {
        ui.AddToLog("SetMouseBestCentrSP not implemented", MessType.Error);
        //var sorted = Rounts.OrderBy(r => r.Value.center.GetDistance(ui.me.gpos)).ToArray();
        //foreach (var b in sorted) {
        //    if (ui.nav.b_can_hit(b.Value.center)) {
        //        gp = b.Value.center;
        //        var sp = ui.TgpToSP(gp);
        //        Mouse.SetCursor(sp, "SetMouseBestCentrSP", 3, false);
        //        return true;
        //    }
        //}
        //from.b_debug = true;
        //ui.tasker.Hold("SetMouseBestCentrSP"); //prevent server disconnect
        //ui.sound_player.PlaySound(@"I hit a wall, my God.mp3"); //@"C:\Sounds\Help me.mp3"
        return false;
    }
}
