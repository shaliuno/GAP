using System.Diagnostics;

namespace Stas.Utils;
/// <summary>
/// thread safe perform checker, based on ui.w8 const
/// Do't Restart() this in parallel loops
/// DEbug loading items from the collection must be in flat looping
/// </summary>
public class SW : Stopwatch {
    /// <summary>
    /// for reset max_ft after use cleare the log
    /// </summary>
    public static Dictionary<string, SW> registred = new();
    string name { get; }
    List<double> elapsed = new List<double>();
    /// <summary>
    /// max frame time this session
    /// </summary>
    double max_ft = 0;
    public void Restart(bool full = false) {
        base.Restart();
        if (full) {
            elapsed.Clear();
            max_ft = 0;
        }
    }
    int max_coolect = 100;
    public SW(string _name, int max_coolect = 100) {
        name = _name;
        this.max_coolect = max_coolect;
        registred[name] = this;
    }
    /// <summary>
    /// only returns the last value calculated in MakeRes.
    /// U need call MakeRes() same frame befor call here</summary>
    public string GetRes => res;
    /// <summary>
    /// MakeRes +AddToLog
    /// </summary>
    public void Print() {
        MakeRes();
        ut.AddToLog(res, MessType.TimeDebug);
    }
    string res;
    //we need this method to calculate the value separately and output it for debugging
    public void MakeRes() {
        var elaps = Elapsed.TotalMilliseconds;

        lock (elapsed) {
            elapsed.Add(elaps);
            if (elapsed.Count > max_coolect)
                elapsed.RemoveAt(0);
            var ft = elapsed.Sum() / elapsed.Count;//frame time
            if (ft > max_ft) {
                max_ft = ft;
            }
            var fps = (1000f / ft).ToRoundStr(0);
            res = (name + " max=[" + max_ft.ToRoundStr(3) + "]ms curr=[" + ft.ToRoundStr(3) + "]ms fps=[" + fps + "]");
        }
    }
}