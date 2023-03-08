using ImGuiNET;
namespace Stas.GA; 

/// <summary>
///     The <see cref="Charges" /> component in the entity.
/// </summary>
public class Charges : EntComp {
    public Charges(IntPtr address) : base(address) {
        _tname = "Charges";
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<ChargesOffsets>(this.Address);
        this.Current = data.current;
        var data_ext = ui.m.Read<ChargesExt>(data.ext_ptr);
        ChargesPerUse = data_ext.ChargesPerUse; //M.Read<int>(Address + 0x10, 0x18)
        MaxBaseCharges = data_ext.MaxBaseCharges2; //M.Read<int>(Address + 0x10, 0x14) 
    }
    protected override void Clear() {
        ui.AddToLog("Component Address should never be Zero.", MessType.Warning);
    }
    /// <summary>
    ///     Gets a value indicating number of charges the flask has.
    /// </summary>
    public int Current { get; private set; }
    public int ChargesPerUse { get; private set; }
    public int MaxBaseCharges { get; private set; }
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Current Charges: {this.Current}");
    }
}