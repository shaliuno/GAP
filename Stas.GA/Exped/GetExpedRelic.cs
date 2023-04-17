using System.Runtime.InteropServices;
using System.IO;

namespace Stas.GA;
public partial class AreaInstance {
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetExpedRelicWrap")]
    public static extern int GetExpedRelicWrap(IntPtr omp_ptr, ref MapItemWrap miw);

    public unsafe MapItem GetExpedRelic(Entity e) {
        var miw = new MapItemWrap();
        var omp_ptr = e.componentAddresses["ObjectMagicProperties"];
#if DEBUG
        e.GetComp<ObjectMagicProperties>(out var omp);
        var mods = omp.Mods;
        if (mods.Count > 0) { 
            
        }
#endif
        GetExpedRelicWrap(omp_ptr, ref miw);
        var info = Marshal.PtrToStringUni(miw.info_ptr);
        var remn = new Remnant();
        if (miw.remn_ptr == default) //free version
            return null;
        using (var ums = new UnmanagedMemoryStream((byte*)miw.remn_ptr, miw.remn_size, miw.remn_size, FileAccess.Read)) {
            var ba = new byte[miw.remn_size];
            ums.Read(ba, 0, miw.remn_size);
            remn.FillFromByteArray(ba);
        }
        return asStaticMapItem(e, miw.type, miw.index, info, miw.priority, remn);
    }   
}

