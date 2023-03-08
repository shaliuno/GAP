namespace Stas.GA;

public class PartyPanel : Element {
    internal PartyPanel() : base("PartyPanel") {
        b_dynamic_childrens = true;
    }
    internal override void Tick(nint ptr, string from) {
        base.Tick(ptr, from);
        members_root = GetChildFromIndices(0, 0);
        if (members_root != null) {
            members_root.GetChildren("Tick from children", true);
            _membs.Clear();
            foreach (var chld in members_root.children) {
                var nm = new PartyMember(chld.Address);
                _membs.Add(nm);
            }
        }
    }
    public Element members_root { get; private set; } = new Element("memb_liat_root");
    List<PartyMember> _membs = new ();
    public List<PartyMember> members => _membs;
}

public class PartyMember : Element {
    public string name { get; private set; }
    public string area_name { get; private set; }
    public Element face_icon { get; private set; }
    public Element portal_icon { get; private set; }

    internal PartyMember(IntPtr address) : base(address, "PartyMember") {
        b_dynamic_childrens = true;
        _tname = "PartyMember";
        if (address != default)
            Tick(address, tName + "()");
    }
    internal override void Tick(nint ptr, string from) {
        base.Tick(ptr, from);
        this.name = GetChildAtIndex(0)?.Text;
        face_icon = GetChildAtIndex(1);
        if (children_pointers.Length == 4) {
            area_name = GetChildAtIndex(2)?.Text;
            portal_icon = GetChildAtIndex(3);
        }
        else { //same location with master
            area_name = null;
            portal_icon = null;//dont use
        }
    }
    public override string ToString() {
        var a = "";
        if (area_name != null)
            a = "[" + area_name + "]";
        return name + a;
    }
}
