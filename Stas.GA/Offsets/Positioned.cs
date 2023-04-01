using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PositionedOffsets {
    [FieldOffset(0x000)] public ComponentHeader Header;
    [FieldOffset(0x1D8)] public byte Reaction; 
    [FieldOffset(0x1D9)] public byte unkn_1d8;
    [FieldOffset(0x290)] public float Rotation;
    [FieldOffset(0x2A0)] public float Size;
    [FieldOffset(0x27C)] public float SizeScale;
    [FieldOffset(0x288)] public StdTuple2D<int> GridPosition; 
    [FieldOffset(0x2AC)] public V2 curr_pos; 
}
