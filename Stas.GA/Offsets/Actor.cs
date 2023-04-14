using System.Runtime.InteropServices;
namespace Stas.GA;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ActorOffset {
    [FieldOffset(0x68)] public IntPtr ptr_68;
    [FieldOffset(0x190)] public IntPtr ent_ptr;
    [FieldOffset(0x1A8)] public IntPtr ActionPtr; 
    [FieldOffset(0x1D8)] public StdVector Effects; 
    [FieldOffset(0x208)] public short ActionId;
    [FieldOffset(0x234)] public int AnimationId; 
    [FieldOffset(0x258)] public float TimeSinceLastMove; 
    [FieldOffset(0x25C)] public float TimeSinceLastAction;
    [FieldOffset(0x690)] public StdVector ActorSkillsArray;
    [FieldOffset(0x6A8)] public StdVector ui_skills_state_array; 
    [FieldOffset(0x6C0)] public StdVector ActorVaalSkills;
    [FieldOffset(0x6D8)] public StdVector DeployedObjectArray;
  
}