﻿using ImGuiNET;
namespace Stas.GA;
public class Portal : EntComp {
    public override string tName => "Portal";
    public Portal(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
}
