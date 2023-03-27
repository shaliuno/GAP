using System.Runtime.InteropServices;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ItemsOnGroundLabelElementOffsets {
    [FieldOffset(648)] //0x288
    public IntPtr LabelOnHoverPtr;

    [FieldOffset(656)] //0x290
    public IntPtr ItemOnHoverPtr;

    [FieldOffset(672)] //0x2A0
    public IntPtr LabelsOnGroundListPtr;
}
