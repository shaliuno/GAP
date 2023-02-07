using Stas.GA;
using Stas.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;
class Starter{
    static void Main() {
        var b_dll_ok = false;
        var game_process = Process.GetProcessesByName("PathOfExile")[0];
        if (Start(game_process.Id, true) == 0) {
            b_dll_ok = true;
        }
        else {
            ui.AddToLog("Can't start Native.Dll", MessType.Critical);
        }
    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "Start")]
    static extern int Start(int pid, bool debug);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetLifeOffsets")]
    static extern IntPtr GetLifeOffsets(IntPtr ptr, ref LifeOffset offs);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "HardExit")]
    static extern int HardExit();
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetGuiOffsets")]
    static extern int GetGuiOffsets(IntPtr gui_ptr, ref guiOffset offs);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetEscPtr")]
    static extern IntPtr GetEscPtr();
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPassiveTreePtr")]
    static extern IntPtr GetPassiveTreePtr();
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPlInvPtr")]
    static extern IntPtr GetPlInvPtr();
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetExpedMarkerWrap")]
    static extern int GetExpedMarkerWrap(IntPtr anim_ptr, ref MapItemWrap miw);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetExpedRelicWrap")]
    static extern int GetExpedRelicWrap(IntPtr omp_ptr, ref MapItemWrap miw);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetCameraOffsets")]
    static extern int GetCameraOffsets(IntPtr cam_ptr, IntPtr ingame_state_ptr, ref CameraOffsets offs);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPatt")]
    static extern int GetPatt(ref Patt_wrapp res);
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "MapWasChanged")]
    static extern void MapWasChanged();
}

