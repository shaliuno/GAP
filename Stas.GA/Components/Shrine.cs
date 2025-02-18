﻿using ImGuiNET;
namespace Stas.GA;

public class Shrine : EntComp {
    public Shrine(IntPtr ptr) : base(ptr, "Shrine") {
        if (ptr != default) 
            Tick(ptr, tName + "()"); 
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<ShrineOffsets>(this.Address);
        this.IsUsed = data.IsUsed;
    }


    /// <summary>
    ///     Gets a value indicating whether Shrine is opened or not.
    /// </summary>
    public bool IsUsed { get; private set; }

    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Is Shrine Used: {this.IsUsed}");
    }
}