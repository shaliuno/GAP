namespace Stas.GA; 

internal class LimitedLifespan : RemoteObjectBase {
    public Entity parent { get; } = new Entity();
    public LimitedLifespan(IntPtr ptr):base(ptr, "LimitedLifespan") {
    }
    public void TryFindeLInkedEnt() {
        var start = Address.ToInt64();
        for (var i = start; i < start + 1024; i += 8) {
            var ent = new Entity( ui.m.Read<IntPtr>(i));
            if (ent.IsValid ) {
                var offs = i - start;
            }
        }
    }

    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
    
    protected override void Clear() {
        ui.AddToLog(tName + ".CleanUpData need implement", MessType.Critical);
    }
}
