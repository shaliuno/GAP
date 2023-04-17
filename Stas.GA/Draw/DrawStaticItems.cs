using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;

namespace Stas.GA;
partial class DrawMain {
    void DrawStaticItems() {
        //if (ui.b_contrl)
        //    return;
        var sorted = ui.curr_map.static_items.Values.OrderBy(i => i.priority).ThenBy(i => i.gdist_to_me).ToArray();
        foreach (var mi in sorted) {
            var exped = mi.m_type == miType.ExpedArtifact
                    || mi.m_type == miType.ExpedMarker
                    || mi.m_type == miType.ExpedRemnant;
            if (exped && ui.curr_map.danger > 0)
                continue;
            if (!mi.WasDeleted())
                DrawMapItem(mi, mi.uv, mi.size);
        }
    }
}
