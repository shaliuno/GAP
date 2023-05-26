namespace Stas.GA;

public class ObjectMagicProperties : EntComp {
    public ObjectMagicProperties(IntPtr address) : base(address) {
    }
    ObjectMagicPropertiesOffsets data;
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == default) {
            return;
        }
        //if (Address == new nint(0x00000264a865d3f0)) {
        //}
        data = ui.m.Read<ObjectMagicPropertiesOffsets>(Address);
        ImpMods = GetMods(data.ImpAfex.First, data.ImpAfex.Last);
        ExpMods = GetMods(data.ExpAfex.First, data.ExpAfex.Last);
        Mods = ImpMods?.Union(ExpMods).ToList<string>();
    }
    List<string> GetMods(long first, long last) {

        List<string> list = new List<string>();
        if (first != 0L && last != 0L) {
            for (long num = first; num < last; num += 56L) {
                string item = ui.m.ReadStringU(ui.m.Read<long>(num + 40L, "GetMods", 0), 512, true);
                list.Add(item);
                if (list.Count > 5000) {
                    break;
                }
            }
            return list;
        }
        return list;
    }
    public Rarity Rarity {
        get {
            return this.data.rarity;
        }
    }

    public bool IsIdentified {
        get {
            return base.Address == default || this.data.IsIdentified == 1;
        }
    }
    public List<string> ModNams => Mods;
    public List<string> Mods { get; private set; } = new();
    internal List<string> ImpMods { get; private set; }
    internal List<string> ExpMods { get; private set; }



}
