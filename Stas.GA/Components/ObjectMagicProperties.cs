using ImGuiNET;
namespace Stas.GA;

public class ObjectMagicProperties : EntComp {
    public ObjectMagicProperties(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == default) {
            return;
        }
        var data = ui.m.Read<ObjectMagicPropertiesOffsets>(this.Address);
        var props = data.props;
        Rarity = (Rarity)props.Rarity;
        GetMods(props);
    }
    public Rarity Rarity { get; private set; } = Rarity.Normal;
    public List<string> Mods { get; private set; } = new List<string>();

    long last_hash;

    /// TODO need totale reworking
    void GetMods(ModsAndObjectMagicProperties detal) {
        var emods = detal.Mods.ExplicitMods;

        var hash = emods.GetHashCode();
        if (hash == last_hash) {//not need update
            return;
        }

        var first = emods.First.ToInt64();
        var last = emods.Last.ToInt64();
        var end = emods.First.ToInt64() + 256 * MOD_RECORD_SIZE;

        if (first == 0 || last == 0 || last < first) {
            Mods = new List<string>();
        }

        last = Math.Min(last, end);
        for (var i = first + MOD_RECORDS_OFFSET; i < last; i += MOD_RECORD_SIZE) {
            var s_ptr = ui.m.Read<long>(i + MOD_RECORD_KEY_OFFSET, tName, 0); //2718175514169
            //todo sametime null-error  here - need remake this method  
            var mod = ui.m.ReadStringU(s_ptr);
            _ModNamesList.Add(mod);
        }

        if (first == end) {
            ui.AddToLog($"{nameof(ObjectMagicProperties)} read mods error address");
        }
        last_hash = hash;
        Mods = _ModNamesList;
    }

    const int MOD_RECORDS_OFFSET = 0x18;
    const int MOD_RECORD_SIZE = 0x38;
    const int MOD_RECORD_KEY_OFFSET = 0x10;
    readonly List<string> _ModNamesList = new();

    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Rarity: {this.Rarity}");
    }

}