namespace Stas.GA;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ObjectMagicPropertiesOffsets {
    [FieldOffset(0x140)] public byte IsIdentified;
    [FieldOffset(0x144)] public Rarity rarity;
    [FieldOffset(0x150)] public StdVector ImpAfex;
    [FieldOffset(0x168)] public StdVector ExpAfex;
    [FieldOffset(0x210)] public StdVector StatDict;
}
