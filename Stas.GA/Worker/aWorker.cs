using System.Diagnostics;

using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA; 

public abstract class aWorker :iSett{
    public aWorker(string name) : base(name) {

    }
    #region fields
    [JsonInclude]
    public bool b_use_life_flask = true;
    [JsonInclude]
    public Keys life_flask_key = Keys.E;
    [JsonInclude]
    public int life_flask_ms = 6000;
    [JsonInclude]
    public int min_life_percent = 55;

    [JsonInclude]
    public bool b_mana_use_auto = false;
    [JsonInclude]
    public bool b_use_mana_flask = true;
    [JsonInclude]
    public Keys mana_flask_key = Keys.F6;
    [JsonInclude]
    public int mana_flask_ms = 7000;

    [JsonInclude]
    public Keys silver_flask_key = Keys.F5;
    [JsonIgnore]
    protected List<aSkill> all_skills => new List<aSkill>() { 
        main, rb, mb, jump, d3, d4, d5, d6, d7, d8, d9, d0, totem, link, mark};
    [JsonIgnore]
    public List<aSkill> my_skills = new List<aSkill>();
    [JsonIgnore]
    public List<Keys> my_keys = new List<Keys>() { Keys.RButton }; //Keys.LButton
    [JsonIgnore]
    public aSkill main { get; set; }
    [JsonIgnore]
    public aSkill d4 { get; set; }
    [JsonIgnore]
    public aSkill mb { get; set; }
    [JsonIgnore]
    public aSkill rb { get; set; }
    [JsonIgnore]
    public aSkill d3 { get; set; }
    [JsonIgnore]
    public aTimeBuff BuffFromHit;
    [JsonIgnore]
    public aSkill d5 { get; set; }
    [JsonIgnore]
    public aSkill d6 { get; set; }
    [JsonIgnore]
    public aSkill d7 { get; set; }
    [JsonIgnore]
    public aSkill d8 { get; set; }
    [JsonIgnore]
    public aSkill d9 { get; set; }
    [JsonIgnore]
    public aSkill d0 { get; set; }
    [JsonIgnore]
    public aLink link { get; set; }
    [JsonIgnore]
    public aTotem totem { get; set; }
    [JsonIgnore]
    public tFlare flare { get; set; }
    [JsonIgnore]
    public tTNT tnt { get; set; }
    [JsonIgnore]
    public JumpSkill jump { get; set; }
    [JsonIgnore]
    public aMark mark { get; set; }
    public string test { get; set; }
    public Stopwatch sw = new Stopwatch();
    [JsonInclude]//only one party member can pull and be a tank
    public bool b_can_pull_alone;
    [JsonInclude]
    public bool b_open_door;
    [JsonIgnore]
    public bool b_moving => Mouse.IsButtonDown(Keys.LButton);
    public string my_log_id;
    public string bot_log_id;
    public string last_log_string;
    protected DateTime w8_until;
    public bool b_debug;
    protected aSkill curr; //which i am using now
    public Random R = new Random();
    /// <summary>
    /// for CI build true too
    /// </summary>
    public bool b_use_low_life;
    /// <summary>
    /// using for pull and retire
    /// </summary>
    public float max_danger;
    /// <summary>
    /// have tank buld and can opent doors and body pull
    /// </summary>
     #endregion

   
    protected void CountSkills() {
        foreach (var s in all_skills)
            if (s != null) {
                my_skills.Add(s);
                my_keys.Add(s.key);
            }
        if (b_can_pull_alone) {
            var pull = my_skills.FirstOrDefault(s => s.b_can_pull);
            Debug.Assert(pull != null);
        }
    }
    public void StartMoving(string from, bool debug = true) {
        if(!b_moving) {
            Mouse.LeftDown(from);
        }
    }
    public void StopMoving(string from, bool debug = false) {
        if (b_moving) { 
            Mouse.LeftUp(from+ "... StopMoving");
        }
    }
    public void ReliseKeys() {
        StopMoving("ReliseKeys");
        var keys = new List<Keys>() { Keys.LShiftKey, Keys.LControlKey, Keys.LMenu };
        keys.AddRange(my_skills.Select(s => s.key).ToList());
        foreach(var k in keys) {
            if(Keyboard.IsKeyDown(k))
                Keyboard.KeyUp(k);
        }
    }
    public void SavingAss() {
        Keyboard.KeyPress(Keys.Return);
        Keyboard.KeyPress(Keys.OemQuestion); //  "/exit"
        Keyboard.KeyPress(Keys.E);
        Keyboard.KeyPress(Keys.X);
        Keyboard.KeyPress(Keys.I);
        Keyboard.KeyPress(Keys.T);
        Keyboard.KeyPress(Keys.Return);
    }

}
