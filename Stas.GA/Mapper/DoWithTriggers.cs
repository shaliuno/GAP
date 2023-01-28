﻿using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public partial class AreaInstance  {
    void DoWithTriggers(Entity e) {
        if (e.gpos == V2.Zero) {
            ui.AddToLog(tName + ".DoWithTriggers: ent.gpos==Zero", MessType.Error);
            return;
        }
        if (!e.GetComp<TriggerableBlockage>(out var trigger)) {
            ui.AddToLog(tName + ".DoWithTriggers: trigger==null", MessType.Error);
            return;
        }
        var min = e.gpos.Increase(-11, -11); // trigger.Min
        var max = e.gpos.Increase(11, 11); //trigger.Max
        var gc = ui.nav.Get_gc_by_gp(e.gpos);
        if (gc != null) {
            gc.Set_all_cell_rout();
            gc.trigger = trigger;
            frame_trigger.Add(new GridCell((int)e.id, min, max) {
                b_block = trigger.IsBlocked
            });
        }
        else {
            ui.AddToLog(tName + ".DoWithTriggers: GridCell not found from nav", MessType.Error);
        }
    }
}

