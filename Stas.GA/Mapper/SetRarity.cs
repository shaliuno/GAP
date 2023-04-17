using sh = Stas.GA.SpriteHelper;
namespace Stas.GA;

public partial class AreaInstance {
    public static float GetIconSizeByRarity(Rarity rar) {
        switch (rar) {
            case Rarity.Normal:
                return ui.sett.icon_size;
            case Rarity.Magic:
                return ui.sett.icon_size + 2;
            case Rarity.Rare:
                return ui.sett.icon_size + 4;
            case Rarity.Unique:
                return ui.sett.icon_size + 6;
            default:
                return ui.sett.icon_size;
        }
    }
    void SetRarity(MapItem nmi) {
        switch (nmi.ent.rarity) {
            case Rarity.Normal:
                nmi.priority = IconPriority.Low;
                nmi.size = GetIconSizeByRarity(Rarity.Normal);
                nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeRedCircle); //LootFilterLargeCyanCircle
                break;
            case Rarity.Magic:
                nmi.priority = IconPriority.Medium;
                nmi.size = GetIconSizeByRarity(Rarity.Magic);
                nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeBlueCircle);
                break;
            case Rarity.Rare:
                nmi.priority = IconPriority.High;
                nmi.size = GetIconSizeByRarity(Rarity.Rare);
                nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargeYellowCircle);
                break;
            case Rarity.Unique:
                nmi.priority = IconPriority.Critical;
                nmi.size = GetIconSizeByRarity(Rarity.Unique);
                nmi.uv = sh.GetUV(MapIconsIndex.LootFilterLargePurpleCircle);
                break;
            default:
                nmi.uv = sh.GetUV(MapIconsIndex.unknow);
                ui.AddToLog("SetRarity err: " + nmi.ent.rarity);
                break;
                //throw new NotImplementedException();
        }
    }
}
