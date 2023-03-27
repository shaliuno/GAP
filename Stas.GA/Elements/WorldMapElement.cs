namespace Stas.GA;

public class WorldMapElement :Element {
    public WorldMapElement(nint ptr, string name = "WorldMapElement") : base(ptr, name) {
    }
    public Element Panel => new Element(ui.m.Read<nint>(Address + 0xAB8, "Panel", 0xC10));
    public Element part_1 => GetTextElem_by_Str("Part 1");
    public Element part_2 => GetTextElem_by_Str("Part 2");
    public Element epilogue => GetTextElem_by_Str("Epilogue");

    public Element act_10 => GetTextElem_by_Str("Act 10");

    public Element parts => GetChildFromIndices(2, 0, 1);
    public Element acts => parts?.GetChildFromIndices(1, 0, 1, 1);
    public Element act_10_points => acts?.GetChildFromIndices(4, 0, 2, 0);
}
