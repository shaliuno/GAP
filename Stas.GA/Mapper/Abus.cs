using System.Collections.Concurrent;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
using System.Drawing;

namespace Stas.GA;
public partial class AreaInstance {
    MapItem GetAbyss(Entity e, MapItem mi) {
        // ui.tasker.TaskPop(new Abyss(e));
        e.GetComp<Transitionable>(out var transit);
        e.GetComp<MinimapIcon>(out var icon);
        var pa = e.Path.Split('/');
        mi.info = pa[pa.Length - 1];
        if (mi.info == "AbyssStartNode") { //1 154 trans flags - not opened 10 67 after
            if (transit.Flag1 != 1)
                return null;
            mi.uv = sh.GetUV(MapIconsIndex.AbyssStart);
            mi.size = 20;
        }
        else if (e.Path.Contains("AbyssCrackSpawners") || e.Path.Contains("AbyssNode")
                || e.Path.Contains("AbyssNodeSmall")) {
            if (icon != null && icon.IsHide == true) //|| transit.Flag1 != 1 << alot throw exepption
                return null;
            mi.uv = sh.GetUV(MapIconsIndex.AbyssCrack);
        }
        else if ((e.Path.Contains("Final") && e.Path.Contains("Chest")) ) {
            if (!e.IsTargetable)
                return null; //we will see that in chest part too()
            else {
                mi.uv = sh.GetUV(MapIconsIndex.RewardNiceBox);
                mi.size = 20;
            }
        }
        else if (e.Path.Contains("AbyssSubAreaTransition")) {
            mi.uv = sh.GetUV(MapIconsIndex.Green_door);
            mi.size = 26;
        }
        else {
            if (ui.sett.b_develop)
                mi.uv = sh.GetUV(MapIconsIndex.unknow);
            else
                return null;
        }
        mi.size = 20;
        return mi;
    }
}
