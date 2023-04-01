using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct RenderOffsets {
    [FieldOffset(0x0000)] public ComponentHeader Header;
    // Same as Positioned Component CurrentWorldPosition,
    // but this one contains Z axis; Z axis is where the HealthBar is.
    // If you want to use ground Z axis, swap current one with TerrainHeight.
    [FieldOffset(0x98)] public V3 CurrentWorldPosition; 
    [FieldOffset(0xC0)] public NativeStringU name;
    // Changing this value will move the in-game healthbar up/down.
    // Not sure if it's really X,Y,Z or something else. They all move
    // healthbar up/down. This might be useless.
    [FieldOffset(0xA4)] public V3 CharactorModelBounds; 
    // [FieldOffset(0x00A0)] public StdWString ClassName;

    // Exactly the same as provided in the Positioned component.
    // [FieldOffset(0x00C0)] public float RotationCurrent;
    // [FieldOffset(0x00C4)] public float RotationFuture;
    [FieldOffset(0xFC)] public float TerrainHeight; 
}
