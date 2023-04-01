namespace Stas.GA;
public class Mods : EntComp {
    public Mods(IntPtr ptr) : base(ptr) {
    }
    bool b_init = false;
    void Init() {
        var data = ui.m.Read<ModsComponentOffsets>(Address);
        Identified = data.Identified;
        var mods = GetMods(data.ScourgeModsArray);
        var mods2 = GetMods(data.EnchantedModsArray);
        var mods3 = GetMods(data.ImplicitModsArray);
        var mods4 = GetMods(data.ExplicitModsArray);
        ItemMods = mods.Concat(mods2).ToList<ItemMod>().Concat(mods4).ToList<ItemMod>().Concat(mods3).ToList<ItemMod>();
        ItemRarity = (ItemRarity)data.ItemRarity;
        ItemLevel = data.ItemLevel;

        if (data.ModsComponentDetailsKey > Address) {
            var details = ui.m.Read<ModsComponentDetailsOffsets>(data.ModsComponentDetailsKey);
            HumanCraftedStats = GetStats(details.CraftedStatsArray);
            HumanEnchantedStats = GetStats(details.EnchantedStatsArray);
            HumanFracturedStats = GetStats(details.FracturedStatsArray);
            HumanImpStats = GetStats(details.ImplicitStatsArray);
            HumanScourgeStats = GetStats(details.ScourgeStatsArray);
            HumanStats = GetStats(details.ExplicitStatsArray);
        }

        IsMirrored = data.IsMirrored == 1;
        IsSplit = data.IsSplit == 1;
        IsSynthesized = data.IsSynthesised == 1;
        IsUsable = data.IsUsable == 1;
        IsVeiled = VeiledCount > 0;
        b_init = true;
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        version = ui.curr_frame;
        if (Address == IntPtr.Zero) {
            Clear();
            return;
        }

        //PublicPrice = ui.m.Read<NativeStringU>(BaseStruct.PublicPricePtr).ToString(M);
    }
    protected override void Clear() {
        b_init = false;
        ItemMods.Clear();
        ItemRarity = ItemRarity.Normal;
        ItemLevel = 1;
    }
    public int VeiledCount => ItemMods?.Count(x => x.Group.StartsWith("Veiled")) ?? 0;
    public bool Identified { get; private set; }
    public string PublicPrice { get; private set; }
    public List<string> HumanCraftedStats { get; private set; } = new List<string>();
    public List<string> HumanEnchantedStats { get; private set; } = new List<string>();
    public List<string> HumanFracturedStats { get; private set; } = new List<string>();
    public List<string> HumanImpStats { get; private set; } = new List<string>();
    public List<string> HumanScourgeStats { get; private set; } = new List<string>();
    public List<string> HumanStats { get; private set; } = new List<string>();
    public int FracturedCount => HumanFracturedStats.Count;
    public bool IsFractured => FracturedCount > 0;
    public bool IsMirrored { get; private set; }
    public bool IsSplit { get; private set; }
    public bool IsSynthesized { get; private set; }
    public bool IsTalisman => ItemMods != null && ItemMods.Any(x => x.RawName.StartsWith("Talisman"));
    public bool IsUsable { get; private set; }
    public bool IsVeiled { get; private set; }
    public List<ItemMod> ItemMods { get; private set; } = new();
    public ItemRarity ItemRarity { get; private set; } = ItemRarity.Normal;
    public int ItemLevel { get; private set; } = 1;
    private List<string> GetStats(StdVector source) {
        var stats = new List<string>();
        if (Address == 0) {
            return stats;
        }
        var readPointersArray = ui.m.ReadPointersArray(source.First, source.Last, ModsComponentOffsets.StatRecordSize);
        stats.AddRange(readPointersArray.Select(statAddress =>
            ui.string_cashe.Read((nint)statAddress, () => ui.m.ReadStringU(statAddress))));

        return stats;
    }
    private IEnumerable<ItemMod> GetMods(StdVector source) {
        var mods = new List<ItemMod>();
        if (Address == 0) {
            return mods;
        }

        if (source.Size / ModsComponentOffsets.ItemModRecordSize > 24) {
            return mods;
        }

        for (var modAddress = source.First;
             modAddress < source.Last;
             modAddress += ModsComponentOffsets.ItemModRecordSize) {
            mods.Add(new ItemMod(modAddress));
        }

        return mods;
    }   
}

