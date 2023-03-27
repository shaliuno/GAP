namespace Stas.GA;
internal class WorldItemNo : NetworkObject {
    public WorldItemNo(Entity entity) : base(entity) {
    }
    public Item Item {
        get {
            if (base.ew.GetComp<WorldItem>(out var wi)) {
                return new Item(wi.ItemEntity.Address, wi.ItemEntity.id, base.Entity.id);
            }
            return null;
        }
    }
}
