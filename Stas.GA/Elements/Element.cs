using System.Text;
using System.Numerics;
using System.Drawing;
namespace Stas.GA;

public partial class Element : ObjectBase {
    public Element(nint ptr, string name = null) : base(ptr, name) {
        _cacheElement = new FrameCache<ElemOffsets>(() => Address == default ? default : ui.m.Read<ElemOffsets>(Address));
        _cacheElementIsVisibleLocal = new FrameCache<bool>(() => Address != default && ui.m.Read<bool>(Address + ui.IsVisibleLocalOffs));
    }

    public bool b_can_click {
        get {
            var dict = ui.w8ting_click_until;
            if (dict.ContainsKey(Address) && dict[Address] > DateTime.Now)
                return false;
            return true;
        }
    }
    public bool b_selected_tab => ui.m.Read<Byte>(Address + 0x230) == 1;
    public Element Tooltip2 => Address == default ? null : new Element(ui.m.Read<IntPtr>(Address + 0x418));
    public bool b_label_mouse_over => ui.m.Read<ushort>(Address + 0x90) == 0x4CF0;
    public bool b_mouse_over => ui.m.Read<byte>(Address + 0x1F3) == 1;
    public bool b_selected => ui.m.Read<byte>(Address + 0x110) == 0x81;
    public bool b_link_selected => TextBoxOverlayColor == ((uint)0xFFFFFFF).ToColor();
    public bool b_goodbye_selected => ui.m.Read<uint>(Address + 0x1C8) == 0xFFFFCA96; //0x2B4 3.17

    //public bool b_link_seleced { get {
    //        var color = M.Read<uint>(Address + 0x188);
    //        return color == (uint)0xFFFFFFF9; //.ToString("X")== "FFFFFFF9";
    //    } } 
    public Color link_color => Color.FromArgb((int)offs.link_color); //ColorBGRA
    public Element GetElemByLink(NpcLinks link_enum) {
        return GetTextElem_by_Str(link_enum.ToString());
    }
    public Element GetElem_ends_wit(string str, bool ignore_case = true) {
        if (ignore_case) {
            if (Text != null && Text.ToLower().EndsWith(str.ToLower())) {
                return this;
            }
        }
        else {
            if (Text != null && Text.EndsWith(str)) {
                return this;
            }
        }
        foreach (var ch in children) {
            var element = ch.GetElem_ends_wit(str);
            if (element != null) return element;
        }
        return null;
    }
    //public void GetElems_by_str(string str, List<Element> res) {
    //    if(Text?.ToLower() == str.ToLower()) {
    //        res.Add(this);
    //        DebugWindow.AddToLog("GetElemsByStr=[" + res.Count + "]");
    //    }
    //    foreach(var ch in Children) {
    //        ch.GetElems_by_str(str, res);
    //    }
    //}
    public Element GetTextElem_with_Str(string str, bool only_vis = true) {
        if (only_vis && !this.IsVisible)
            return null;
        if (Text != null && Text.ToLower().Contains(str.ToLower())) {
            return this;
        }
        foreach (var ch in children) {
            var element = ch.GetTextElem_with_Str(str);
            if (element != null) return element;
        }
        return null;
    }
    public void GetAllTextElem_by_Str(string str, List<Element> res) {
        if (Text?.ToLower() == str.ToLower()) {
            res.Add(this);
        }
        foreach (var ch in children)
            ch.GetAllTextElem_by_Str(str, res);
    }
    public Element GetTextElem_by_Str(string str, bool ignore_case = true) {
        if (ignore_case) {
            if (Text?.ToLower() == str.ToLower()) {
                return this;
            }
        }
        else {
            if (Text == str) {
                return this;
            }
        }
        foreach (var ch in children) {
            var element = ch.GetTextElem_by_Str(str);
            if (element != null) return element;
        }
        return null;
    }

    public const int OffsetBuffers = 0;

