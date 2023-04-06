using System.Diagnostics;
namespace Stas.Utils;

public abstract class iSave {
    public virtual string fname => _tname + ".sett";
    public virtual string tname => _tname;
    public DateTime created_date { get; }
    string _tname;
    public iSave(string tname) { //_extension
        _tname = tname;
        created_date = DateTime.Now;
    }
    public virtual T Load<T>(Action if_err = null) where T : iSave, new() {
        try {
            if (!File.Exists(fname)) {
                ut.AddToLog(tname + ".loading err: Not founf=[" + fname + "]", MessType.Error);
                FILE.SaveAsJson(this, fname);
                Console.WriteLine(tname + ". Load OK");
                return new T();
            }
            else {
                Console.WriteLine(tname + ". Load OK");
                return FILE.LoadJson<T>(fname, if_err);
            }
                
        }
        catch (Exception ex) {
            Console.WriteLine(tname+". Load err="+ex.Message);
            if (if_err == null) {
                File.Delete(fname);
                FILE.SaveAsJson(this, fname);
                return new T();
            }
            else {
                if_err.Invoke();
                return null;
            }
        }
    }
    public virtual void Save() {
        FILE.SaveAsJson(this, fname);
    }
    public override string ToString() {
        return tname;
    }
}
public abstract class iSett : iSave {
    public iSett(string name):base(name) { }
    public override T Load<T>(Action if_err = null) {
        Debug.Assert(!string.IsNullOrEmpty(fname) && fname.EndsWith(".sett"));
        return base.Load<T>(if_err);
    }
    public override void Save() {
        Debug.Assert(!string.IsNullOrEmpty(fname) && fname.EndsWith(".sett"));
        base.Save();
    }
}