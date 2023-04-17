using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;

public partial class Looter {

    //internal IList<LabelOnGround> ItemsOnGroundLabels =>labels_on_ground_elem.LabelsOnGround;
    internal IList<LabelOnGround> ItemsOnGroundLabels => labels_on_ground_elem.LabelsOnGround.Where(x => x.Address != default && x.IsVisible).ToList();
    ItemsOnGroundLabelElement labels_on_ground_elem  => new ItemsOnGroundLabelElement(ui.gui.data.itemsOnGroundLabelRoot);
    internal IntPtr labels_ptr;
    string morphPath = "Metadata/MiscellaneousObjects/Metamorphosis/MetamorphosisMonsterMarker";
    List<uint> need_delete = new List<uint>();
    void ReadingFrameLoot() {
        var la = ItemsOnGroundLabels;
        if (la == null) {
            ui.AddToLog("Looter Err: ItemsOnGroundLabels==null", MessType.Error);
            Thread.Sleep(500);
            return;
        }
        need_delete.Clear();
        frame_keys.Clear();
        //var all_sorted = la.OrderBy(e => e.ItemOnGround?.gdist_to_me).ToArray();
        var close = la.Where(delegate (LabelOnGround log) {
            if (log.Label == null || log.ItemOnGround ==null) {
                return false;
            }
            return log.Label.IsValid && log.ItemOnGround.IsValid && log.ItemOnGround.gdist_to_me < ui.sett.loot_dist;

        }).OrderBy(e => e.ItemOnGround?.gdist_to_me).ToList(); ;
      
      
        foreach (var l in close) { //9896.739, 8516.304, -211.15015
            var e = l.ItemOnGround;
            var cpa = l.CanPickUp;
            if (e == null || !e.IsValid || e.pos == V3.Zero || bad_labels.Contains(l.Address) )
                continue; //|| Math.Abs(ui.me.Pos.Z - e.Pos.Z) > 50
          
            if (!e.GetComp<WorldItem>(out var worldItem))
                continue;
            //e.GetComp<Targetable>(out var trg);
            var item_ent = worldItem.ItemEntity;
          
            if (item_ent?.Path == null || item_ent?.Path.Length < 1)
                continue;
            try {
                var key = e.GetKey;
                if (loot_items.ContainsKey(key)) {
                    var bit = ui.BaseItemTypes.Translate(item_ent.Path);
                    var old = loot_items[key].loot.BaseName == bit.BaseName
                      && loot_items[key].loot.ClassName == bit.ClassName;
                    if (old) {
                        loot_items[key].Update(l);
                        frame_keys.Add(key);
                        continue;
                    } else {
                        ui.AddToLog("wrong loot ent.key", MessType.Error);
                    }
                }
                var loot = new Loot(l, item_ent);
                var bna = loot.BaseName.Split(" ");
                var last_base_name = bna[bna.Length - 1];
                var bad_rarity = loot.Rarity < ItemRarity.Rare || loot.IsIdentified;
                var b_chaos_Set = chaos_set.Contains(loot.ClassName) && !bad_rarity;
                var b_need = item_i_need.Contains(loot.ClassName) ;
                var b_6s = false;
                if(loot.Sockets == 6){
                    if ((loot.Height == 4 && sett.b_6s_big) || (loot.Height == 3 && sett.b_6s_small))
                        b_6s = true;
                }
                if (!b_need  && !b_6s && !b_chaos_Set)
                    continue;
            
                var mi = MakeLootMapItem(loot);
                if (mi != null) {
                    loot_items[mi.key] = mi;
                    frame_keys.Add(key);
                }
            } catch (Exception ex) {
                ui.AddToLog("AddLoot Err: " + ex.Message);
                continue;
            }
        }
        //checking for the validity of the loot static cash in loot_dist radius
        //by comparing with the one received in the last frame
        //necessary if we collecting a loot manually
        foreach (var l in loot_items.Where(li => 
                    li.Value.gpos.GetDistance(ui.me.gpos) < ui.sett.loot_dist)) {
            if (!frame_keys.Contains(l.Key))
                need_delete.Add(l.Key);
        }
        foreach (var l in need_delete)
            loot_items.TryRemove(l, out _);
        debug_info = "loot=[" + la.Count + "/" + close.Count + "/" + loot_items.Count + "]"; 
        void DebugFlasks() {
            var flask = la.FirstOrDefault(l => l.ToString() == "Eternal Mana Flask");
            ui.AddToLog("count=[" + la.Count + "] pos=[" + flask?.Label.Position.ToString() + "]");
        }
    }

   
}


