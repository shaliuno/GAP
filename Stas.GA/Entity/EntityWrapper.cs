namespace Stas.GA;
public class EntityWrapper : Entity {

    public EntityWrapper(IntPtr address) : base(address) {
    }


    public override bool Equals(object obj) {
        EntityWrapper entityWrapper = obj as EntityWrapper;
        return entityWrapper != null && entityWrapper.id == base.id;
    }

    public override int GetHashCode() {
        return base.id.GetHashCode();
    }

    public override string ToString() {
        return base.Metadata;
    }

    private readonly bool _isInList = true;
}
