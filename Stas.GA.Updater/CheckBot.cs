using Stas.Utils;
using System.Diagnostics;
namespace Stas.GA.Updater;

public partial class ui {
    static Dictionary<string, DateTime> files = new();
    static List<string> need_update = new();
    static void StartCheckBot() {
        var work_thread = new Thread(async () => {
            while (b_running) {
                if (curr_state != State.Authorized) {
                    ui.AddToLog("work_thread.w8 for authorization...");
                    Thread.Sleep(500);
                    continue;
                }
                b_all_updated = false;
                foreach (var f in files) {
                    //var vers  =FileVersionInfo.GetVersionInfo(net_fn);
                    var local_fn = Path.Combine(sett.local_dir, Path.GetFileName(f.Key));
                    if (!File.Exists(local_fn)) {
                        need_update.Add(f.Key);
                        continue;
                    }
                    var srv_dt = f.Value;
                    var local_dt = File.GetLastWriteTime(local_fn);
                    if (srv_dt > local_dt) {
                        need_update.Add(f.Key);
                    }
                }
                if (need_update.Count > 0) {
                    if (KillBotExe(3)) {
                        foreach (var f in need_update) {
                            await LoadFile(f);
                            var ndt = File.GetLastWriteTime(f);//new data for file
                            Debug.Assert(files[f] == ndt);
                        }
                        b_all_updated = true;
                    }
                    else {
                        AddToLog("[Error] Cant close" + sett.bot_exe_name);
                        Thread.Sleep(3000);
                        continue;
                    }
                }
                else {
                    b_all_updated = true;
                    AddToLog("Nothing changed");
                    AddToLog("Last cheked[" + DateTime.Now + "]");
                }
                var is_opened = IsProcessOpen(bot_p_name);
                if (!is_opened && b_all_updated && sett.b_auto_bot) {
                    AddToLog("Starting the Bot");
                    Process.Start(sett.bot_exe_name);
                    Thread.Sleep(5000);
                }
                Thread.Sleep(2000);
            }
        });
        work_thread.IsBackground = true;
        work_thread.Start();
    }
    static bool IsProcessOpen(string name) {
        var pa = Process.GetProcessesByName(name);
        return pa.Count() > 0;
    }
}
