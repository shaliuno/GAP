namespace Stas.GA;
public class Targetable : EntComp {
    public Targetable(IntPtr ptr) : base(ptr) {
        _tname = "Targetable";
        if (ptr != default)
            Tick(ptr, tName + "()");
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<TargetableComponentOffsets>(this.Address);
        isTargetable = data.isTargetable;
        isHighlightable = data.isHighlightable;
        isTargeted = data.isTargeted;
    }
   
    public bool isTargetable { get; private set; }
    public bool isHighlightable { get; private set; }
    public bool isTargeted { get; private set; }

    protected override void Clear() {
        
    }
}