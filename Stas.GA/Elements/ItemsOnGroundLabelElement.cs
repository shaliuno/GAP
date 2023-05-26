using System.Diagnostics;
namespace Stas.GA;

public class ItemsOnGroundLabelElement : Element {
    CachedValue<ItemsOnGroundLabelElementOffsets> _data;
    public ItemsOnGroundLabelElement(nint ptr, string name = "ItemsOnGroundLabelElement") :base(ptr, name) {
        _data = new FrameCache<ItemsOnGroundLabelElementOffsets>(() 
            => ui.m.Read<ItemsOnGroundLabelElementOffsets>(Address)); 
    }
    public Element LabelOnHover {
        get {
            var readObjectAt = new Element(_data.Value.LabelOnHoverPtr);
            return readObjectAt.Address == default ? null : readObjectAt;
        }
    }

    public Entity ItemOnHover {
        get {
            var ptr = ui.m.Read<IntPtr>(_data.Value.ItemOnHoverPtr);
            var readObjectAt = new Entity(ptr);
            return readObjectAt.Address == default ? null : readObjectAt;
        }
    }

    public string ItemOnHoverPath => ItemOnHover != null ? ItemOnHover.Path : "Null";
    public string LabelOnHoverText => LabelOnHover != null ? LabelOnHover.Text : "Null";
    public int CountLabels => ui.m.Read<int>(Address + 0x2B0);
    public int CountLabels2 => ui.m.Read<int>(Address + 0x2F0);

    public List<LabelOnGround> LabelsOnGround {
        get {
            var address = ui.m.Read<IntPtr>(Address + 0x2A0);//0x000002b31af74180
            var result = new List<LabelOnGround>();

            if (address.ToInt64() <= 0)
                return new List<LabelOnGround>();

            var limit = 0;

            for (var i = ui.m.Read<long>(address); i != address.ToInt64(); i = ui.m.Read<long>(i)) {
                var labelOnGround = new LabelOnGround(new nint(i), i.ToString());
                if (labelOnGround?.Label?.IsValid ?? false) {
                    result.Add(labelOnGround);
                    var ent = labelOnGround.ItemOnGround;
                    Debug.Assert(ent != null && ent.IsValid);
                }
                   

                limit++;

                if (limit > 100000)
                    return new List<LabelOnGround>();
            }

            return result;
        }
    }
}