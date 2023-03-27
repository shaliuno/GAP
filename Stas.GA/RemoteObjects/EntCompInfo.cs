namespace Stas.GA;
public class EntCompInfo : RemoteObjectBase {
    Entity ent;

    public EntCompInfo(EntityWrapper entity) : base(entity.Address) {
        Address = entity.Address;
        ent = entity;
    }

    public EntCompInfo(Entity entity) : base(entity.Address) {
        Address = entity.Address;
        ent = entity;
    }

    public EntCompInfo(Item entity) : base(entity.Address) {
        Address = entity.Address;
        ent = new EntityWrapper(Address);
    }

    public EntCompInfo(IntPtr address) : base(address) {
        Address = address;
        ent = new EntityWrapper(Address);
    }
    internal override void Tick(nint ptr, string from = null) {
       
    }
    protected override void Clear() {
      
    }
    public RenderItem RenderItemComponent {
        get {
            if( this.ent.GetComp<RenderItem>(out var ri))
                return ri;
            return null;
        }
    }
}
