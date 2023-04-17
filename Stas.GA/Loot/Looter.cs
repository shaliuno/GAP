using System.Collections.Concurrent;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;
using System.IO;

namespace Stas.GA;

public partial class Looter {
    public string fname => @"c:\log\loot_settings.sett";
    public LootSettings sett;
    public string debug_info = "Looter info";
    int loot_w8 = 300;
    Stopwatch sw = new Stopwatch();
    List<double> elaps = new List<double>();
    HashSet<long> bad_labels = new HashSet<long>();
    PoeNinja ninja;
    /// <summary>
    /// static loot cash
    /// </summary>
    public ConcurrentDictionary<uint, LootMapItem> loot_items = new ConcurrentDictionary<uint, LootMapItem>();
    public Looter() {
        LoadSett();
        var thr_work = new Thread(() => { //its auto starting after chenging a map - not need right now - still testing
            ninja = new PoeNinja().Load<PoeNinja>(() => {
                File.Delete("PoeNinja.sett");
                ninja = new PoeNinja();
            });
            while (true) {
                ninja.Check();
                if (!ninja.b_ready || !ui.b_ingame || ui.b_home) {
                    Thread.Sleep(loot_w8);
                    continue;
                }
                sw.Restart();
                ReadingFrameLoot();

                if (elaps.Count > 60)
                    elaps.RemoveAt(0);
                var d_elaps = sw.Elapsed.TotalMilliseconds;
                elaps.Add(d_elaps);
                var ft = elaps.Sum() / elaps.Count; //frame time
                var fps = 1000f / ft;
                debug_info += (" fps=[" + fps.ToRoundStr(0) + "] w8=[" + loot_w8 + "]");

                if (d_elaps < loot_w8)
                    Thread.Sleep(loot_w8 - (int)d_elaps);
                else
                    ui.AddToLog("Loot calculations are too slow=[" + d_elaps.ToRoundStr(0) + "]ms..", MessType.Error);
            }
        });
        thr_work.IsBackground = true;
        thr_work.Start();
    }
    List<uint> frame_keys = new();
    List<string> item_i_need = new();
    List<string> chaos_set = new();
    Dictionary<string, bool> currency = new();

