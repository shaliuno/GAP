﻿using System.Drawing;
using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;
namespace Stas.GA;
partial class DrawMain {
    List<MapItem> double_icons = new();
    void DrawMapContent() {
        double_icons.Clear();
        if (ui.me.Address == default && !ui.sett.b_league_start) {
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

        V2 uv = default;
        V2 uv2 = new V2(1f, 0f);
        V2 uv3 = new V2(1f, 1f);
        V2 uv4 = new V2(0f, 1f);
        var transp = Color.FromArgb(ui.sett.map_alpha, 255, 255, 255);

        if (ui.curr_map.map_ptr != IntPtr.Zero)
            map_ptr.AddImageQuad(ui.curr_map.map_ptr, lt, rt, rb, lb, uv, uv2, uv3, uv4, transp.ToImgui());

        var di = ui.curr_map.mi_debug;//local snap this thred. mb need use lock(){}
        if (di != null) {
            DrawDebugMapItem(map_ptr, di);
            return;
        }
        DrawNavVisited();
        var mia = ui.curr_map.map_items.OrderBy(e => e.priority);
        foreach (var mi in mia) {
            if (mi.priority == IconPriority.Double) {
                DrawMapItem(mi, mi.uv2, mi.size);
                double_icons.Add(mi);
            }
            else
                DrawMapItem(mi, mi.uv, mi.size);
        }
        foreach (var mi in double_icons) {
            DrawMapItem(mi, mi.uv, AreaInstance.GetIconSizeByRarity(mi.ent.rarity));
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