    // dd id
    // dd (something zero)
    // 16 dup <128-bytes structure>
    // then the rest is
    readonly CachedValue<ElemOffsets> _cacheElement;
    readonly CachedValue<bool> _cacheElementIsVisibleLocal;
    readonly List<Element> _childrens = new List<Element>();
    CachedValue<RectangleF> _getClientRect;
    Element _parent;
    long childHashCache;
    public ElemOffsets offs => _cacheElement.Value;
    public bool IsValid =>  offs.SelfPointer == Address && Address > 0;
    public long chld_count {
        get {
            if (!IsValid)
                return 0;
            if (offs.chld_ptr.Last != default && offs.chld_ptr.First != default)
                return (offs.chld_ptr.Last - offs.chld_ptr.First) / 8;
            return 0;
        }
    }
    public bool IsVisibleLocal => (offs.IsVisibleLocal & 8) == 8;// ==(byte) 0x2E;  //0x26 is hidden
    public Element Root => ui.states.ingame_state.ui_root;
    public Element Parent => offs.parent_ptr == default ? null : (_parent ??= new Element(offs.parent_ptr));
    public Vector2 Position => offs.Position;
    public float X => offs.X;
    public float Y => offs.Y;
    public Element Tooltip => Address == default ? null : new Element(offs.Tooltip);
    public float Scale => offs.Scale;
    public float Width => offs.Width;
    public float Height => offs.Height;
    public bool IsHighlighted => offs.isHighlighted;
    [Obsolete("Element.isHighlighted is deprecated. Use IsHighlighted.", false)]
    public bool isHighlighted => IsHighlighted;
    public Color BorderColor => Color.FromArgb((int)offs.ElementBorderColor);
    public Color BackgroundColor => Color.FromArgb((int)offs.ElementBackgroundColor);
    public Color OverlayColor => Color.FromArgb((int)offs.ElementOverlayColor);
    public Color TextBoxBorderColor => Color.FromArgb((int)offs.TextBoxBorderColor);
    public Color TextBoxBackgroundColor => Color.FromArgb((int)offs.TextBoxBackgroundColor);
    public Color TextBoxOverlayColor => Color.FromArgb((int)offs.TextBoxOverlayColor);

    public Color HighlightBorderColor => Color.FromArgb((int)offs.HighlightBorderColor);

    public virtual string Text {
        get {
            var offs = Address + ui.elem_text_offs;
            return ui.string_cashe.Read(offs, () => {
                var length = ui.m.Read<long>(offs + 0x10);
                var Capacity = (int)ui.m.Read<long>(offs + 0x18);
                var addr = Capacity < 8 ? offs : ui.m.Read<long>(offs);

                if (addr <= 0 || length > 5120 || length <= 0)
                    return "";
                else
                    return Sanitize(ui.m.ReadStringU(addr, (int)length * 2));
            });
        }
    }

    private static string Sanitize(string text) {
        return !string.IsNullOrWhiteSpace(text) ? text.Replace("\u00A0\u00A0\u00A0\u00A0", "{{icon}}") : string.Empty;
    }

    public bool IsVisible {
        get {
            if (Address >= 1770350607106052 || Address <= 0) return false;
            return IsVisibleLocal && GetParentChain().All(current => current.IsVisibleLocal);
        }
    }

    public IList<Element> children => GetChildren<Element>();
    public long ChildHash => offs.chld_ptr.GetHashCode();
    public RectangleF GetClientRectCache =>
        _getClientRect?.Value ?? (_getClientRect = new TimeCache<RectangleF>(get_client_rectangle, 200)).Value;
    public Element this[int index] => GetChildAtIndex(index);

    public int? IndexInParent => Parent?.children.IndexOf(this);

    public string PathFromRoot {
        get {
            var parentChain = GetParentChain();
            if (parentChain.Count != 0) {
                parentChain.RemoveAt(parentChain.Count - 1);
                parentChain.Reverse();
            }
            parentChain.Add(this);
            var properties = (from property in ui.gui.GetType().GetProperties()
                              where typeof(Element).IsAssignableFrom(property.PropertyType)
                              where property.GetIndexParameters().Length == 0
                              let value = property.GetValue(ui.gui) as Element
                              where value?.Address == parentChain.First().Address
                              select property.Name).ToList();

            return (properties.Count > 0 ? $"({properties.First()})" : "") + string.Join("->", parentChain.Select(x => x.IndexInParent));
        }
    }

