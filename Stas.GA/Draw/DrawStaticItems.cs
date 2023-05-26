using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;
using Stas.Utils;

namespace Stas.GA;
partial class DrawMain {
    void DrawStaticItems() {
        //if (ui.b_contrl)
        //    return;
        ///todo: need remake
        var sorted = ui.curr_map.static_items.Values.OrderBy(i => i.priority).ThenBy(i => i.gdist_to_me).ToArray();
        foreach (var mi in sorted) {
            var exped = mi.m_type == miType.ExpedArtifact
                    || mi.m_type == miType.ExpedMarker
                    || mi.m_type == miType.ExpedRemnant;
            if (exped && ui.curr_map.danger > 0)
                continue;
            switch (mi.m_type) {
                case miType.Archnemesis:
                    //if ((ent.IsValid && ent.IsDead) || !ent.IsValid) //
                    //    return ui.curr_map.static_items.TryRemove(key, out _);
                    //break;
                case miType.IncursionPortal:
                    //if (ent.GetComp<MinimapIcon>(out var icon) && icon.IsHide) {
                    //    return ui.curr_map.static_items.TryRemove(key, out _);
                    //}
                    break;
                case miType.Sulphite:
                case miType.portal:
                    //if (!ent.IsTargetable)
                    //    return ui.curr_map.static_items.TryRemove(key, out _);
                    break;
                case miType.Chest:
                default:
                    break;
            }
             DrawMapItem(mi, mi.uv, mi.size);
        }
    }
}
