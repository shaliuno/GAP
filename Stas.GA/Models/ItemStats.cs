namespace Stas.GA;

public sealed class ItemStats {
    private static StatTranslator translate;
    private readonly Entity item;
    private readonly float[] stats;

    public ItemStats(Entity item) {
        this.item = item;
        if (translate == null) translate = new StatTranslator();
        stats = new float[Enum.GetValues(typeof(ItemStatEnum)).Length];
        ParseSockets();
        ParseExplicitMods();
        if (item.GetComp<Weapon>(out var _)) 
            ParseWeaponStats();
    }

    private void ParseWeaponStats() {
        if(!item.GetComp<Weapon>(out var comp) || !item.GetComp<Quality>(out var qul)) 
            return;
        var num = (comp.DamageMin + comp.DamageMax) / 2f + GetStat(ItemStatEnum.LocalPhysicalDamage);
        num *= 1f + (GetStat(ItemStatEnum.LocalPhysicalDamagePercent) + qul.ItemQuality) / 100f;
        AddToMod(ItemStatEnum.AveragePhysicalDamage, num);
        var num2 = 1f / (comp.AttackTime / 1000f);
        num2 *= 1f + GetStat(ItemStatEnum.LocalAttackSpeed) / 100f;
        AddToMod(ItemStatEnum.AttackPerSecond, num2);
        var num3 = comp.CritChance / 100f;
        num3 *= 1f + GetStat(ItemStatEnum.LocalCritChance) / 100f;
        AddToMod(ItemStatEnum.WeaponCritChance, num3);
        var num4 = GetStat(ItemStatEnum.LocalAddedColdDamage)
            + GetStat(ItemStatEnum.LocalAddedFireDamage)
            + GetStat(ItemStatEnum.LocalAddedLightningDamage);
        AddToMod(ItemStatEnum.AverageElementalDamage, num4);
        AddToMod(ItemStatEnum.DPS, (num + num4) * num2);
        AddToMod(ItemStatEnum.PhysicalDPS, num * num2);
    }

    private void ParseExplicitMods() {
        if (item.GetComp<Mods>(out var mods)) {
            foreach (var current in mods.ItemMods) {
                translate.Translate(this, current);
            }
        }
        AddToMod(ItemStatEnum.ElementalResistance,  GetStat(ItemStatEnum.LightningResistance) 
            + GetStat(ItemStatEnum.FireResistance) + GetStat(ItemStatEnum.ColdResistance));

        AddToMod(ItemStatEnum.TotalResistance, GetStat(ItemStatEnum.ElementalResistance) + GetStat(ItemStatEnum.TotalResistance));
    }

    private void ParseSockets() {
        //todo ParseSockets do nothing
    }

    public void AddToMod(ItemStatEnum stat, float value) {
        stats[(int)stat] += value;
    }

    public float GetStat(ItemStatEnum stat) {
        return stats[(int)stat];
    }

}
