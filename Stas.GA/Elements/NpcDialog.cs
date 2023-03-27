namespace Stas.GA;

public enum NpcLinks {
    Continue, Goodbye, Purchase, Sell
}

public class NpcDialog :Element{
    public NpcDialog(IntPtr ptr, string name= "NpcDialog") :base(ptr, name) {
    }
    public string npc_name => GetChildFromIndices(1, 3)?.Text;
    public List<Element> links { get {
            var links = new List<Element>();
            var ela = GetChildFromIndices(0, 2);
            foreach(var e in ela.children)
                if(e.children.Count==1)
                    links.Add(new Element(e.GetChildAtIndex(0).Address));
            return links;
        } }
    public Element GetLinkWith(string txt) {
        return GetTextElem_by_Str(txt);
    }

    public Element GetLinkEndsWith(string txt) {
        //return res.GetObject<LinkElement>(res.Address);
        return GetElem_ends_wit(txt);
    }

    public Element Reward { get {
            return GetLinkEndsWith("Reward")??GetLinkEndsWith("Reward 2");
        } }
    public Element Goodbye => GetLinkWith("Goodbye");
    public Element Continue => GetLinkWith("Continue");
    public Element Sell => GetLinkWith("Sell Items");
    public Element Purchase => GetLinkWith("Purchase Items");
}

