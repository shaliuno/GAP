namespace Stas.GA; 

public class ArchnemesisMod : EntComp {
    public ArchnemesisMod(nint address) : base(address, "ArchnemesisMod") {
    }

    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
       
    }
    protected override void Clear() {
       
    }
}
