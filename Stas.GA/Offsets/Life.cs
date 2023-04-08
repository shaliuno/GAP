using System.Runtime.InteropServices;

namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct LifeOffset {
    [FieldOffset(0x000)] public ComponentHeader Header;
    [FieldOffset(0x178)] public VitalStruct Health; 
    [FieldOffset(0x1C8)] public VitalStruct Mana;
    [FieldOffset(0x200)] public VitalStruct EnergyShield; 
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct VitalStruct {
    [FieldOffset(0x00)]
    public IntPtr VirtualTable;

    [FieldOffset(0x08)]
    public IntPtr PtrToLifeComponent;

    [FieldOffset(0x10)]
    public int ReservedFlat;

    [FieldOffset(0x14)]
    public int ReservedPercent;

    [FieldOffset(0x28)]
    public float Regeneration;

    [FieldOffset(0x2C)]
    public int Total;

    [FieldOffset(0x30)]
    public int Current;

  
    public int ReservedTotal => (int)Math.Ceiling(this.ReservedPercent / 10000f * this.Total) + this.ReservedFlat;

   
    public int Unreserved => this.Total - this.ReservedTotal;

  
    public int CurrentInPercent {
        get {
            if (this.Total == 0) {
                return 0;
            }
            return (int)Math.Round(100d * this.Current / this.Unreserved);
        }
    }
}