﻿using ImGuiNET;
namespace Stas.GA;
public class Portal : EntComp {
    public Portal(IntPtr address) : base(address) {
        _tname = "Portal";
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
    }
}
