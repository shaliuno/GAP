namespace Stas.GA;

public class RenderItem : EntComp {
    public RenderItem(IntPtr address) : base(address, "RenderItem") {
    }

    internal override void Tick(nint ptr, string from = null) {
        if (_rp == null)
            _rp = ui.m.ReadNativeString(base.Address + 40L);
        ResourcePath = _rp;
    }
    string _rp = null;
    public string ResourcePath { get; private set; }

}