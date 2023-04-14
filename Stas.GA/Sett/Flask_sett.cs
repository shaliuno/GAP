namespace Stas.GA;

public partial class Settings  {
    [JsonInclude]
    public bool b_use_left_flasks = false;
    [JsonInclude]
    public bool b_use_right_flasks = false;
    [JsonInclude]
    public bool b_use_life_flask = false;
    [JsonInclude]
    public bool b_use_mana_flask = false;
    [JsonInclude]
    public int min_life_percent = 55;
    [JsonInclude]
    public int trigger_life_left_persent = 50;
    [JsonInclude]
    public int mana_cast_price = 20;
    [JsonInclude]
    public int mana_flask_ms = 7000;
    [JsonInclude]
    public int life_flask_ms = 3000;
    [JsonInclude]
    public Keys two_left_flask_key = Keys.Q;
    [JsonInclude]
    public Keys two_right_flask_key = Keys.W;
    [JsonInclude]
    public Keys mana_flask_key = Keys.F6;
    [JsonInclude]
    public Keys life_flask_key = Keys.E;
    [JsonInclude]
    public bool b_mana_use_auto = false;
  
    [JsonInclude]
    public bool b_use_silver_flask = false;
    [JsonInclude]
    public int silver_gdist = 100;
    [JsonInclude]
    public Keys silver_flask_key = Keys.F5;
}