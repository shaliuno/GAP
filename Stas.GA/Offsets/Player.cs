namespace Stas.GA;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PlayerOffsets
{
    [FieldOffset(0x000)] public ComponentHeader Header;
    [FieldOffset(0x168)] public StdWString Name; //3.21
}