namespace Stas.GA;

public class PartyPanel :Element {
    public PartyPanel(nint ptr, string name = "PartyPanel") : base(ptr, name) {
    }
    //how to finde?
    //ui.test_elem = ui.gui.GetTextElemWithStr("ILya_arch").Parent.Parent.Parent.Parent;
    List<PartyMember> _membs = new List<PartyMember>();
    public Element memb_liat_root => GetChildFromIndices(0, 0);
    public List<PartyMember> members {
        get {
            _membs.Clear();
            var ma = memb_liat_root?.children;
            if (ma == null)
                return null;
            int i = 0;
            foreach(var m in ma) {
                var nm = new PartyMember(m.Address);
                nm.name = m.GetChildAtIndex(0)?.Text;
                nm.face_icon = m.GetChildAtIndex(1);
                nm.area_name = m.GetChildAtIndex(2)?.Text;
                nm.portal_icon = m.GetChildAtIndex(3);
                _membs.Add(nm);
                i++;
            }
            return _membs;
        }
    }
}
public class PartyMember :Element {
    public PartyMember(IntPtr ptr):base(ptr, "PartyMember") {
    }
    public string name;
    public string area_name;
    public Element face_icon;
    public Element portal_icon;
    public override string ToString() {
        var an = "";
        if(area_name != null)
            an = "[" + area_name + "]";
        return name + an ;
    }
}
/*
public class PartyElement :Element {
    List<PartyMemberIcon> _membs = new List<PartyMemberIcon>();
    public List<PartyMemberIcon> members { get {
            _membs.Clear();
            var x = X * Scale;
            var y = Y * Scale;
            var memb_elems = GetChildAtIndex(0)?.GetChildAtIndex(0)?.Children;
            if(memb_elems != null) {
                foreach(var m in memb_elems) {
                    var nm = new PartyMemberIcon() { root = m };
                    nm.name = m.GetChildAtIndex(0)?.Text;
                    var ei = m.GetChildAtIndex(2);
                    if(ei != null) {
                        if(m.ChildCount == 4) { // same location
                            ei = m.GetChildAtIndex(3);
                            nm.area = m.GetChildAtIndex(2)?.Text;
                        }
                        nm.icon_pos = new Vector2(x + m.X * m.Scale + ei.X * ei.Scale,
                            y + m.Y * m.Scale + ei.Y * ei.Scale);
                        nm.icon_size = new Vector2(ei.Width * ei.Scale, ei.Height * ei.Scale);
                        _membs.Add(nm);
                    }
                }
            }
            return _membs;
        }
    }
}
public class PartyMemberIcon {
    public Element root;
    public string name;
    public string area;
    public Vector2 icon_pos;
    public Vector2 icon_size;
    public override string ToString() {
        var a = "";
        if(area != null)
            a = "[" + area + "]";
        return name + a + " "+ icon_pos.ToIntString();
    }
}
*/

