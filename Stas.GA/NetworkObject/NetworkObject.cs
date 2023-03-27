using System.Runtime.InteropServices;

namespace Stas.GA;
public class NetworkObject : IEquatable<NetworkObject> {
    public override int GetHashCode() {
        if (!(ew != null)) {
            return 0;
        }
        return ew.GetHashCode();
    }

    public NetworkObject(EntityWrapper entity) {
        ew = entity;
    }

    public NetworkObject(Entity player) {
        EntityWrapper entity = new EntityWrapper(player.Address);
        ew = entity;
    }

    public NetworkObject(IntPtr address) {
        EntityWrapper entity = new EntityWrapper(address);
        ew = entity;
    }
    EntCompInfo _components;
    public EntCompInfo Components {
        get {
            if (_components == null) {
                _components = new EntCompInfo(ew);
            }
            return _components;
        }
    }
    public EntityWrapper ew { get; set; }
    public EntityWrapper Entity {
        get {
            return ew;
        }
    }

    public string AnimatedPropertiesMetadata {
        get {
            if (ew.GetComp<Animated>(out var anim)) {
                return anim.BaseAnimatedObjectEntity.Metadata;
            }
            return string.Empty;
        }
    }

    public bool BaseHashChanged {
        get {
            return false;
        }
    }

    public bool Equals(NetworkObject other) {
        return !Equals(other, null) && Entity.Address == other.Entity.Address;
    }

    public override bool Equals(object obj) {
        return obj != null && (this == (NetworkObject)obj || !(obj.GetType() != GetType()) && Equals((NetworkObject)obj));
    }

    public static bool operator ==(NetworkObject left, NetworkObject right) {
        return Equals(left, right);
    }
    public static bool operator !=(NetworkObject left, NetworkObject right) {
        return !Equals(left, right);
    }
    internal NetworkObject ConvertNetworkObject() {
        if (ew == null || ew.Address == 0L) {
            return null;
        }
        return this;
    }


    // Token: 0x020004C8 RID: 1224
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Struct105 {
        // Token: 0x04002704 RID: 9988
        public IntPtr Struct133_intptr_0;

        // Token: 0x04002705 RID: 9989
        public StdVector Struct133_StdVector_0;

        // Token: 0x04002706 RID: 9990
        public StdVector StdVector_0;

        // Token: 0x04002707 RID: 9991
        public IntPtr intptr_0;

        // Token: 0x04002708 RID: 9992
        public int int_0;

        // Token: 0x04002709 RID: 9993
        public byte byte_0;

        // Token: 0x0400270A RID: 9994
        internal byte byte_1;

        // Token: 0x0400270B RID: 9995
        internal byte byte_2;

        // Token: 0x0400270C RID: 9996
        internal byte byte_3;

        // Token: 0x0400270D RID: 9997
        public uint uint_0;

        // Token: 0x0400270E RID: 9998
        public IntPtr intptr_1;

        // Token: 0x0400270F RID: 9999
        public IntPtr intptr_2;

        // Token: 0x04002710 RID: 10000
        public IntPtr intptr_3;

        // Token: 0x04002711 RID: 10001
        internal Struct252 struct252_0;

        // Token: 0x04002712 RID: 10002
        internal Struct252 struct252_1;

        // Token: 0x04002713 RID: 10003
        internal Struct252 struct252_2;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Struct252 {
        public int int_0;

        public int int_1;

        public Struct251 struct251_0;

        public Struct251 struct251_1;

        public Struct251 struct251_2;

        public Struct251 struct251_3;

        public Struct251 struct251_4;

        public Struct251 struct251_5;

        public Struct251 struct251_6;

        public int int_2;

        public int int_3;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Struct251 {
        public long intptr_0;

        public long intptr_1;
    }
}