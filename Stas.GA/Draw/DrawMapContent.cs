using System.Drawing;
using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;


namespace Stas.GA;
partial class DrawMain {
    void DrawMapContent() {
        if (ui.me.Address == default) {
            ui.AddToLog("draw Map Err: ui.me==null", MessType.Error);
            return;
        }
        sw_map.Restart();
        //ui.me.Tick(ui.me.Address);//we tick here mb for more precision

        var rm = ui.MTransform(); //true
        var lt = V2.Transform(new V2(0f, 0f), rm);
        var rt = V2.Transform(new V2(ui.curr_map.cols, 0f), rm);
        var rb = V2.Transform(new V2(ui.curr_map.cols, ui.curr_map.rows), rm);
        var lb = V2.Transform(new V2(0f, ui.curr_map.rows), rm);
        if (ui.curr_map.map_ptr != IntPtr.Zero)
            map_ptr.AddImageQuad(ui.curr_map.map_ptr, lt, rt, rb, lb);

        var di = ui.curr_map.mi_debug;//local snap this thred. mb need use lock(){}
        if (di != null) {
            DrawDebugMapItem(map_ptr, di);
            return;
        }
        DrawNavVisited();
        var mia = ui.curr_map.map_items.OrderBy(e => e.priority);
        foreach (var mi in mia) {
            DrawMapItem(mi);
        }

        if (ui.b_contrl) {
            DrawStaticItems();
            DrawBlight();
            DrawExped();
        }
        else {
            DrawExped();
            DrawBlight();
            DrawStaticItems();
        }

        foreach (var it in ui.curr_map.iTasks) {
            var to = V2.Transform(it.to, ui.MTransform());
            var his = 12;

            var from = V2.Transform(it.from, ui.MTransform());
            map_ptr.AddLine(from, to, it.color.ToImgui(), it.line);
            map_ptr.AddCircleFilled(to, 5, Color.Gray.ToImgui());

            map_ptr.AddText(to.Increase(his, -his / 2), Color.LightGreen.ToImgui(), it.info);
        }
        if (ui.tasker != null) {
            foreach (var it in ui.tasker.i_tasks) {
                var to = V2.Transform(it.to, ui.MTransform());
                var his = 12;
                var from = V2.Transform(it.from, ui.MTransform());
                map_ptr.AddLine(from, to, it.color.ToImgui(), it.line);
                map_ptr.AddCircleFilled(to, 5, Color.Gray.ToImgui());
                map_ptr.AddText(to.Increase(his, -his / 2), Color.LightGreen.ToImgui(), it.info);
            }
        }
    }
}
