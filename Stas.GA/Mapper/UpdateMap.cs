using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;
using Color = System.Drawing.Color;

namespace Stas.GA;
public partial class AreaInstance {
    Stopwatch sw = new Stopwatch();
    public IntPtr map_ptr;
    public float progress = 0f;
    public int rows;
    public int cols;
    public int[,] bit_data { get; private set; }
    public bool b_added_col { get; private set; }
    int make_ticks = 0; //for debug time creating
    /// <summary>
    /// after map creating only
    /// </summary>
    public bool b_ready { get; private set; }
    /// <summary>
    /// must be run in separate thread only
    /// </summary>
    public void UpdateMap(object state) {
        ClearOldData();
        UpdMapImage();
        ui.nav.b_ready = true;
        if (!ui.b_mine) {
            //ui.nav.LoadVisited();
            ui.curr_map.GetTileTgtName();
            ui.LoadQuest();
        }
        ui.area_change_counter.Tick(ui.area_change_counter.Address, "UpdateMap");
        ui.curr_loaded_files.Load(tName);
        //ui.looter.LoadOldLoot();
    }
    const string map_name = "walkable_map";
    object map_locker = new object();
    void UpdMapImage() {
        int curr_w8 = 0;
        int w8 = 3;
        ui.draw_main.RemoveImage(map_name);
        AreaInstanceOffsets data = default;
        while (data.TerrainMetadata.BytesPerRow <= 0
            || data.TerrainMetadata.GridWalkableData.Size <= 200 * 200
            || data.TerrainMetadata.GridWalkableData.Size > 4000 * 4000) {
            Thread.Sleep(w8);
            b_ready = false;
            ui.AddToLog(tName + ".UpdMapImage w8=[" + (curr_w8 += w8) + "]", MessType.Warning);
            ui.AddToLog(tName + ".UpdMapImage w8 right data...", MessType.Warning);
            if (Address != default)
                data = ui.m.Read<AreaInstanceOffsets>(Address);
        }
        terr_meta_data = data.TerrainMetadata;
        walkable_data = ui.m.ReadStdVector<byte>(terr_meta_data.GridWalkableData);
       
        sw.Restart();
        var td = terr_meta_data;
        cols = (int)td.TotalTiles.X * 23;
        rows = (int)td.TotalTiles.Y * 23;
        var bytesPerRow = td.BytesPerRow;
        Debug.Assert(bytesPerRow > 0);
        if ((cols & 1) > 0) {
            cols++;
            b_added_col = true;
        }
        else
            b_added_col = false;
        bit_data = new int[cols, rows];
        Configuration customConfig = Configuration.Default.Clone();
        customConfig.PreferContiguousImageBuffers = true;
        Image<Rgba32> image = new(customConfig, bytesPerRow * 2, walkable_data.Length / bytesPerRow);
        var walkArray = new WalkableFlag[cols, rows];
        var dataIndex = 0;
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x += 2) { //1794
                var b = walkable_data[dataIndex + (x >> 1)];
                var cp = b & 0xf;
                bit_data[x, y] = cp;
                image[x, y] = GetRgba32(cp);

                cp = (b >> 4);
                bit_data[(x + 1), y] = cp;
                image[x + 1, y] = GetRgba32(cp);
            }
            dataIndex += td.BytesPerRow;//897;
            progress = (float)y / rows;

        }

#if DEBUG
        //image.Save("current_map_" + ui.curr_map_hash + ".jpeg");
#endif
        ui.draw_main.AddOrGetImagePointer(map_name, image, false, out map_ptr); ;
        image.Dispose();
        ui.nav =new NavGrid(cols, rows, walkArray);
        b_ready = true;
        ui.AddToLog("Map create time=[" + sw.ElapsedTostring() + "]", MessType.Warning);
    }
    
    void ClearOldData() {
        ui.string_cashe.Clear();
        ui.sett.map_scale = ui.sett.map_scale_def;
        //quest_ent.Clear(); //mb don't
        b_ready = false;
        ui.w8ting_click_until.Clear();
        environmentPtr = default;
        environments.Clear();
        MonsterLevel = 0;
        AreaHash = 0;
        server_data.Tick(IntPtr.Zero, tName+ ".ClearOldData");
        //player.Tick(IntPtr.Zero, tName+ ".ClearOldData");
        terr_meta_data = default;
        height_data = Array.Empty<float[]>();
        walkable_data = Array.Empty<byte>();
        TgtTilesLocations.Clear();
        blight_pamp = null;
        blight_beams.Clear();
        bad_etypes.Clear();
        id_ifos.Clear();
        bad_map_items.Clear();
        static_items.Clear();
        make_ticks = 0;
        exped_keys.Clear();
        exped_beams.Clear();
        ui.nav.b_ready = false;//for not draw old visited
        ui.test?.spa?.Clear(); //debug data need only actuale
    }

    internal static Rgba32 GetRgba32(int i) {
        Rgba32 res;
        switch (i) {
            case 0:
                res = new Rgba32(0, 0, 0, 0);
                break;
            case 1:
                res = new Rgba32(255, 255, 255, 20);
                break;
            case 2:
                res = new Rgba32(255, 255, 255, 50);
                break;
            case 3:
                res = new Rgba32(255, 255, 255, 90);
                break;
            case 4:
                res = new Rgba32(255, 255, 255, 25);
                break;
            case 5:
                res = new Rgba32(255, 255, 255, 15);
                break;
            default:
                throw new Exception(i.ToString());
        }
        return res;
    }

    internal static Color GetColor(int i) {
        Color res;
        switch (i) {
            case 0:
                res = Color.FromArgb(0, 0, 0, 0);
                break;
            case 1:
                res = Color.FromArgb(20, 255, 255, 255);
                break;
            case 2:
                res = Color.FromArgb(50, 255, 255, 255);
                break;
            case 3:
                res = Color.FromArgb(90, 255, 255, 255);
                break;
            case 4:
                res = Color.FromArgb(25, 255, 255, 255);
                break;
            case 5:
                res = Color.FromArgb(15, 255, 255, 255);
                break;
            default:
                throw new Exception(i.ToString());
        }
        return res;
    }
}