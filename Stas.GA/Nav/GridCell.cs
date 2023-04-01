using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
public class GridCell : Cell {
    public bool b_calc_neighbour;
    public const int size = 23;
    public V2 my_pos;
    int[,] data => ui.curr_map.bit_data;
    public byte[,] checked_bit = new byte[size, size];
    bool check(int x, int y) => checked_bit[x, y] == 1;
    public int bit(int x, int y) => data[(int)min.X + x, (int)min.Y + y]; //3842 3059
    public List<GridCell> gcels_conncted = new List<GridCell>(); //only for wisual debug
    public List<Cell> blocks = new List<Cell>();
    public List<Cell> routs = new List<Cell>(); //836
    public List<Cell> border_routs = new List<Cell>();
    public float visited_persent = 0f;
    public float routs_percent;
    public float routs_weight_with_neibor;
    public TriggerableBlockage trigger;
    public bool b_trigger_corrected = false;
    public string path;
    public string tile_key;
    string _fn;
    public float routs_area { get; private set; }
    public string fname {
        get {
            if (path == null) 
                return null;
            if (_fn == null) {
                var pa = path.Split("/");
                _fn = pa[pa.Length - 1];
            }
            return _fn;
        }
    }
    public GridCell(int _id, V2 _min, V2 _max) : base(_min, _max) {
        id = _id;
        FillGrid();
    }
    public void FillGrid() {
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (!check(x, y)) {
                    var fb = bit(x, y); //first bit
                    if (fb < 2) { // is block
                        MakeCell(x, y, true);
                    }
                    else {
                        MakeCell(x, y, false);
                    }
                    //return; //for testing step by step
                }
            }
        }

        Debug.Assert(all_filled);
        routs_area = routs.Sum(r => r.area);
    }

    void MakeCell(int _x, int _y, bool is_block) {
        var lx = size;
        var ly = size;
        var nc = new Cell(_x + min.X, _y + min.Y, _x + 1 + min.X, _y + 1 + min.Y) {
            id = ui.nav.lcid,
            root = this
        };
        var y_stop = false;
        var stop = 0;
        for (int y = _y; y < ly; y++) {
            if (y_stop)
                break;
            for (int x = _x; x < lx; x++) {
                if (ok(x, y, is_block) && NotFoundInOld(x + 0.5f, y + 0.5f)) {
                    if (x == size - 1) {
                        stop = 1;
                    }
                    if (x == lx - 1) {
                        nc.max = new V2(x + 1 + min.X, y + 1 + min.Y);
                    }
                }
                else {
                    if (stop == 0) {
                        stop = 1;
                        lx = x;
                        nc.max = new V2(x + min.X, y + 1 + min.Y);
                        break;
                    }
                    else {
                        y_stop = true;
                        break;
                    }
                }
            }
        }
        if (is_block)
            blocks.Add(nc);
        else {
            routs.Add(nc);
            var crp = nc.area / (size * size);
            routs_percent += crp;
            if (nc.max.X == max.X || nc.max.Y == max.Y || nc.min.X == min.X || nc.min.Y == min.Y) {
                border_routs.Add(nc);
            }
        }
        ui.nav.lcid += 1;
        for (int y = (int)nc.min.Y; y < (int)nc.max.Y; y++) {
            for (int x = (int)nc.min.X; x < (int)nc.max.X; x++) {
                checked_bit[x - (int)min.X, y - (int)min.Y] = 1;
            }
        }
    }
    public Cell Get_rout_by_gp(V2 gp, string from) {
        if (gp == default) {
            ui.AddToLog(tName + ".Get_rout_by_gp=>"+ from + " gp==default", MessType.Error);
            return null;
        }
        foreach (var g in routs) {
            if (g.min.X <= gp.X && g.min.Y <= gp.Y && g.max.X >= gp.X && g.max.Y >= gp.Y)
                return g;
        }
        return null;
    }
    public Cell Get_block_by_gp(V2 gp, string from) {
        if (gp == default) {
            ui.AddToLog(tName + ".Get_block_by_gp=>"+ from + " gp==default", MessType.Error);
            return null;
        }
        return blocks.Find(g => g.min.X <= gp.X && g.min.Y <= gp.Y && g.max.X >= gp.X && g.max.Y >= gp.Y);
    }
    public void Set_all_cell_rout() {
        if (b_trigger_corrected)
            return;
        lock (blocks) {
            foreach (var b in blocks) {
                routs.Add(b);
            }
            blocks.Clear();
            b_trigger_corrected = true;
        }
    }

    bool all_filled {
        get {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    if (checked_bit[x, y] != 1)
                        return false;
                }
            }
            return true;
        }
    }
    public bool NotFoundInOld(float x, float y) {
        foreach (var b in blocks) {
            if (b.Contains(x + min.X, y + min.Y))
                return false;
        }
        foreach (var b in routs) {
            if (b.Contains(x + min.X, y + min.Y))
                return false;
        }
        return true;
    }

    bool ok(int x, int y, bool is_block) {
        if (bit(x, y) < 2 && is_block)
            return true;
        if (bit(x, y) > 1 && !is_block)
            return true;
        return false;
    }
    public void Reset() {
        checked_bit = new byte[size, size];
        routs.Clear();
        blocks.Clear();
    }
    public override string ToString() {
        return "min=" + min + " max=" + max + " id=" + id + " n=" + gcels_conncted.Count;
    }
}