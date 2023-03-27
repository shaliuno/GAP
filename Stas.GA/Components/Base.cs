using System.Collections.Concurrent;
using ImGuiNET;
namespace Stas.GA;

/// <summary>
///     The <see cref="Base" /> component in the entity.
/// </summary>
public class Base : EntComp {
    public Base(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<BaseOffsets>(this.Address);
        if (BaseItemTypeDatCache.TryGetValue(data.BaseInternalPtr, out var itemName)) {
            this.ItemBaseName = itemName;
        }
        else {
            var baseItemTypeDatRow = ui.m.Read<BaseItemTypesDatOffsets>(data.BaseInternalPtr);
            var name = ui.m.ReadStdWString(baseItemTypeDatRow.BaseNamePtr);
            if (!string.IsNullOrEmpty(name)) {
                BaseItemTypeDatCache[data.BaseInternalPtr] = name;
                this.ItemBaseName = name;
            }
        }
        InfluenceFlag = (Influence)data.InfluenceFlag;
        isCorrupted = (data.isCorrupted & 0x01) == 0x01;
    }
    Influence InfluenceFlag;
    public bool isShaper => (InfluenceFlag & Influence.Shaper) == Influence.Shaper;
    public bool isElder => (InfluenceFlag & Influence.Elder) == Influence.Elder;
    public bool isCrusader => (InfluenceFlag & Influence.Crusader) == Influence.Crusader;
    public bool isHunter => (InfluenceFlag & Influence.Hunter) == Influence.Hunter;
    public bool isRedeemer => (InfluenceFlag & Influence.Redeemer) == Influence.Redeemer;
    public bool isWarlord => (InfluenceFlag & Influence.Warlord) == Influence.Warlord;
    public bool isCorrupted { get; private set; }
    /// <summary>
    ///     Cache the BaseItemType.Dat data to save few reads per frame.
    /// </summary>
    private static readonly ConcurrentDictionary<IntPtr, string> BaseItemTypeDatCache = new();

    /// <summary>
    ///     Gets the items base name.
    /// </summary>
    public string ItemBaseName { get; private set; } = string.Empty;

    /// <inheritdoc />
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Base Name: {this.ItemBaseName}");
    }

    private static void OnGameClose() {
        BaseItemTypeDatCache.Clear();
    }
}

[Flags]
public enum Influence : byte {
    Shaper = 1,
    Elder = 2,
    Crusader = 4,
    Redeemer = 8,
    Hunter = 16,
    Warlord = 32,
}