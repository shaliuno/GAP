using System.Text;
using Vortice.Direct3D11;

namespace Stas.GA;

public partial class GameUiElements : Element {
    //all of this element can be found by ui.test.GetRootElemUnderCursor(0);
    public Element map_devise => new Element(data.MapDeviceWindow, "map_devise");
    public Element KiracMission => new Element(data.KiracMission, "KiracMission");
    public Element open_right_panel => new Element(data.open_right_panel, "OpenRightPanel");
    public Element open_left_panel => new Element(data.open_left_panel, "OpenLeftPanel");
    public NpcDialog NpcDialog => new NpcDialog(data.NpcDialog);
    public Element LeagueNpcDialog => new Element(data.LeagueNpcDialog, "LeagueNpcDialog");
    public Element BetrayalWindow => new Element(data.BetrayalWindow, "BetrayalWindow"); 
    public Element AtlasSkillPanel => new Element(data.AtlasSkillPanel, "AtlasSkillPanel");
    public Element DelveWindow => new Element(data.DelveWindow, "DelveWindow");
    public Element TempleOfAtzoatl => new Element(data.TempleOfAtzoatl, "TempleOfAtzoatl");
    public Element AtlasPanel => new Element(data.AtlasPanel, "AtlasPanel");
    internal Element passive_tree => new Element(data.passives_tree, "AtlasPanel");
    bool _bbi;
    public string b_busy_info { get; private set; }
    Dictionary<string, Element> need_check_vis = new();
    object locker = new object();
    StringBuilder sb= new StringBuilder();
    /// <summary>
    /// same time make debug string b_busy_info
    /// </summary>
    public bool b_busy {
        get {
            _bbi = false;
            if (!need_check_was_init)//dict making not done// first cold call
                return false;
            lock (locker) {// we need lock sb from multy thread reading
                sb.Clear();
                foreach (var e in need_check_vis.Values) {
                    if (e == null) {
                        sb.AppendLine("same safeElemem==null");
                        continue;
                    }
                    if ( e.IsValid) {
                        if (e.IsVisible) {
                            _bbi = true;
                            sb.AppendLine(e.tName + "=[true]");
                            break;
                        }
                        else
                            sb.AppendLine(e.tName + "=[false]");
                    }
                    else
                        sb.AppendLine(e.tName + " NOT valid");
                }
                b_busy_info = sb.ToString();
            }
            return _bbi;
        }
    }
   
}
