using Color = System.Drawing.Color;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;


namespace Stas.GA;

public partial class DrawMain {
    float TNT_r => 30 + (30 * ui.exped_sett.radius_persent / 100);//default radius of explosion

    void DrawExped() {
        var detor = ui.curr_map.exped_detonator;
        if (ui.b_contr_alt || detor == null)
            return;
    }
}

