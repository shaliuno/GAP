﻿using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Stas.GA.Updater;

public partial class ui {
    
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetHWID")]
    public static extern uint GetHWID();
  
    internal static DrawMain overlay { get; set; } = null;
    public static string tName { get; } = "ui";
    public static int w8 { get; } = 16;////1000 / 60 = 16(60 frame sec)
    public static bool b_running  = true;
    public static Settings sett;
    static string bot_p_name => Path.GetFileNameWithoutExtension(sett.bot_exe_name);
    public static State curr_state = State.App_started;
    static bool b_all_updated = false;
    static uint hwid;//my pc id
    static ui() {
        sett = new Settings().Load<Settings>();
        sett.title_name = "Updater_29.01.txt";
        sett.b_auto_bot = true;
        //sett.Save();
        log = new FixedSizedLog(sett.log_size);
        hwid = GetHWID(); //3086599914
        RSA.TestCripto();
        Connect();
        StartCheckGame();
        StartCheckBot();
    }
  
    static bool b_must_be_transperent = false;
    public static void SetDebugPossible() {
        ui.overlay.SetOverlayClickable(false, false);
        Thread.Sleep(100);
    }
   
    static bool bad_process {
        get {
            try {
                return game_process == null
                       || game_process.HasExited
                       || game_process.MainWindowHandle.ToInt64() <= 0x00;
            }
            catch (Exception) {
                ui.AddToLog("game process err", MessType.Critical);
                return true;
            }
        }
    }
    static public IntPtr game_ptr {
        get {
            if (bad_process)
                return default;
            else
                return game_process.MainWindowHandle;
        }
    }
    public static Process game_process { get; private set; }
    public static IntPtr curr_top_ptr { get; private set; }
   
    #region LOG
    public static FixedSizedLog log { get; } 
    public static void ClearLog() {
        log.Clear();
    }

    public static void AddToLog(string str, MessType _mt = MessType.Ok) {
        log.Add(str, _mt);
    }
    #endregion


    static Process GetProcessByName(string pname) {
        var pa = Process.GetProcessesByName(pname);
        if (pa.Length == 0)
            return null;
        else
            return pa[0];
    }

    static bool KillBotExe(int maxSeconds) {
        var timer = new Stopwatch();
        timer.Start();
        var maxMs = maxSeconds * 1000;

        while (IsProcessOpen(bot_p_name) && timer.ElapsedMilliseconds < maxMs) {
            AddToLog($"Log -> Try to kill {bot_p_name}");
            KillProcess(bot_p_name);
            if (!IsProcessOpen(bot_p_name))
                return true;

            AddToLog($"Log -> {bot_p_name} is still running... {timer.ElapsedMilliseconds} / {maxMs}");
            Thread.Sleep(500);
        }
        return !IsProcessOpen(bot_p_name);
    }


    static void KillProcess(string name) {
        var processes = Process.GetProcessesByName(name);
        foreach (var process in processes) {
            process.Kill();
        }
    }
}
