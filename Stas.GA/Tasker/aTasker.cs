﻿
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public abstract partial class aTasker {
    string tName =>GetType().Name;  
    #region bool flags
    /// <summary>
    ///dont do quest/looting/etc if no danger, but fight and protect the Leader
    /// </summary>
    public bool b_fallow_hard { get; private set; }
    public bool b_hold => task != null && task.GetType() == typeof(Hold);
    #endregion
    List<aSkill> need_stop_cast;
    public aTask task { get; private set; }
    public ConcurrentBag<iTask> i_tasks = new ConcurrentBag<iTask>();
    protected ConcurrentStack<aTask> tasks = new ConcurrentStack<aTask>();
    public Dictionary<V2, Entity> checked_npc = new Dictionary<V2, Entity>();
    public Thread tasker_thread;
    protected abstract void MakeRoleTask();
    Stopwatch sw = new Stopwatch();
    List<double> elapsed = new();
    public void Tick() {
        if (ui.life != null) {
            UseLifeFlask();
            UseManaFlask();
        }
        while (ui.worker == null) {//it's possible right after the program start
            return;
        }
    }
    public void Add_iTask(iTask it) {
        i_tasks.Add(it);
    }
    public void Hold(string from = null) {
        //_fh = true;
        Reset("Hold... " + from);
        //ui.nav.SaveVisited();
        ui.tasker.TaskPop(new Hold());
    }
    public void Unhold(string from = null) {
        Reset("Unhold... " + from);
        b_fallow_hard = false;
    }
    public void SetFallowHard(bool b_set) {
        b_fallow_hard = b_set;
        ui.AddToLog("SetFallowHard.. b_set=[" + b_set + "]");

        if (b_hold) {
            TaskDone(task, "SetFallowHard");
        }
        if (!b_set)
            StopCastAll();
        else {
            if (this is Slave) {
                var ll = ui.leader;
                //For remove grace aura(if present) and visualise bot ready
                //TODO[2] worker can be null here after map change
                if (ll.b_OK && ll.gdist_to_me < 20 && ui.worker?.totem != null) {
                   // ui.tasker.TaskPop(new UseTotems(ll.pos));
                }
            }
        }
    }
   
    float min_dist = 8; //if party have 4x ; 6 if party have 2-3
    public float GetFallowDist { //max buff dist = 50
        get {
            if (ui.b_home)
                return 60;
            else {
                if (b_fallow_hard) {
                    return min_dist;
                }
                var jr = 46f;
                if (ui.worker.jump != null)
                    jr = ui.worker.jump.grange;
                if (ui.curr_map.danger == 0)
                    return 0.9f * jr + ui.sett.buff_gdist; //for faster return to buff distance
                else
                    return ui.sett.buff_gdist;
            }
        }
    }
    public void CleareDebug(aTask task) {
        Remove_iTaskById(task.id);
        ui.nav.b_ready = false;
        ui.test?.gpa?.Clear();
    }
    public void Remove_iTaskById(uint _id) {
        var tmp_list = i_tasks.Where(t => t.id != _id);
        i_tasks = new ConcurrentBag<iTask>(tmp_list);
    }
}
public enum TaskErrors {
    NoError, NavRes_no_path, im_stuck_here, i_m_dead, init_error, ent_error,
    TimeOut,
    Cant_get_Top,
    SetTop,
    skill_not_ready,
    cant_save_hit,
    Cant_see_tgp,
    tgp_is_zero
}

