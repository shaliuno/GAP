using System.Drawing;
namespace Stas.GA;
using V2 = System.Numerics.Vector2;
partial class DrawMain {
    void DrawNavDebug() {
        if(!ui.sett.b_debug_nav_node)
            return;
        var rm = ui.MTransform();
        foreach (var node in ui.nav.explorer.Graph.Nodes) {
            const int cs = 10; //CIRCLE_SIZE
            var randomColor = Color.Gray;
            if (node.GraphPart != null) {
                var rand = new Random(node.GraphPart.Id);
                randomColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            }
            var color = Color.FromArgb(200, Color.Black);
            if (node.IsVisited)
                color = _green2;
            var centre = V2.Transform(new V2(node.Pos.X, node.Pos.Y), rm);
            map_ptr.AddCircleFilled(centre, cs, color.ToImgui());
            foreach (var link in node.Links) {
                var from = V2.Transform(new V2(node.Pos.X, node.Pos.Y), rm);
                var to = V2.Transform(new V2(link.Pos.X, link.Pos.Y), rm);
                map_ptr.AddLine(from, to, randomColor.ToImgui(), 1);
            }

            if (node.PriorityFromEndDistance != 0) {
                var tpos = V2.Transform(new V2(node.Pos.X, node.Pos.Y), rm);
                map_ptr.AddText(tpos, Color.White.ToImgui(), node.PriorityFromEndDistance.ToString());

            }

            if (ui.nav.path != null) {
                Node prev = null;

                foreach (var n in ui.nav.path) {
                    if (prev != null) {
                        var from = V2.Transform(new V2(prev.Pos.X, prev.Pos.Y), rm);
                        var to = V2.Transform(new V2(node.Pos.X, node.Pos.Y), rm);
                        map_ptr.AddLine(from, to, Color.White.ToImgui());

                        prev = node;
                    }
                }
            }
            if (ui.nav.curPlayerNode != null) {
                var my_pos = V2.Transform(ui.me.gpos_f, rm);
                var ppos = V2.Transform(new V2(ui.nav.curPlayerNode.Pos.X, ui.nav.curPlayerNode.Pos.Y), rm);
                var pr = ui.sett.PlayerVisibilityRadius * 2;
                map_ptr.AddCircle(ppos, pr, Color.White.ToImgui());
                map_ptr.AddLine(my_pos, ppos, Color.GreenYellow.ToImgui());
            }
        }
    }
}
