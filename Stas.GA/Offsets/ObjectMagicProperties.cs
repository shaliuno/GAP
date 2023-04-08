namespace Stas.GA;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ObjectMagicPropertiesOffsets { //3.21
    [FieldOffset(0x90)] public ModsAndObjectMagicProperties props; //144
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ModsAndObjectMagicProperties {
    [FieldOffset(324)]
    public int Rarity;

    [FieldOffset(192)]
    public AllModsType Mods;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AllModsType {
    public StdVector ImplicitMods;

    public StdVector ExplicitMods;

    public StdVector EnchantMods;

    public StdVector HellscapeMods;

    public StdVector CrucibleMods;
}