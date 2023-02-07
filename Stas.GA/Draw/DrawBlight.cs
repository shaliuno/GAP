
using System.Drawing;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
public partial class DrawMain {
    void DrawBlight() {
        if (ui.b_contr_alt)
            return;
        var orange = Color.FromArgb(100, Color.Orange).ToImgui();
        var blue = Color.FromArgb(100, Color.Blue).ToImgui();
        var rm = ui.MTransform();
        var ls = (int)EXT.Lerp(5, 20, (float)ui.sett.map_scale / 20);
        foreach (var beam in ui.curr_map.blight_beams) {
            var bs3 = beam.BeamStart * ui.worldToGridScale;
            var bs2 = new V2(bs3.X, bs3.Y);
            var bs = V2.Transform(bs2, ui.MTransform());
            map_ptr.AddCircleFilled(bs, ls / 2, orange);

            var be3 = beam.BeamEnd * ui.worldToGridScale;
            var be2 = new V2(be3.X, be3.Y);
            var be = V2.Transform(be2, ui.MTransform());
            map_ptr.AddCircleFilled(be, ls / 2, orange);
            map_ptr.AddLine(bs, be, blue, ls);
        }
    }
}