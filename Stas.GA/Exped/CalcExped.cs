using System.Threading.Tasks;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;

namespace Stas.GA;
public class ExpedKey {
    public ExpedKey(V3 pos, uint id) {
        this.pos = pos;
        this.id = id;
        var k = ui.worldToGridScale;
        gpos = new V2(pos.X * k, pos.Y * k);
    }
    public V3 pos { get; }
    public V2 gpos { get; }
    public uint id { get; }
}
public partial class AreaInstance {
    Dictionary<uint, ExpedKey> exped_key_frame = new();
    Dictionary<uint, Beam> beams_frame = new();
    public StaticMapItem exped_detonator => static_items.Values.FirstOrDefault(i => i.m_type == miType.ExpedDeton);
    //TODO exped_key_frame нужно сделать в кеш, чтобы они не пропадали если их не видно 
    public void CalcExped() {
        if (exped_detonator == null)
            return;
     
    }
}
