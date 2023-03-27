namespace Stas.GA {
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BaseOffsets {
        [FieldOffset(0x10)] public IntPtr BaseInternalPtr;
        //[FieldOffset(0x18)] public long ItemVisualIdentityKey;
        //[FieldOffset(0x38)] public long FlavourTextKey;
        [FieldOffset(0x0030)] public StdWString BaseNamePtr;
        [FieldOffset(0x60)] public long PublicPricePtr;
        [FieldOffset(0x78)] public StdWString ItemDescription;
        [FieldOffset(0x80)] public StdWString PtrEntityPath; //offset 0 from this leads back to "Metadata/..."
        [FieldOffset(0x88)] public StdWString ItemType; //offset 0 from this
        [FieldOffset(0x90)] public long BaseItemTypesPtr;
        [FieldOffset(0xC6)] public byte InfluenceFlag;
        [FieldOffset(0xC7)] public byte isCorrupted;
        [FieldOffset(0xC8)] public int UnspentAbsorbedCorruption;
        [FieldOffset(0xCC)] public int ScourgedTier;
    }
}