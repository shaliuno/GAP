using System.Diagnostics;

namespace Stas.GA;
/// <summary>
///     Points to the item in the game.
///     Item is basically anything that can be put in the inventory/stash.
/// </summary>
public class Item : Entity {
    uint? _localId;
    private uint? _worldItemId;

    public Item(IntPtr address, uint? localId = null, uint? worldItemId = null) : base(address) {
        _localId = localId;
        _worldItemId = worldItemId;
    }
    public Item(IntPtr ptr, bool hasInventoryLocation, Vector2i locationBottomRight, Vector2i locationTopLeft) : base(ptr) {
        LocationBottomRight = locationBottomRight;
        LocationTopLeft = locationTopLeft;
        HasInventoryLocation = hasInventoryLocation;
        _localId = ui.m.Read<uint>(Address + 2920L); //7733358
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
    protected override void Clear() {
    }
    public bool IsValid {
        get {
            long addr = Address + 8L;
            var curr_val = ui.m.Read<int>(addr); //-2117284256
            return curr_val == 6619213;
        }
    }
    //EntCompInfo _components;
    //public EntCompInfo Components {
    //    get {
    //        if (_components == null) {
    //            _components = new EntCompInfo(this);
    //        }
    //        return _components;
    //    }
    //}
    DateTime last_meta_dt = DateTime.MinValue;
    string _md = null;
    public string metadata {
        get {
            //same like entcom =check comment there
            if (last_meta_dt == DateTime.MinValue) {
                var owner_ptr = ui.m.Read<IntPtr>(Address + 8);
                var m_ptr = ui.m.Read<IntPtr>(owner_ptr + 8);
                _md = ui.m.ReadStdWString(ui.m.Read<StdWString>(m_ptr + 8L));
                last_meta_dt = DateTime.Now;
            }
            return _md;
        }
    }
    public string RenderArt {
        get {
            Debug.Assert(this.Address != default);
            if (GetComp<RenderItem>(out var renderItemComponent)) {
                return renderItemComponent.ResourcePath;
            }
            return "";
        }
    }

    public Vector2i LocationBottomRight { get; }
    public Vector2i LocationTopLeft { get; }
    public bool HasInventoryLocation { get; }
}