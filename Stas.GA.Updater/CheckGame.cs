using System.Diagnostics;
namespace Stas.GA.Updater;

public partial class ui {
    static string poe_p_name = "PathOfExile";
    static string poe_w_name = "Path of Exile";
    static void StartCheckGame() {
        //var sw = new Stopwatch();
       

        var poew_thread = new Thread(async () => {
            while (b_running) {
                try {
                    if (state != State.Authorized) {
                        ui.AddToLog("Waiting for authorization...");
                        Thread.Sleep(500);
                        continue;
                    }
                    curr_top_ptr = EXT.GetForegroundWindow(); // <1 ms
                    game_process = GetProcessByName(poe_p_name);
                    if (game_process == null) {
                        StartGame();
                    }
                }
                catch (Exception ex) {
                    AddToLog(ex.Message);
                    Thread.Sleep(5000);
                    continue;
                }
                Thread.Sleep(100);
            }
        });
        poew_thread.IsBackground = true;
        poew_thread.Start();
    }

    static  void StartGame() {
        if (!IsProcessOpen(poe_p_name)) {
            if (File.Exists(sett.game_bat_name)) {
                AddToLog("Starting the POE");
                Process.Start(sett.game_bat_name);
                while (!IsProcessOpen(poe_p_name)) {
                    AddToLog("waiting POE window...");
                    Thread.Sleep(1000);
                }
            }
            else {
                AddToLog("StartGame: file [" + sett.game_bat_name + "] not found");
                return;
            }
        }
    }

}
