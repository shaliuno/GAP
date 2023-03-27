using Stas.GA;

public abstract class EntComp : RemoteObjectBase {
    public EntComp(IntPtr address, string tname = null) : base(address, tname) {
    }
    IntPtr _optr = IntPtr.Zero;
    /// <summary>
    /// ovner entity ptr
    /// </summary>
    public IntPtr owner_ptr {
        get {
            if (_optr == IntPtr.Zero) {
                _optr = ui.m.Read<IntPtr>(Address + 8);//entity ptr
            }
            return _optr;
        }
    }
    public NetworkObject GetNO(string from) {
        if (this.owner_ptr == default)
            return null;
        return new NetworkObject(new IntPtr(base.Address + 8L));
    }
    DateTime last_meta_dt = DateTime.MinValue;
    string _md = null;
    public string metadata {
        get {
            //we use datatime here for not reread if ReadStdWString returtn null first time
            if (last_meta_dt == DateTime.MinValue) {
                //var ent = new Entity(owner_ptr); //<-for debug only dont uncoment it will throw overflow if not using it only ones
                //var _optr = ui.m.Read<IntPtr>(Address + 8);//entity ptr
                var m_ptr = ui.m.Read<IntPtr>(this.owner_ptr + 8);
                _md = ui.m.ReadStdWString(ui.m.Read<StdWString>(m_ptr + 8L));
                last_meta_dt = DateTime.Now;
            }
            return _md;
        }
    }
    protected override void Clear() {
        _md = null;
        last_meta_dt = DateTime.MinValue;
        _optr = IntPtr.Zero;
    }
}
