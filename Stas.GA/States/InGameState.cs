using System.Diagnostics;
namespace Stas.GA;

/// <summary>
///    [2] ui.states.ingame_state 
/// </summary>
public class InGameState : RemoteObjectBase {
    internal InGameState(IntPtr address) : base(address) {

    }
    InGameStateOffset data;
    internal override void Tick(IntPtr ptr, string from) {
        Address = ptr;
        if (Address == IntPtr.Zero) {
            Clear();
            return;
        }
        data = ui.m.Read<InGameStateOffset>(Address);
        //for debug info only
        var gst = (gState)ui.m.Read<byte>(Address + 0x0B);
        world_data.Tick(data.WorldData);
        area_instance.Tick(data.AreaInstanceData, tName);
        gui.Update(data.IngameUi, tName + ".Tick");
        ui_root.Update(data.UiRootPtr, tName + ".Tick");
        UIHover.Update(data.UIHover, tName + ".Tick");
    }
    protected override void Clear() {
        //TODO debug where and when it is called from!
        world_data.Tick(IntPtr.Zero);
        area_instance.Tick(IntPtr.Zero, tName);
        ui_root.Update(IntPtr.Zero, tName + ".CleanUpData");
        gui.Update(IntPtr.Zero, tName + ".CleanUpData");
    }
    void FindUiRoot() {
        var elems = new Dictionary<Element, string>();
        for (int i = 0; i < 8000; i += 8) {
            var elem = new Element(ui.m.Read<IntPtr>(Address + i));
            if (elem.IsValid) { //&& elem.children_pointers.Length == 2
                elems.Add(elem, i.ToString("X") + "[" + elem.children.Count + "]");
            }
            ui.AddToLog("test=[" + i + "]");
            Thread.Sleep(1);
        }
    }

    public WorldData world_data { get; } = new(IntPtr.Zero);
    public AreaInstance area_instance { get; } = new(default);
    public Element UIHover = new Element(default, "UIHover");
    internal Element ui_root = new Element(default, "UiRoot");
    public GameUiElements gui = new GameUiElements();

}