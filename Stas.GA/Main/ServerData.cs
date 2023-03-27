using ImGuiNET;
using System.Diagnostics;

namespace Stas.GA;
/// <summary>
///     core.states.ingame_state.curr_area_instance.server_data
/// </summary>
public class ServerData : RemoteObjectBase {
    internal ServerData(IntPtr address) : base(address, "ServerData") {
        next_upd = DateTime.Now;//.AddMilliseconds(3);
    }
    public IList<ushort> skill_bar_ids { get; private set; } = new List<ushort>();
    IntPtr last_ptr = default;
    DateTime next_upd;
    bool still_update = false;
    object locker = new();
    internal override void Tick(IntPtr ptr, string from) {
        Address = ptr;
        if (Address == default) {
            Clear();
            return;
        }
        if (last_ptr != Address) { // only happens when area is changed.
            ClearCurrentlySelectedInventory();
            last_ptr = Address;
        }
        if (DateTime.Now < next_upd )
            return;
        var data = ui.m.Read<ServerDataStructure>(Address + ServerDataStructure.SKIP);
        var inventoryData = ui.m.ReadStdVector<InventoryArrayStruct>(data.PlayerInventories);
        next_upd = DateTime.Now.AddMilliseconds(200);
        //TODO twise load here on start - need fix
        Debug.Assert(still_update == false);
        if (still_update) {
            ui.AddToLog(tName + "still_update == true", MessType.Critical);
            return;
        }
        lock (locker) {
            still_update = true;
            this.PlayerInventories.Clear();
            for (var i = 0; i < inventoryData.Length; i++) {
                var invName = (InventoryName)inventoryData[i].InventoryId;
                var invAddr = inventoryData[i].InventoryPtr0;
                this.PlayerInventories[invName] = invAddr;
                switch (invName) {
                    case InventoryName.Flask1:
                        FlaskInventory.Tick(invAddr, "ServerData.Tick()");
                        break;
                }
            }
            still_update = false;
        }
        
        //GetPlayerInventoryItems(data);
        //GetNerestPlayers(data);
    }

    public List<Player> nearest_players { get; private set; } = new();
    List<Player> tmp_players = new List<Player>();
    void GetNerestPlayers(ServerDataStructure data) {
        const int structSize = 0x18;
        var first = data.NearestPlayers.First;
        var last = data.NearestPlayers.Last;

        if (first < 0 || last < 0 || (last - first) / structSize > 64) {
            nearest_players.Clear();
            return;
        }

        tmp_players.Clear();
        for (var playerAddress = first; playerAddress < last; playerAddress += structSize) {
            tmp_players.Add(new Player(ui.m.Read<IntPtr>(playerAddress)));
        }
        nearest_players = tmp_players;
    }
    private InventoryName selectedInvName = InventoryName.NoInvSelected;

   

    /// <summary>
    ///     Gets an object that points to the flask inventory.
    /// </summary>
    public Inventory FlaskInventory { get; } = new(IntPtr.Zero, "Flask");

    /// <summary>
    ///     Gets the inventory to debug.
    /// </summary>
    internal Inventory SelectedInv { get; } = new(IntPtr.Zero, "CurrentlySelected");

    /// <summary>
    ///     Gets the inventories associated with the player.
    /// </summary>
    Dictionary<InventoryName, IntPtr> PlayerInventories { get; } = new();

    void GetPlayerInventoryItems(ServerDataStructure data) {
        var inventoryData = ui.m.ReadStdVector<InventoryArrayStruct>(data.PlayerInventories);
        PlayerInventories.Clear();
        for (var i = 0; i < inventoryData.Length; i++) {
            var invName = (InventoryName)inventoryData[i].InventoryId;
            var invAddr = inventoryData[i].InventoryPtr0;
            PlayerInventories[invName] = invAddr;
            switch (invName) {
                case InventoryName.Flask1:
                    FlaskInventory.Tick(invAddr, tName+ ".GetPlayerInventoryItems");
                    break;
            }
        }
    }
    void GetSkillBarIds(ServerDataStructure data) {
        var curr_sb = data.SkillBarIds;
        var res = new List<ushort>{
                curr_sb.SkillBar1,
                curr_sb.SkillBar2,
                curr_sb.SkillBar3,
                curr_sb.SkillBar4,
                curr_sb.SkillBar5,
                curr_sb.SkillBar6,
                curr_sb.SkillBar7,
                curr_sb.SkillBar8,
                curr_sb.SkillBar9,
                curr_sb.SkillBar10,
                curr_sb.SkillBar11,
                curr_sb.SkillBar12,
                curr_sb.SkillBar13
            };
        skill_bar_ids = res;
    }


    private void ClearCurrentlySelectedInventory() {
        selectedInvName = InventoryName.NoInvSelected;
        SelectedInv.Tick(IntPtr.Zero, tName+ ".ClearCurrentlySelectedInventory");
    }
    /// <inheritdoc />
    protected override void Clear() {
        ClearCurrentlySelectedInventory();
        PlayerInventories.Clear();
        FlaskInventory.Tick(IntPtr.Zero, tName+ ".Clear");
    }
    internal override void ToImGui() {
        if ((int)selectedInvName > PlayerInventories.Count) {
            ClearCurrentlySelectedInventory();
        }

        ImGuiExt.IntPtrToImGui("Address", Address);
        if (ImGui.TreeNode("FlaskInventory")) {
            FlaskInventory.ToImGui();
            ImGui.TreePop();
        }

        ImGui.Text("please click Clear Selected before leaving this window.");
        if (ImGuiExt.IEnumerableComboBox(
            "###Inventory Selector",
            PlayerInventories.Keys,
            ref selectedInvName)) {
            SelectedInv.Tick(PlayerInventories[selectedInvName], tName+ ".ToImGui");
        }

        ImGui.SameLine();
        if (ImGui.Button("Clear Selected")) {
            ClearCurrentlySelectedInventory();
        }

        if (selectedInvName != InventoryName.NoInvSelected) {
            if (ImGui.TreeNode("Currently Selected Inventory")) {
                SelectedInv.ToImGui();
                ImGui.TreePop();
            }
        }
    }
}