    LootMapItem MakeLootMapItem(Loot loot) {
        if (loot.ClassName.EndsWith("Currency")) {
            if (currency.ContainsKey(loot.BaseName) && !currency[loot.BaseName])
                return null;
            var mi = new LootMapItem(loot);
            var ss = "";
            if (loot.stack_size > 1)
                ss = "[" + loot.stack_size + "]";
            mi.info = loot.BaseName + ss;
            var icon = GetCurrencyIcon(loot);
            var iconIndexByName = sh.IconIndexByName(icon.Item1);
            if (iconIndexByName == 0) {
                mi.info = loot.ToString();
                mi.size = 20;
                mi.uv = sh.GetUV(MapIconsIndex.question_mark);
                ui.AddToLog("AddLoot NOT found iconindex by name:" + icon.Item1);
            }
            else {
                mi.size = icon.Item2;
                mi.uv = sh.GetUV(iconIndexByName);
            }
            return mi;
        }

        if (loot.mods != null) {
            //loot.BaseName.EndsWith("Flask") ||
            if (loot.BaseName.EndsWith("Cluster Jewel")
                || loot.BaseName.EndsWith("Map")
                || (loot.sockets != null && loot.sockets.NumberOfSockets == 6) ||
                (loot.Rarity == ItemRarity.Rare && chaos_set.Any(cn => cn == loot.ClassName) && loot.mods.ItemLevel >= 60)
                || loot.Rarity == ItemRarity.Unique) {
                var mi = new LootMapItem(loot);
                mi.info = loot.BaseName;
                var mod = loot.mods.ItemMods;
                mi.size = 16;
                var name = loot.ClassName;
                if (loot.BaseName.EndsWith("Cluster Jewel")) {
                    mi.size = 25; name = "ClusterJewel";
                }
                if (loot.BaseName.EndsWith("Flask")) {
                    name = "Flask";
                }
                if (loot.BaseName.EndsWith("Map")) {
                    name = "Map";
                }
                if (loot.sockets != null && loot.sockets.NumberOfSockets == 6) {
                    name = "sockets_6";
                }
                var ii = sh.IconIndexByName(name);
                if (loot.Rarity == ItemRarity.Unique)
                    ii = MapIconsIndex.LootUniques;
                if (ii > 0)
                    mi.uv = SpriteHelper.GetUV(ii);
                else
                    mi.uv = SpriteHelper.GetUV(MapIconsIndex.question_mark);
                return mi;
            }
        }
        else {
            if (loot.ClassName.EndsWith("Skill Gem") && loot.Quality < 3) {
                return null;
            }
            var mi = new LootMapItem(loot);
            mi.info = loot.BaseName;
            var name = loot.ClassName; //loot.ClassName == "QuestItem";
            if (name == "Body Armour") {
                if (loot.Sockets == 6)
                    name = "socets_6";
                if (loot.LargestLink == 6)
                    name = "links_6";
            }

            if (loot.BaseName.EndsWith("ZZZZ"))
                name = "ZZZ";
            var ii = sh.IconIndexByName(name);
            if (ii > 0)
                mi.uv = sh.GetUV(ii);
            else
                mi.uv = sh.GetUV(MapIconsIndex.question_mark);
            return mi;
        }
        return null;
    }
    int qms = 18; //question mark icon size
    (string, int) GetCurrencyIcon(Loot loot) {
        if (ninja.prices.Count == 0) {
            ui.AddToLog("GetLootIcon err: prices.Count == 0");
            return ("question_mark", qms);
        }
        if (loot.BaseName == null) {
            ui.AddToLog("GetLootIcon err: BaseName==null");
            return ("question_mark", qms);
        }
        if (!ninja.prices.ContainsKey(loot.BaseName)) {
            if (loot.BaseName.EndsWith("Shard")) {
                var sb = loot.BaseName.Split(' ')[0];
                switch (loot.BaseName) {
                    case "Chaos Shard":
                        return GetIconByPrice(loot.stack_size * 1f / 20);
                    case "Alchemy Shard":
                    case "Transmutation Shard":
                    case "Alteration Shard":
                    case "Binding Shard":
                        var sv = ninja.prices["Orb of " + sb][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    case "Regal Shard":
                    case "Engineer's Shard":
                        sv = ninja.prices[sb + " Orb"][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    case "Horizon Shard":
                        sv = ninja.prices["Orb of Horizons"][0].value;
                        return GetIconByPrice(loot.stack_size * sv / 20);
                    default:
                        ui.AddToLog("unknow Shard base==" + sb);
                        return ("question_mark", qms);
                }
            }
            else if (loot.BaseName == "Chaos Orb")
                return GetIconByPrice(loot.stack_size * 1f);
            else {
                ui.AddToLog("Get price err for:" + loot.BaseName);
                return ("question_mark", qms);
            }
        }
        var price = ninja.prices[loot.BaseName];
        Debug.Assert(loot.ClassName != null);
        float val = 0f;
        switch (loot.ClassName) {
            case "StackableCurrency":
                val = loot.stack_size * price[0].value;
                break;
            default:
                ui.AddToLog("unknow ClassName==" + loot.ClassName);
                break;
        }
        return GetIconByPrice(val);
    }
    public (string, int) GetIconByPrice(float val) {
        var ex = ninja.exa_rate;
        var div = ninja.divine_rate;
        var alch = ninja.alchemy_rate;
        Debug.Assert(ex != 0 && div != 0 && alch != 0);
        if (val >= 3f * div) //3div+
            return ("mirror", 26);
        if (val < 3f * ex && val >= 1f * div)  //1-3div
            return ("currency0", 23);
        if (val < 1 * div && val >= ex) // ex - div
            return ("currency1", 18);
        if (val < ex && val >= 5) //5с - ex
            return ("currency2", 15);
        if (val < 5 && val >= 3) // 3-5 chaos
            return ("currency3", 12);
        if (val < 3f && val >= 1) // 1-3 chaos
            return ("currency4", 10);
        if (val < 1 && val >= alch) // orb alchemy
            return ("currency5", 8);
        if (val < alch && val > 0f)
            return ("currency6", 8);
        return ("question_mark", qms);
    }
    /// <summary>
    /// load loot items after reenter on map? like load visited
    /// </summary>
    public void LoadOldLoot() {
    }
    public void AreaChanged() {
        loot_items.Clear();
        bad_labels.Clear();
    }
}
