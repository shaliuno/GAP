using Color = System.Drawing.Color;
using System.Drawing;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
namespace Stas.GA; 

partial class DrawMain {
    void DrawMapItem(aMapItem ami, RectangleF uv, float size) {

        if (ami.uv == RectangleF.Empty) {
            ui.AddToLog("DrawMapItem err: uv==empty for: " + ami.ToString(), MessType.Error);
            return;
        }
        if (ui.b_only_unknow && ami.mii != MapIconsIndex.unknow)
            return;

        var rm = ui.MTransform();
        var his = size / 2;
        his *= ui.sett.map_scale;
        var mi_gpos = ami.pos * ui.worldToGridScale;
        var pos = V2.Transform(new V2(mi_gpos.X, mi_gpos.Y), rm);
        var p_min = new V2(uv.Left, uv.Top);
        var p_max = new V2(uv.Left + uv.Width, uv.Top + uv.Height);
        var transp = Color.FromArgb(ui.sett.map_icon_alpha, 255,255,255);
        map_ptr.AddImage(icons, pos.Increase(-his, -his), pos.Increase(his, his), p_min, p_max, transp.ToImgui());

        if (ami is StaticMapItem) {
            var smi = (StaticMapItem)ami;
            if (smi.b_done) {
                var uvd = SpriteHelper.GetUV(MapIconsIndex.done);
                map_ptr.AddImage(icons, pos.Increase(-his, -his), pos.Increase(his, his),
                    new V2(uvd.Left, uvd.Top),
                    new V2(uvd.Left + uvd.Width, uvd.Top + uvd.Height));
            }
            if (smi.remn != null) {
                var pval = smi.remn.positive.Sum(p => p.Value);
                var pval_str = pval.ToString(); if (pval_str.Length == 1) pval_str = " " + pval_str;
                if (pval == 0) pval_str = "...";
                map_ptr.AddText(pos.Increase(-10, -15), Color.White.ToImgui(), pval_str);

                var nval = smi.remn.negative.Sum(p => p.Value);
                var n_val_str = nval.ToString(); if (n_val_str.Length == 1) n_val_str = " " + n_val_str;
                if (nval == 0) n_val_str = "...";
                map_ptr.AddText(pos.Increase(-10, 0), Color.White.ToImgui(), n_val_str);
            }
        }
      
        if (ui.b_contrl && ui.sett.b_develop) {
            if (ami is MapItem && ami != null && ami.ent?.danger_rt > 0) {
                var lt = pos.Increase(-his, -0.7f * his);
                var rt = pos.Increase(20, -0.7f * his);
                var lb = pos.Increase(-his, 0.7f * his + 3);
                var rb = pos.Increase(20, 0.7f * his + 3);
                map_ptr.AddQuadFilled(lt, rt, rb, lb, Color.Red.ToImgui());
                map_ptr.AddText(pos.Increase(0, -his), Color.White.ToImgui(),
                    ami.ent.danger_rt.ToRoundStr(3));

                if (ami.ent.GetComp<Positioned>(out var test)) {
                    //var past = V2.Transform(test.past_pos * ui.worldToGridScale, ui.MTransform());
                    var curr = V2.Transform(test.curr_pos * ui.worldToGridScale, ui.MTransform());
                    //var next = V2.Transform(test.next_pos * ui.worldToGridScale, ui.MTransform());

                    //map_ptr.AddLine(past, curr, Color.Red.ToImgui(), 2);
                    //map_ptr.AddLine(curr, next, Color.Blue.ToImgui(), 2);
                }

            }
            var info = ami.ent?.eType+ " id=" + ami.ent?.id;
            map_ptr.AddText(pos.Increase(his / 2, -8), Color.DarkGray.ToImgui(), info) ;
        }

        if (ui.b_alt) {// 
            var info = ami.info; 
            if(string.IsNullOrEmpty(info))
                info= "id=" + ami.ent?.id;
            if (ami is StaticMapItem) {
                info += " [St]";
            }
            map_ptr.AddText(pos.Increase(his / 2, -8), Color.LightGreen.ToImgui(), info);
        }
    }
}
