namespace Stas.GA;
public class Targetable : EntComp {
    public Targetable(IntPtr address) : base(address) {
    }
    TargetableComponentOffsets data;
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        data = ui.m.Read<TargetableComponentOffsets>(this.Address);
    }

    public bool isTargetable => data.isTargetable;
    public bool isHighlightable => data.isHighlightable;
    public bool isTargeted => data.isTargeted;

    protected override void Clear() {

    }
}