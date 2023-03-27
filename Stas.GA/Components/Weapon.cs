
namespace Stas.GA;
public class Weapon : EntComp {
    public Weapon(nint address) : base(address) {
    }
    internal override void Tick(nint ptr, string from = null) {
        DamageMin = ui.m.Read<int>(Address + 0x28, tName, 0x14);
        DamageMax = ui.m.Read<int>(Address + 0x28, tName, 0x18);
        AttackTime = ui.m.Read<int>(Address + 0x28, tName, 0x1C);
        CritChance = ui.m.Read<int>(Address + 0x28, tName, 0x20);
    }
    public int DamageMin { get; private set; } = 0;
    public int DamageMax { get; private set; }= 0;
    public int AttackTime { get; private set; }= 1;
    public int CritChance { get; private set; } = 0;

   
}
