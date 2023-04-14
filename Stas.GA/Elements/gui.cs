using System.Diagnostics;
using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

/// <summary>
///     This is actually UiRoot main child which contains all the UiElements (100+).
///      Normally it's at index 1 of UiRoot.
/// this need be updated after reenter to game from select hero/login
/// </summary>
public partial class GameUiElements : Element {

    Thread worker;
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetGuiOffsets")]
    public static extern int GetGuiOffsets(IntPtr gui_ptr, ref guiOffset offs);
    internal guiOffset data = new guiOffset();
    bool need_check_was_init=false;
    internal GameUiElements() : base(default, "gui" ) {
        worker = new Thread(() => {
            while (ui.b_running) { //0x000001dd1e44b6c0
                if (Address == default 
                || ui.curr_state != gState.InGameState 
                || ui.sett.b_league_start) {
                    Thread.Sleep(ui.w8 * 10);
                    continue;
                }
                GetGuiOffsets(Address, ref data);
                Debug.Assert(data.map_root_ptr != default);//alt+f4 on poe window = frbug here
                if (data.map_root_ptr == default) {
                    ui.AddToLog(tName + "data.map_root_ptr = def\n" +
                        " was the client forcibly disconnected?", MessType.Critical);
                    return;
                }
                need_check_vis["large_map"] = large_map;
                need_check_vis["map_devise"] = map_devise;
                need_check_vis["KiracMission"] = KiracMission;
                need_check_vis["open_left_panel"] = open_left_panel;
                need_check_vis["open_right_panel"] = open_right_panel;
                need_check_vis["passive_tree"] = passive_tree;
                need_check_vis["NpcDialog"] = NpcDialog;
                need_check_vis["LeagueNpcDialog"] = LeagueNpcDialog;
                need_check_vis["BetrayalWindow"] = BetrayalWindow;
                need_check_vis["AtlasPanel"] = AtlasPanel;
                need_check_vis["AtlasSkillPanel"] = AtlasSkillPanel;
                need_check_vis["DelveWindow"] = DelveWindow;
                need_check_vis["TempleOfAtzoatl"] = TempleOfAtzoatl;
                need_check_was_init = true;
                Thread.Sleep(100);
            }
        });
        worker.IsBackground = true;
        worker.Start();
    }
    void FindlargeMap(guiOffset data) {
        var elem_found = new Dictionary<string, Element>();
        var m_ptr = data.map_root_ptr;
        for (int i = 0; i < 8000; i += 8) {
            var ne = new Element(ui.m.Read<IntPtr>(m_ptr + i), "Test" + i);
            if (ne.IsValid && ne.Address == map_root.children[0].Address)
                elem_found.Add(i.ToString("X"), ne);
        }
    }

    internal Element ExpedPlacement => new  Element(data.ExpedPlacement, "ExpedPlacement");
    internal Element options_elem  => new (data.OptionPanel, "options");
    public EscDialog esc_dialog { //find 'exit to character selection'
        get {
            var start = ui.base_offsets[PattNams.GameStates];
            var addr = ui.m.Read<nint>(start, "esc_dialog", 0x78, 0x38, 0xC0);
            var res = new EscDialog(addr);
            return res.IsValid ? res : null;
        }
    }
    internal Element skip_element => new Element(data.skip, "skip_element");
    internal UltimatumElem ultimatum =>  new UltimatumElem(data.UltimatumProgressPanel);
    internal DelveDarknessElem delve_darkness_elem => new DelveDarknessElem(data.ui_debuf_panell);
    internal ModalDialog modal_dialog => new ModalDialog(data.ModalDialog) ;
    internal IncomingUserRequest incomin_user_request => new IncomingUserRequest(data.incomin_user_request);
    internal ChatBoxElem chat_box_elem => new ChatBoxElem(data.ChatPanel);
    internal SkillBarElement SkillBar => new SkillBarElement(data.ui_skills);
    internal PartyPanel party_panel => new PartyPanel(data.party_panel);
    internal Element player_inventory  => new Element(IntPtr.Zero, "player_inventory");
    internal Element map_root => 
        new Element(data.map_root_ptr, "map_root_ptr");
    internal Element large_map {
        get {
            if (map_root == null || map_root.chld_count != 4)
                return null;
            else {
                var res = map_root.children[0];
                res.Set_tname("Map");
                return res;
            }
        }
    }
    internal StashElement stash_element => new (data.StashElement);
    internal QuestRewardWindow QuestRewardWindow => new QuestRewardWindow(data.QuestRewardWindow);
    internal WorldMapElement world_map  => new WorldMapElement(data.WorldMap);
    internal If_I_Dead if_I_dead  => new If_I_Dead(data.if_i_dead);
    internal Element ui_ritual_rewards  => new Element(data.RitualWindow, "ui_ritual_rewards");
    internal Element debuffs_pannel => new Element(data.ui_debuf_panell, "debuffs_pannel");
    internal Element ui_ppa => new Element(data.ui_passive_point_available, "ui_ppa");
    internal Element ChatHelpPop  => new Element(data.chat_help_pop, "ChatHelpPop");
    internal Element ui_menu_btn => new Element(data.ui_menu_btn, "ui_menu_btn");
    internal Element ui_flask_root => new Element(data.ui_flask_root, "ui_flask_root") ;
    internal Element ui_xp_bar => new Element(data.ui_xp_bar, "ui_xp_bar") ;
    internal Element MyBuffPanel => new Element(data.ui_buff_panel, "MyBuffPanel");
}
