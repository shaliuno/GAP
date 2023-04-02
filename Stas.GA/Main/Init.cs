using Stas.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Stas.GA;
public partial class ui {
    public static float danger => curr_map.danger;
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "Start")]
    public static extern int Start(int pid, bool debug);
    public static IntPtr first_children_offset = 0x30;
    public static int elem_text_offs = 0x3F8;
    public static int IsVisibleLocalOffs = 0x161;
    static Dictionary<string, MapIconsIndex> Icons;
    public static Dictionary<string, string> gui_offs_nams = new();
    public static ConcurrentDictionary<IntPtr, DateTime> w8ting_click_until = new();
    /// <summary>
    ///Necessary for fast UpdateComponentData.  Must be cleared for each new map
    /// </summary>
    public static StringCache string_cashe = new();
    public static MapIconsIndex IconIndexByName(string name) {
        name = name.Replace(" ", "").Replace("'", "");
        Icons.TryGetValue(name, out var result);
        return result;
    }
    static Thread frame_thread;
    public static Memory m { get; private set; }
    public static int w8 { get; } = 16;////1000 / 60 = 16(60 frame sec)
    public static List<Keys> flask_keys = new();
    //public static AHK ahk { get; private set; }
    public static Inventory flasks => curr_map.server_data.FlaskInventory;
    static Stopwatch sw_main = new Stopwatch();
    public static HotKeysFromGame hot_keys;
    static InputChecker input_check;
    public static SafeScreen safe_screen;
    static int init_count = 0;
    public static void InitNative(int pid) {
        Console.WriteLine("InitNative started=[" + (init_count++) + "]");
        //ReloadSett();
        m = new Memory(pid);   
    }
   
    public static void Init() {
        int w8_err = 300;
        Icons = new Dictionary<string, MapIconsIndex>(200);
        var icons = Enum.GetValues(typeof(MapIconsIndex));
        foreach (var icon in icons) {
            Icons[icon.ToString()] = (MapIconsIndex)icon;
        }
        sett = new Settings().Load<Settings>();
        hot_keys = new HotKeysFromGame();
        var elaps = 0; var add_w8 = 5; var max_w8 = 50;
        while (hot_keys.use_bound_skill1.Key == Keys.None) {
            Thread.Sleep(add_w8);
            elaps += add_w8;
            ui.AddToLog(tName + " w8tiing hot_keys init...");
            if (elaps > max_w8) {
                ui.AddToLog(tName + "HotKeysFromGame not load - check settings..", MessType.Critical);
                break;
            } 
        }
        flask_keys.AddRange(new List<Keys>(){hot_keys.use_flask_in_slot1.Key, hot_keys.use_flask_in_slot2.Key,
            hot_keys.use_flask_in_slot3.Key, hot_keys.use_flask_in_slot4.Key,hot_keys.use_flask_in_slot5.Key});
 
        udp_sound = new UdpSound();
        safe_screen = new SafeScreen();
        StartGameWatcher();
        SetRole();
        //looter = new Looter();
        //ahk = new AHK();
        input_check = new InputChecker();
        need_upd_per_frame = new List<RemoteObjectBase>() {   }; //camera//gui
        var game_not_loadin = 0;

        frame_thread = new Thread(() => {
            while (b_running) {
                curr_frame += 1;
                if (game_ptr == IntPtr.Zero) {
                    game_not_loadin += 1;
                    if (game_not_loadin > 100) {
                        AddToLog("w8 game not loading... ", MessType.Critical);
                        AddToLog("Use [Alt]+[Shift] to activate this window", MessType.Critical);
                    }
                    Thread.Sleep(400);
                    continue;
                }
                sw_main.Restart();
                if(states.b_ready)
                    states.Tick(states.Address, tName + ".main");
               
                if (curr_state == gState.InGameState) {
                    foreach (var n in need_upd_per_frame)
                        n?.Tick(n.Address, tName + ".main");
                    CheckWorker();
                    //todo: temporary dont need a worker
                    tasker.Tick();
                    if (worker == null) {
                        if(!sett.b_debug_native_dll)
                            ui.AddToLog(tName + ".main worker need be setup", MessType.Warning);
                        Thread.Sleep(w8);
                        continue;
                    }
                    //CheckFlasks(false); //not right offset jet
                    //CheckMapPlayers();//not right offset jet
                }

                #region tick timer & w8ting for relax CPU
                var d_elaps = sw_main.Elapsed.TotalMilliseconds;
                elapsed.Add(d_elaps);
                if (elapsed.Count > 60)
                    elapsed.RemoveAt(0);
                var frame_time = elapsed.Sum() / elapsed.Count;
                if (frame_time < w8) {
                    Thread.Sleep(w8 - (int)frame_time);
                }
                else {
                    Thread.Sleep(1);
                    AddToLog("Input: Big Tick Time", MessType.Error);
                }
                #endregion
            }
        });
        frame_thread.IsBackground = true;
        frame_thread.Start();
    }
    public static void ReloadSett() {
        sett = new Settings().Load<Settings>();
        //TODO cant use it - w8 GH relise
        sett.b_use_gh_flask = false;
        sett.b_use_gh_map = false;
        sett.b_develop = true; //<<==change if you're a developer
        exped_sett = new ExpedSett().Load<ExpedSett>();
    }
    public static void Dispose() {
        CloseGame();
        try {
            b_running = false;
        }
        catch (Exception) {
        }
    }
}

[Flags]
public enum Role {
    None = 0,
    Slave = 1,
    Master = 2,//pull manual or auto
    AiBot = 4,//AI auto farm
    Coollector = 8,
    //Trader = 16 //now separated application on phone
}


