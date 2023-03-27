using System.Runtime.InteropServices;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct FilesOffsets {
    [FieldOffset(0x8)] public long ListPtr;
    [FieldOffset(0x18)] public long MoreInformation;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FileNode {
    public long Next;
    public long Prev;
    public long Key;
    public long Value;
}



[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct FileRootBlock {
    [FieldOffset(0x10)] public long Capacity;
    [FieldOffset(0x08)] public long FileNodesPtr;
    [FieldOffset(0x20)] public long Count;
}
