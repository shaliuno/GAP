namespace Stas.GA;
internal class Projectile : EntComp {

    public Projectile(IntPtr ptr) : base(ptr) {
        _tname = "Positioned";
        if (ptr != default)
            Tick(ptr, tName + "()");
    }
   
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
  
    protected override void Clear() {
        base.Clear();
    }
}
