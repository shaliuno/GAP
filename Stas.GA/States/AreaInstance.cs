using System.Collections.Concurrent;
using System.Diagnostics;
using ImGuiNET;
using static System.Runtime.InteropServices.JavaScript.JSType;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;
/// <summary>
///   [3] core.states.ingame_state.curr_area_instance  =>mapper
/// </summary>
public partial class AreaInstance : RemoteObjectBase {
    #region init
    internal AreaInstance(IntPtr address) : base(address, "AreaInstance") {
        environments = new();
        EntityCaches = new() {
            new("Breach", 1104, 1108, this.AwakeEntities),
            new("LeagueAffliction", 1114, 1114, this.AwakeEntities),
            new("Hellscape", 1244, 1255, this.AwakeEntities)
        };
    }
    #endregion
    public object my_pos_locker = new object();
    public List<V2> me_pos = new();
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero) {
            Clear();
            return;
        }
     
        var data = ui.m.Read<AreaInstanceOffsets>(Address);
        //AwakeEntities.Clear();
        //EntityCaches.ForEach((e) => e.Clear());
        //need_check.Clear();

        MonsterLevel = data.MonsterLevel;
        AreaHash = data.CurrentAreaHash;
        player.Tick(data.LocalPlayerPtr, tName + ".Tick");
        //Debug.Assert(player.gpos != default && player.pos != default);
        if (player.gpos_f != default) {
            lock (my_pos_locker) {
                me_pos.Add(player.gpos_f);
                if (me_pos.Count > 256) {
                    me_pos.RemoveAt(0);
                }
            }
        }

        try {
            //TODO same time here 'Collection was modified
            UpdateEnvironmentAndCaches(data.Environments);
        }
        catch (Exception ) {
            ui.AddToLog(tName + ".UpdateEnvironmentAndCaches ....", MessType.Critical);
        }
        server_data.Tick(data.ServerDataPtr, tName+ ".Tick");
        UpdateEntities(data.AwakeEntities);
    }
    protected override void Clear() {

    }

    string entityIdFilter = string.Empty;
    string entityPathFilter = string.Empty;
    bool filterByPath = false;
    StdVector environmentPtr = default;
    readonly List<int> environments;

    /// <summary>
    ///     Gets the Monster Level of current Area.
    /// </summary>
    public int MonsterLevel { get; private set; } = 0;

    /// <summary>
    ///     Gets the Hash of the current Area/Zone.
    ///     This value is sent to the client from the server.
    /// </summary>
    public uint AreaHash { get; private set; } = 0;

    /// <summary>
    ///     Gets the data related to the player the user is playing.
    /// </summary>
    public ServerData server_data { get; } = new(IntPtr.Zero);

    /// <summary>
    ///     Gets the player Entity.
    /// </summary>
    public Entity player { get; } = new();

    /// <summary>
    ///     Gets the Awake Entities of the current Area/Zone.
    ///     Awake Entities are the ones which player can interact with
    ///     e.g. Monsters, Players, NPC, Chests and etc. Sleeping entities
    ///     are opposite of awake entities e.g. Decorations, Effects, particles and etc.
    /// </summary>
    public ConcurrentDictionary<EntityNodeKey, Entity> AwakeEntities { get; } = new();

    /// <summary>
    ///     Gets important environments entity caches. This only contain awake entities.
    /// </summary>
    public List<DisappearingEntity> EntityCaches { get; }
  
    /// <summary>
    ///     Gets the terrain metadata data of the current Area/Zone instance.
    /// </summary>
    public TerrainStruct terr_meta_data { get; private set; } = default;

    /// <summary>
    ///     Gets the terrain height data.
    /// </summary>
    public float[][] height_data { get; private set; } = Array.Empty<float[]>();

    /// <summary>
    ///     Gets the terrain data of the current Area/Zone instance.
    /// </summary>
    public byte[] walkable_data { get; private set; } = Array.Empty<byte>();

    /// <summary>
    ///     Gets the Disctionary of Lists containing only the named tgt tiles locations.
    /// </summary>
    public Dictionary<string, List<V2>> TgtTilesLocations { get; private set; } = new();

    /// <summary>
    ///    Gets the current zoom value of the world.
    /// </summary>
    public float Zoom {
        get {
            if (player.GetComp(out Render render)) {
                var wp = render.WorldPosition;
                var p0 = ui.camera.WorldToScreen(wp);
                wp.Z += render.ModelBounds.Z;
                var p1 = ui.camera.WorldToScreen(wp);

                return Math.Abs(p1.Y - p0.Y) / render.ModelBounds.Z;
            }
            return 0;
        }
    }
    /// <summary>
    /// if ptr was set to def(from GameStates.Clear), here can be throw an exeption
    /// </summary>
    private void UpdateEnvironmentAndCaches(StdVector environments) {
        try {
            this.environments.Clear();
            this.environmentPtr = environments;
            var envData = ui.m.ReadStdVector<EnvironmentStruct>(environments);
            for (var i = 0; i < envData.Length; i++) {
                this.environments.Add(envData[i].Key);
            }
            this.EntityCaches.ForEach((eCache) => eCache.UpdateState(this.environments));
        }
        catch (Exception ex) {
            ui.AddToLog(tName + ".UpdateEnvironmentAndCaches ex=" + ex.Message);
        }
    }

    void AddToCacheParallel(EntityNodeKey key, string path) {
        for (var i = 0; i < this.EntityCaches.Count; i++) {
            if (this.EntityCaches[i].TryAddParallel(key, path)) {
                break;
            }
        }
    }
    private float[][] GetTerrainHeight() {
        byte[] rotationSelector = ui.RotationSelector.Values;
        byte[] rotationHelper = ui.RotatorHelper.Values;
        TileStructure[] tileData = ui.m.ReadStdVector<TileStructure>(terr_meta_data.TileDetailsPtr);
        var tileHeightCache = (from addr in tileData.Select((TileStructure x) => x.SubTileDetailsPtr).Distinct().AsParallel()
                               select new {
                                   addr = addr,
                                   data = ui.m.ReadStdVector<sbyte>(ui.m.Read<SubTileStructure>(addr).SubTileHeight)
                               }).ToDictionary(x => x.addr, x => x.data);
        int gridSizeX = terr_meta_data.NumCols * 23;
        int num = terr_meta_data.NumRows * 23;
        float[][] result = new float[num][];
        Parallel.For(0, num, delegate (int y) {
            result[y] = new float[gridSizeX];
            for (int i = 0; i < gridSizeX; i++) {
                int num2 = y / 23 * terr_meta_data.NumCols + i / 23;
                if (num2 < 0 || num2 >= tileData.Length) {
                    ui.AddToLog($"Tile data array length is {tileData.Length}, index was {num2}", MessType.Error);
                    result[y][i] = 0f;
                }
                else {
                    TileStructure tileStructure = tileData[num2];
                    sbyte[] array = tileHeightCache[tileStructure.SubTileDetailsPtr];
                    int num3 = 0;
                    if (array.Length == 1) {
                        num3 = array[0];
                    }
                    else if (array.Length != 0) {
                        int num4 = i % 23;
                        int num5 = y % 23;
                        int num6 = 22;
                        int[] obj = new int[4]
                        {
                            num6 - num4,
                            num4,
                            num6 - num5,
                            num5
                        };
                        int num7 = rotationSelector[tileStructure.RotationSelector] * 3;
                        int num8 = rotationHelper[num7];
                        int num9 = rotationHelper[num7 + 1];
                        int num10 = rotationHelper[num7 + 2];
                        int num11 = obj[num8 * 2 + num9];
                        int index = obj[num10 + (1 - num8) * 2] * 23 + num11;
                        num3 = GetTileHeightFromPackedArray(array, index);
                    }
                    result[y][i] = 0f - (float)((double)(tileStructure.TileHeight * terr_meta_data.TileHeightMultiplier + num3) * 7.8125);
                }
            }
        });
        return result;
    }
    private unsafe static int GetTileHeightFromPackedArray(sbyte[] tileHeightArray, int index) {
        object obj = tileHeightArray.Length switch {
            69 => (3, 2, 7, 1, 1, true),
            137 => (2, 4, 3, 2, 3, true),
            281 => (1, 16, 1, 4, 15, true),
            _ => default((int, int, int, int, int, bool)),
        };
        var (num, num2, num3, num4, num5, _) = ((int, int, int, int, int, bool))obj;
        if (!((ValueTuple<int, int, int, int, int, bool>*)(&obj))->Item6) {
            if (index >= 0 && index < tileHeightArray.Length) {
                return tileHeightArray[index];
            }
            ui.AddToLog($"Tile height array length is {tileHeightArray.Length}, index (0) was {index}", MessType.Error);
        }
        int num6 = (index >> num) + num2;
        if (num6 < 0 || num6 >= tileHeightArray.Length) {
            ui.AddToLog($"Tile height array length is {tileHeightArray.Length}, index (1) was {num6}", MessType.Error);
        }
        else {
            int num7 = ((byte)tileHeightArray[num6] >> (index & num3) * num4) & num5;
            if (num7 >= 0 && num7 < tileHeightArray.Length) {
                return tileHeightArray[num7];
            }
            ui.AddToLog($"Tile height array length is {tileHeightArray.Length}, index (2) was {num6}, {num7}", MessType.Error);
        }
        return 0;
    }
    void EntitiesWidget(string label, ConcurrentDictionary<EntityNodeKey, Entity> data) {
        if (ImGui.TreeNode($"{label} Entities ({data.Count})###${label} Entities")) {
            if (ImGui.RadioButton("Filter by Id           ", this.filterByPath == false)) {
                this.filterByPath = false;
                this.entityPathFilter = string.Empty;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Filter by Path", this.filterByPath)) {
                this.filterByPath = true;
                this.entityIdFilter = string.Empty;
            }

            if (this.filterByPath) {
                ImGui.InputText(
                    "Entity Path Filter",
                    ref this.entityPathFilter,
                    100);
            }
            else {
                ImGui.InputText(
                    "Entity Id Filter",
                    ref this.entityIdFilter,
                    10,
                    ImGuiInputTextFlags.CharsDecimal);
            }

            foreach (var entity in data) {
                if (!(string.IsNullOrEmpty(this.entityIdFilter) ||
                      $"{entity.Key.id}".Contains(this.entityIdFilter))) {
                    continue;
                }

                if (!(string.IsNullOrEmpty(this.entityPathFilter) ||
                      entity.Value.Path.ToLower().Contains(this.entityPathFilter.ToLower()))) {
                    continue;
                }

                if (ImGui.TreeNode($"{entity.Value.id} {entity.Value.Path}")) {
                    entity.Value.ToImGui();
                    ImGui.TreePop();
                }

                if (entity.Value.IsValid &&
                    entity.Value.GetComp<Render>(out var eRender)) {
                    ImGuiExt.DrawText(
                        eRender.WorldPosition,
                        $"ID: {entity.Key.id}");
                }
            }

            ImGui.TreePop();
        }
    }

    internal override void ToImGui() {
        base.ToImGui();
        if (ImGui.TreeNode("Environment Info")) {
            ImGuiExt.IntPtrToImGui("Address", this.environmentPtr.First);
            if (ImGui.TreeNode($"All Environments ({this.environments.Count})###AllEnvironments")) {
                for (var i = 0; i < this.environments.Count; i++) {
                    if (ImGui.Selectable($"{this.environments[i]}")) {
                        ImGui.SetClipboardText($"{this.environments[i]}");
                    }
                }

                ImGui.TreePop();
            }

            foreach (var eCache in this.EntityCaches) {
                eCache.ToImGui();
            }

            ImGui.TreePop();
        }

        ImGui.Text($"Area Hash: {this.AreaHash}");
        ImGui.Text($"Monster Level: {this.MonsterLevel}");
        ImGui.Text($"World Zoom: {this.Zoom}");
        if (ImGui.TreeNode("Terrain Metadata")) {
            ImGui.Text($"Total Tiles: {this.terr_meta_data.TotalTiles}");
            ImGui.Text($"Tiles Data Pointer: {this.terr_meta_data.TileDetailsPtr}");
            ImGui.Text($"Tiles Height Multiplier: {this.terr_meta_data.TileHeightMultiplier}");
            ImGui.Text($"Grid Walkable Data: {this.terr_meta_data.GridWalkableData}");
            ImGui.Text($"Grid Landscape Data: {this.terr_meta_data.GridLandscapeData}");
            ImGui.Text($"Data Bytes Per Row (for Walkable/Landscape Data): {this.terr_meta_data.BytesPerRow}");
            ImGui.TreePop();
        }

        if (this.player.GetComp<Render>(out var pPos)) {
            var y = (int)pPos.gpos_f.Y;
            var x = (int)pPos.gpos_f.X;
            if (y < this.height_data.Length) {
                if (x < this.height_data[0].Length) {
                    ImGui.Text("Player Pos to Terrain Height: " +
                               $"{this.height_data[y][x]}");
                }
            }
        }

        this.EntitiesWidget("Awake", this.AwakeEntities);
    }
}