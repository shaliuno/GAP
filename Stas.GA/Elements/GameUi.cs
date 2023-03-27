namespace Stas.GA;
public class GameUi : Element {
    public GameUi(nint ptr, string name = "GameUi") : base(ptr, name ) {
    }
    public SkillBarElement skill_bar { get {
            var found = GetChildFromIndices(8, 2);
            if (found != null)
                return new SkillBarElement(found.Address);
            return null;
        } } 
    public Element UnusedPassivePointsButton => GetChildAtIndex(3);
    public int UnusedPassivePointsAmount => GetUnusedPassivePointsAmount();

    private int GetUnusedPassivePointsAmount() {
        var numberInButton = GetChildAtIndex(3)?.GetChildAtIndex(1);
        if (numberInButton == null || !numberInButton.IsVisible) {
            return 0;
        }
        int result;
        var success = int.TryParse(GetChildAtIndex(3)?.GetChildAtIndex(1)?.Text, out result);
        return success ? result : 0;
    }
}
public class SkillBarElement : Element {
    public SkillBarElement(nint ptr) : base(ptr, "SkillBarElement") {
    }

    /// <summary>
    /// its always is visible, but have Width=0 if not visible in game
    /// </summary>
    public Element ui_flare_tnt => GetChildFromIndices((int)chld_count - 1, 0);

    public new SkillElement this[int k] => new SkillElement(children[k].Address);

    Element _flare;
    public Element flare {
        get {
            if (_flare == null)
                _flare = GetChildFromIndices(13, 0, 2, 1);
            return _flare;
        }
    }
    public string flare_count => flare?.Text;
    Element _fke;
    public Element flare_key_elem {
        get {
            if (_fke == null) {
                _fke = GetChildFromIndices(13, 0, 2, 0, 0, 1);
            }
            return _fke;
        }
    }


    public Keys flare_key {
        get {
            var val = flare_key_elem.Text;
            return (Keys)Enum.Parse(typeof(Keys), val);// Keys.F9;
        }
    }
    Element _tnt;
    public Element tnt {
        get {
            if (_tnt == null)
                _tnt = GetChildFromIndices(13, 0, 3, 1);
            return _tnt;
        }
    }
    public string tnt_count => tnt?.Text;

    Element _detonate;
    public Element detonate {
        get {
            if (_detonate == null)
                _detonate = GetChildAtIndex((int)chld_count - 1).GetTextElem_by_Str("D");
            return _detonate;
        }
    }
}
public class SkillElement : Element {
    public SkillElement(nint ptr) : base(ptr, "SkillElement") { 
    }

    public bool isValid => unknown1 != 0;

    // Usefull for aura/golums, if they are active or assigned to a key, it's value would be true.
    public bool IsAssignedKeyOrIsActive => ui.m.Read<int>(unknown1 + 0x08) > 3;

    // Couldn't find the skill path, but found skillicon path.
    public string SkillIconPath => ui.m.ReadStringU(ui.m.Read<long>(unknown1 + 0x10), 100).TrimEnd('0');

    // Number of time a skill is used ... reset on area change.
    public int totalUses => ui.m.Read<int>(unknown3 + 0x58);

    // Usefull for channeling skills only.
    public bool isUsing => ui.m.Read<byte>(unknown3 + 0x08) > 2;

    // A variable is unknown.
    private long unknown1 => ui.m.Read<long>(Address + OffsetBuffers + 0x158);
    private long unknown3 => ui.m.Read<long>(Address + OffsetBuffers + 0x230);
}
