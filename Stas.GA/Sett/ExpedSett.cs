using System.Security.Cryptography.X509Certificates;

namespace Stas.GA;
public class ExpedSett : iSett {
    public Dictionary<string, int> mods = new();
    public const string prefix = "ExpeditionRelicModifier";
    [JsonInclude]
    public int radius_persent = 0;
    [JsonInclude]
    public int range_persent = 0;
    public ExpedSett(): base("ExpedSett"){ 
        //https://discord.com/channels/1036511025622679582/1036511025622679590/1062412139324784730
        //супер сильно выдели под runic monsters are duplicated

        mods.Add("ElitesDuplicated", 50);//top mods
        mods.Add("ExpeditionCurrencyQuantityChest", 10);
        mods.Add("ExpeditionCurrencyQuantityMonster", 10);
        mods.Add("PackSize", 10);

        mods.Add("ItemQuantityMonster", 5);
        mods.Add("ItemQuantityChest", 5);
        mods.Add("ExpeditionLogbookQuantityChest", 6);
        mods.Add("ExpeditionLogbookQuantityMonster", 6);
        mods.Add("ItemRarityMonster", 3);
        mods.Add("ItemRarityChest", 3);

        mods.Add("StackedDeckElite", 2);
        mods.Add("StackedDeckChest", 2);
        mods.Add("ExpeditionBasicCurrencyElite", 3);
        mods.Add("ExpeditionBasicCurrencyChest", 3);

        mods.Add("LegionSplintersElite", 1); //low? but have ingame icons redy
        mods.Add("LegionSplintersChest", 1);
        mods.Add("EternalEmpireLegionElite", 1);
        mods.Add("EternalEmpireLegionChest", 1);
        mods.Add("ExpeditionUniqueElite", 1);
        mods.Add("ExpeditionUniqueChest", 1);
        mods.Add("LostMenUniqueElite", 1);
        mods.Add("LostMenUniqueChest", 1);
        mods.Add("EssencesElite", 1);
        mods.Add("EssencesChest", 1);
        mods.Add("LostMenEssenceElite", 1);
        mods.Add("LostMenEssenceChest", 1);
        mods.Add("VaalGemsElite", 1);
        mods.Add("VaalGemsChest", 1);
        mods.Add("ExpeditionGemsElite", 1);
        mods.Add("ExpeditionGemsChest", 1);
        mods.Add("EternalEmpireEnchantElite", 1);
        mods.Add("EternalEmpireEnchantChest", 1);
        mods.Add("BreachSplintersElite", 1);
        mods.Add("BreachSplintersChest", 1);
        mods.Add("ExpeditionMapsElite", 1);
        mods.Add("ExpeditionMapsChest", 1);
        mods.Add("RareMonsterChance", 1);

        mods.Add("HarbingerCurrencyChest", 2);
        mods.Add("HarbingerCurrencyElite", 2);
        mods.Add("ExpeditionFracturedItemsElite", 3);
        mods.Add("ExpeditionFracturedItemsChest", 3);
        mods.Add("ExpeditionInfluencedItemsElite", 4);
        mods.Add("ExpeditionInfluencedItemsChest", 4);
        mods.Add("SirensScarabElite", 3);
        mods.Add("SirensScarabChest", 3);
        mods.Add("ExpeditionRareTrinketElite", 3);
        mods.Add("ExpeditionRareTrinketChest", 3);

        mods.Add("AttackBlockSpellBlockMaxBlockChance", -55);
        mods.Add("CannotBeCrit", -25);
        mods.Add("ImmuneStatusAilments", 0); //-11
        mods.Add("ImmunePhysicalDamage", 0); //-9
        mods.Add("ImmuneFireDamage", -13);
        mods.Add("ImmuneColdDamage", -17);
        mods.Add("ImmuneLightningDamage", -15);
        mods.Add("ImmuneChaosDamage", 0); //-7

        mods.Add("ElitesRegenerateLifeEveryFourSeconds", -2);
        mods.Add("ExpeditionCorruptedItemsElite", -2);
        mods.Add("ResistancesAndMaxResistances", -2);
        mods.Add("CannotBeLeechedFrom", -2);
        mods.Add("ImmuneToCurses", -2);

        //no idea what is but need check
        mods.Add("Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers", 3);
        mods.Add("Metadata/Terrain/Doodads/Leagues/Expedition/monstermarker_set", 3);
        mods.Add("Metadata/Terrain/Doodads/Leagues/Expedition/elitemarker_set", 3);
        
    }
    public override string fname => @"exped_settings.sett";
    [JsonInclude]
    public bool PhysImmune = true;
    [JsonInclude]
    public bool FireImmune = true;
    [JsonInclude]
    public bool ColdImmune = true;
    [JsonInclude]
    public bool LightningImmune = true;
    [JsonInclude]
    public bool ChaosImmune = true;
    [JsonInclude]
    public bool CritImmune = true;
    [JsonInclude]
    public bool AilmentImmune = true;
    [JsonInclude]
    public bool Culling = true;
    [JsonInclude]
    public bool CorruptedItems = true;
    [JsonInclude]
    public bool Regen = true;
    [JsonInclude]
    public bool BlockChance = true;
    [JsonInclude]
    public bool MaxResistances = true;
    [JsonInclude]
    public bool NoLeech = true;
    [JsonInclude]
    public bool NoCurse = true;
}
