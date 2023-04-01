namespace Stas.GA;

public class ItemMod : RemoteObjectBase {
    public ItemMod(IntPtr ptr) : base(ptr) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
    }
    protected override void Clear() {
        Address = default;
    }
    string _DisplayName;
    string _Group;
    int _Level;
    string _Name;
    string _RawName;
    public int Value1 => ui.m.Read<int>(Address,tName, 0);
    public int Value2 => ui.m.Read<int>(Address, tName, 4);
    public int Value3 => ui.m.Read<int>(Address, tName, 8);
    public int Value4 => ui.m.Read<int>(Address, tName, 0xC);

    public string RawName {
        get {
            if (_RawName == null)
                ParseName();

            return _RawName;
        }
    }

    public string Group {
        get {
            if (_Group == null)
                ParseName();

            return _Group;
        }
    }

    public string Name {
        get {
            if (_RawName == null)
                ParseName();

            return _Name;
        }
    }

    public string DisplayName {
        get {
            if (_RawName == null)
                ParseName();

            return _DisplayName;
        }
    }

    public int Level {
        get {
            if (_RawName == null)
                ParseName();

            return _Level;
        }
    }

    private void ParseName() {
        var addr = ui.m.Read<long>(Address + 0x18, tName, 0);
        _RawName = ui.string_cashe.Read((nint)addr, () => ui.m.ReadStringU(addr));

        _DisplayName = ui.string_cashe.Read((nint)(addr + (_RawName.Length + 2) * 2),
            () => ui.m.ReadStringU(addr + (_RawName.Length + 2) * 2));

        _Name = _RawName.Replace("_", ""); // Master Crafted mod can have underscore on the end, need to ignore
        _Group = ui.string_cashe.Read(Address + 0x18,
            () => ui.m.ReadStringU(ui.m.Read<long>(Address + 0x18, tName, 0x70)));
        var ixDigits = _Name.IndexOfAny("0123456789".ToCharArray());

        if (ixDigits < 0 || !int.TryParse(_Name.Substring(ixDigits), out _Level))
            _Level = 1;
        else
            _Name = _Name.Substring(0, ixDigits);
    }

    public override string ToString() {
        return $"{Name} ({Value1}, {Value2}, {Value3}, {Value4})";
    }


}