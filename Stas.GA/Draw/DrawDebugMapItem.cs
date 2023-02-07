#region using
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using Color = System.Drawing.Color;
using sysd = System.Drawing;
using System.Text;
using System.Drawing;
using System.Linq;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using System.Diagnostics;

#endregion
namespace Stas.GA {
    partial class DrawMain {
        void DrawDebugMapItem(ImDrawListPtr bptr, aMapItem di) {
            Debug.Assert(di != null);
            var rm = ui.MTransform();
            var info = di.info;
            if (string.IsNullOrEmpty(di.info))
                info = "Error info";
            if (di is StaticMapItem)
                info += " [St]";
            info = "t=[" + di.ent.eType + "] id=[" + di.ent.id + "] " + info;

            //info = "d=" + Math.Round(ui.me.Pos.GetDistance(ami.pos) * ui.worldToGridScale, 0) + " " + ami.info;
            var b_draw = true;
            if (di is StaticMapItem) {
                var smi = (StaticMapItem)di;
                if (smi.remn != null) {
                    var offs = 0f;
                    b_draw = false;
                    V2 ph = default, nh = default;
                    if (smi.remn.positive.Count > 0) {
                        info = "";
                        foreach (var v in smi.remn.positive)
                            info += v.Key + "\n";
                        DrawInfo(info, Color.Green, Color.LightGreen);
                        ph = ImGui.CalcTextSize(info) * 4;
                    }
                    if (smi.remn.negative.Count > 0) {
                        info = "";
                        foreach (var v in smi.remn.negative)
                            info += v.Key + "\n";
                        DrawInfo(info, Color.Red, Color.LightPink, ph.Y);
                        nh = ImGui.CalcTextSize(info) * 4;
                    }

                    if (smi.remn.unknow.Count > 0) {
                        info = "";
                        foreach (var v in smi.remn.unknow)
                            info += v.Key + "\n";
                        DrawInfo(info, Color.Gray, Color.LightGray, ph.Y + nh.Y);
                    }
                }
            }
            if (b_draw)
                DrawInfo(info, Color.Green, Color.LightGreen);

            void DrawInfo(string _inp, Color bg, Color bord, float offs = 0) {
                var ts = ImGui.CalcTextSize(_inp);
                var mi_gpos = di.pos * ui.worldToGridScale;
                var his = ts.Y;
                var pos = V2.Transform(new V2(mi_gpos.X, mi_gpos.Y), rm);
                var lt = pos.Increase(0, -his + offs);
                var rt = pos.Increase(ts.X + 2 * his, -his + offs);
                var lb = pos.Increase(0, 1.5f * his + offs);
                var rb = pos.Increase(ts.X + 2 * his, 1.5f * his + offs);
                var tcolor = Color.Black.ToImgui();
                map_ptr.AddQuadFilled(lt, rt, rb, lb, bg.ToImgui());
                map_ptr.AddQuad(lt, rt, rb, lb, bord.ToImgui(), 2f);
                bptr.AddText(pos.Increase(20, offs), tcolor, _inp);
            }
        }
    }
}
