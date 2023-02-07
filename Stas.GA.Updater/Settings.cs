using System.Text.Json.Serialization;

namespace Stas.GA.Updater;

public class Settings :iSave{

    public string boosty_name { get; set; } = "free";
    public string discord_name { get; set; } = "free";
    /// <summary>
    /// this IP is pinned in #authorization chanel
    /// </summary>
    public string srv_ip { get; set; } = "91.77.161.16";
    public int srv_port { get; set; } = 4444;
   
    public int log_size { get; set; }=15;
    [JsonInclude]
    public bool b_log_info = false;
    [JsonInclude]
    public bool b_log_warn = true;
    [JsonInclude]
    public bool b_log_error = true;
    public string title_name = "Updater for 8.0";
    public string local_dir  = @"C:\GameAssist\bin\";
    public string bot_exe_name => local_dir + @"Notepad.exe";
    //C:\Windows\System32\runas.exe /user:poe /savecred "cmd /C cd \"C:/Path of Exile/\" && PathOfExile_x64.exe"
    //https://www.ownedcore.com/forums/mmo/path-of-exile/poe-bots-programs/676345-run-poe-limited-user.html
    public string game_bat_name { get; set; } = @"C:\Stas\run.bat";
    /// <summary>
    /// restart bot if it not loaded
    /// </summary>
    public bool b_auto_bot  = true;
    /// <summary>
    /// restart game if it not loaded
    /// </summary>
    public bool b_auto_game  = true;
    ///// <summary>
    ///// restart game if it not loaded
    ///// </summary>
    //public bool b_auto_login = true;
    readonly string _fname;
    public override string fname => _fname;
    public Settings() {
        _fname = GetType().FullName+".sett";
    }
    public Settings Load(bool reload = false) {
        if(!File.Exists(fname) || reload) {
            FILE.SaveAsJson(this, fname);
            return this;
        }
        else
            return FILE.LoadJson<Settings>(fname);
    }
    public void Save() {
        FILE.SaveAsJson(this, fname);
    }
}