    protected List<Element> GetChildren<T>() where T : Element {
        if (Address == default || offs.chld_ptr.First == 0 || offs.chld_ptr.Last == 0 || chld_count < 0)
            return _childrens;

        if (ChildHash == childHashCache)
            return _childrens;

        var pointers = ui.m.ReadPointersArray(offs.chld_ptr.First, offs.chld_ptr.Last);
        //var pointers2 = M.ReadStdVector<long>(e.Childs);
        //for (int i=0; i < pointers.Count; i++) {
        //    Debug.Assert(pointers[i] == pointers2[i]);
        //}

        if (pointers.Count != chld_count)
            return _childrens;

        _childrens.Clear();
        foreach (var point in pointers) {
            _childrens.Add(new Element(new nint(point)));
        }
        childHashCache = ChildHash;
        return _childrens;
    }

  
    private List<Element> GetParentChain() {
        var list = new List<Element>();

        if (Address == default)
            return list;

        var hashSet = new HashSet<Element>();
        var root = Root;
        var parent = Parent;

        if (root == null)
            return list;

        while (parent != null && !hashSet.Contains(parent) && root.Address != parent.Address && parent.Address != 0) {
            list.Add(parent);
            hashSet.Add(parent);
            parent = parent.Parent;
        }

        return list;
    }

    public Vector2 GetParentPos() {
        float num = 0;
        float num2 = 0;
        var rootScale = ui.states.ingame_state.ui_root.Scale;

        foreach (var current in GetParentChain()) {
            num += current.X * current.Scale / rootScale;
            num2 += current.Y * current.Scale / rootScale;
        }

        return new Vector2(num, num2);
    }
    /// <summary>
    /// Must be corrected for current window position and game scale like: this result / ui.screen_k + ui.w_offs
    /// </summary>
    /// <returns></returns>
    public virtual RectangleF get_client_rectangle() {

        if (Address == default) return RectangleF.Empty;
        var vPos = GetParentPos();
        float width = ui.camera.Width;
        float height = ui.camera.Height;
        var ratioFixMult = width / height / 1.6f;
        var xScale = width / 2560f / ratioFixMult;
        var yScale = height / 1600f;

        var rootScale = ui.states.ingame_state.ui_root.Scale;
        var num = (vPos.X + X * Scale / rootScale) * xScale;
        var num2 = (vPos.Y + Y * Scale / rootScale) * yScale;
        return new RectangleF(num, num2, xScale * Width * Scale / rootScale, yScale * Height * Scale / rootScale);

    }
    //public RectangleF GetClientRrctangle=> get_client_rectangle_local / ui.screen_k + ui.w_offs

    public Element GetChildFromIndices(params int[] indices) {
        var currentElement = this;

        StringBuilder BuildErrorString(int errorIndex) {
            var str = new StringBuilder();
            foreach (var i in indices) {
                if (i == errorIndex) {
                    str.Append('>');
                }

                str.AppendFormat("[{0}] ", i);
                if (i == errorIndex) {
                    str.Append('<');
                }
            }

            return str;
        }

        for (var indexNumber = 0; indexNumber < indices.Length; indexNumber++) {
            var index = indices[indexNumber];
            currentElement = currentElement.GetChildAtIndex(index);

            if (currentElement == null) {
                ui.AddToLog($"{nameof(Element)} with index {index} was not found. Indices: {BuildErrorString(indexNumber)}", MessType.Error);
                return null;
            }

            if (currentElement.Address == default) {
                ui.AddToLog($"{nameof(Element)} with index {index} has address = 0. Indices: {BuildErrorString(indexNumber)}");
                return new Element(default, "error");
            }
        }

        return currentElement;
    }

    public Element GetChildAtIndex(int index) {
        return index >= chld_count ? null : new Element(ui.m.Read<nint>(Address + ui.first_children_offset, "GetChildAtIndex", index * 8));
    }
    public void GetAllStrings(List<string> res) {
        if (Text?.Length > 0) {
            res.Add(Text);
        }
        foreach (var ch in children)
            ch.GetAllStrings(res);
    }

    public override string ToString() {
        return tName + " valid=" + IsValid + " vis=" + IsVisible;
    }
}