namespace Stas.GA;

public class StashElement : Element {
    public StashElement(nint ptr, string name= "StashElement") : base(ptr, name) {
    }
    public long TotalStashes => StashInventoryPanel?.chld_count ?? 0;
    public Element ExitButton => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2D8)) : null;
    private Element StashTitlePanel => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2D0)) : null;
    public Element StashInventoryPanel => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2F8, "StashInventoryPanel", 0x280, 0x980)) : null;
    public Element ViewAllStashButton => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2F8, "ViewAllStashButton", 0x280, 0x988)) : null;
    public Element ViewAllStashPanel => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2F8, "ViewAllStashPanel", 0x280, 0x990)) : null;
    public Element ButtonStashTabListPin => Address != 0 ? new Element(ui.m.Read<nint>(Address + 0x2F8, "ButtonStashTabListPin", 0x280, 0x998)) : null;
    public int IndexVisibleStash => ui.m.Read<int>(Address + 0x2F8, "IndexVisibleStash", 0x280, 0x9E8);
    public Inventory VisibleStash => GetVisibleStash();
    public IList<string> AllStashNames => GetAllStashNames();
    public IList<Inventory> AllInventories => GetAllInventories();
    public IList<Element> TabListButtons => GetTabListButtons();
    Element _trade_root;
    public Element TradeRoot {get{
            if (_trade_root == null) {
                _trade_root= GetTextElem_by_Str("$")?.Parent.Parent.Parent.Parent;
            }
            return _trade_root;
     } }
    public int TradeIndex => ui.m.Read<int>(TradeRoot.Address+ 0x9E8);

    Element _set_root;
    public Element SetRoot {
        get {
            if (_set_root == null) {
                _set_root = GetTextElem_by_Str("RAB")?.Parent.Parent.Parent.Parent;
            }
            return _set_root;
        }
    }
    public int SetIndex => ui.m.Read<int>(SetRoot.Address + 0x9E8);

    private Inventory GetVisibleStash() {
        return GetStashInventoryByIndex(IndexVisibleStash);
    }

    private List<string> GetAllStashNames() {
        var ret = new List<string>();

        for (var i = 0; i < TotalStashes; i++) {
            ret.Add(GetStashName(i));
        }

        return ret;
    }

    private IList<Inventory> GetAllInventories() {
        var result = new List<Inventory>();

        for (var i = 0; i < TotalStashes; i++) {
            result.Add(GetStashInventoryByIndex(i));
        }

        return result;
    }

    public Inventory GetStashInventoryByIndex(int index) //This one is correct
    {
        if (index >= TotalStashes) return null;
        if (index < 0) return null;
        if (StashInventoryPanel.children[index].chld_count == 0) return null;

        Inventory stashInventoryByIndex = null;

        try {
            var found = StashInventoryPanel.children[index].children[0].children[0];
            stashInventoryByIndex = new Inventory(found.Address);
        } catch {
           ui.AddToLog($"Not found inventory stash for index: {index}", MessType.Error);
        }

        return stashInventoryByIndex;
    }

    public IList<Element> GetTabListButtons() {
        var listChild = ViewAllStashPanel.children.FirstOrDefault(x => x.chld_count == TotalStashes);
        return listChild?.children ?? new List<Element>();
    }

    public IList<Element> ViewAllStashPanelChildren {
        get {
            Element viewAllStashPanel = ViewAllStashPanel;
            if (viewAllStashPanel == null) {
                return null;
            }
            return viewAllStashPanel.children.Last(x => x.chld_count == TotalStashes).children.Where((Element x) => {
                IList<Element> children = x.children;
                return children != null && children.Count > 0;
            }).ToList();
        }
    }

    public string GetStashName(int index) {
        if (index >= TotalStashes || index < 0) {
            return string.Empty;
        }
        var viewAllStashPanelChildren = this.ViewAllStashPanelChildren;
        Element element;
        if (viewAllStashPanelChildren == null) {
            element = null;
        } else {
            var element2 = viewAllStashPanelChildren.ElementAt(index);
            IList<Element> children = element2.GetChildAtIndex(0).children;
            if (element2 == null) {
                element = null;
            } else {
                element = children?.Last();
            }
        }
        return element == null ? string.Empty : element.Text;
    }
}